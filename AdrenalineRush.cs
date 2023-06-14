using EFT;
using System;
using System.Linq;
using System.Reflection;
using Aki.Reflection.Patching;
using Adrenaline.Configs;
using UnityEngine;
using BepInEx.Logging;

namespace Adrenaline
{
	public class AdrenalinePatch : ModulePatch
	{
	
		static MethodInfo effectMethod = typeof(ActiveHealthControllerClass).GetMethods(BindingFlags.Instance | BindingFlags.NonPublic).First(m =>
			m.GetParameters().Length == 6
			&& m.GetParameters()[0].Name == "bodyPart"
			&& m.GetParameters()[5].Name == "initCallback"
			&& m.IsGenericMethod);
		//override default GetTargetMethod()
		protected override MethodBase GetTargetMethod()
		{
			return typeof(Player).GetMethod("ReceiveDamage", BindingFlags.Instance | BindingFlags.NonPublic);
		}

		static float cooldown = 0f;
		static float duration = 100f;

		[PatchPostfix]

		static void Postfix(ref Player __instance, EDamageType type)
		{
			if (!AdrenalineConfig.EnableMod.Value || !__instance.IsYourPlayer)
			{
				if (AdrenalineConfig.EnableLog.Value)
				{
					Logger.LogInfo("--- ADRENALINERUSH ---");
					Logger.LogInfo("EnableMod.Value: " + AdrenalineConfig.EnableMod.Value);
					Logger.LogInfo("__instance.IsYourPlayer: " + __instance.IsYourPlayer);
					Logger.LogInfo("skip rush at " + Time.time + " with cooldown " + cooldown);
					Logger.LogInfo("----------------------");
				}

				return;
			}
			//problem: currently checks all players (including scavs) for receivedmg, so 
			//some random npc will get the adrenalinerush and it will go on cooldown
			//GameWorld.MainPlayer?
			//what is the correct way to access player?
			if (type == EDamageType.Bullet || type == EDamageType.Explosion || type == EDamageType.Sniper || type == EDamageType.Landmine || type == EDamageType.GrenadeFragment || type == EDamageType.Blunt)
			{
				if (AdrenalineConfig.EnableLog.Value)
				{
					Logger.LogInfo("--- ADRENALINERUSH ---");
					Logger.LogInfo("Took Damage of type: " + type);
				}
				//checks cooldown and all effects on head for painkiller effect. if there are, dont give adrenaline
				if (Time.time < cooldown || __instance.ActiveHealthController.BodyPartEffects.Effects[EBodyPart.Head].Any(eff => eff.Key == "PainKiller" || eff.Key == "TunnelVision"))
				{
					if (AdrenalineConfig.EnableLog.Value)
						Logger.LogInfo("skip rush at " + Time.time + " with cooldown " + cooldown);

					return;
				}
				else
				{
					// next possible adrenalinerush = painkiller + (tunnelvision) + cooldown duration

					duration = AdrenalineConfig.AdrenalineDuration.Value;
					cooldown = Time.time + AdrenalineConfig.AdrenalineCooldown.Value + duration;
					//if (getDebug();)
					if (AdrenalineConfig.EnableLog.Value)
						Logger.LogInfo("new rush at " + Time.time + " with cooldown " + cooldown);
					
					//here the actual painkiller effect is created
					//EBodyPart bodyPart, float? delayTime = null, float? workTime = null, float? residueTime = null, float? strength = null, Action<T> initCallback = null
					try
					{
						effectMethod.MakeGenericMethod(typeof(ActiveHealthControllerClass).GetNestedType("PainKiller", BindingFlags.Instance | BindingFlags.NonPublic)).Invoke(__instance.ActiveHealthController, new object[] { EBodyPart.Head, 0f, duration, 2f, 1f, null });
						//Add TunnelVision after PainKiller
						if (!AdrenalineConfig.DisableDrawbacks.Value)
						{
							effectMethod.MakeGenericMethod(typeof(ActiveHealthControllerClass).GetNestedType("TunnelVision", BindingFlags.Instance | BindingFlags.NonPublic)).Invoke(__instance.ActiveHealthController, new object[] { EBodyPart.Head, duration, duration / 2f, 2f, 1f, null });
						}
						{
							effectMethod.MakeGenericMethod(typeof(ActiveHealthControllerClass).GetNestedType("Pain", BindingFlags.Instance | BindingFlags.NonPublic)).Invoke(__instance.ActiveHealthController, new object[] { EBodyPart.Head, duration, duration / 2f, 2f, 1f, null });
						}
					}
					catch (Exception ex) { Logger.LogError("AdrenalineRush Exception " + ex); }
				}
			}
		}
	}
}
