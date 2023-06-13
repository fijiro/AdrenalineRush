using EFT;
using System;
using System.Linq;
using System.Reflection;
using Aki.Reflection.Patching;
using Adrenaline.Configs;
using UnityEngine;

namespace Adrenaline
{
	public class AdrenalinePatch : ModulePatch
	{
		//
		//static Type effectType = typeof(ActiveHealthControllerClass).GetNestedTypes().First(t => t.GetProperty("Strength") != null);
		static MethodInfo effectMethod = typeof(ActiveHealthControllerClass).GetMethods(BindingFlags.Instance | BindingFlags.NonPublic).First(m =>
			m.GetParameters().Length == 6
			&& m.GetParameters()[0].Name == "bodyPart"
			&& m.GetParameters()[5].Name == "initCallback"
			&& m.IsGenericMethod);
		//override default GetTargetMethod()
		protected override MethodBase GetTargetMethod()
		{
			return typeof(Player).GetMethod("ApplyDamageInfo", BindingFlags.Instance | BindingFlags.NonPublic);
		}

		static float cooldown = 0f;
		static float duration = 100f;

		[PatchPostfix]

		static void Postfix(ref Player __instance, EDamageType type)
		{
			if (AdrenalineConfig.EnableMod.Value == false) return;

			Logger.LogInfo("--- ADRENALINERUSH ---");
			Logger.LogInfo("Took Damage of type: " + type);
			Logger.LogInfo("----------------------");

			if (type == EDamageType.Bullet || type == EDamageType.Explosion || type == EDamageType.Sniper || type == EDamageType.Landmine || type == EDamageType.GrenadeFragment || type == EDamageType.Blunt)
			{
				//checks cooldown and all effects on head for painkiller effect. if there are, dont give adrenaline
				if (Time.time < cooldown || __instance.ActiveHealthController.BodyPartEffects.Effects[EBodyPart.Head].Any(eff => eff.Key == "PainKiller" || eff.Key == "TunnelVision"))
				{
					Logger.LogInfo("--- ADRENALINERUSH ---");
					Logger.LogInfo("skip rush at " + Time.time + " with cooldown " + cooldown);
					Logger.LogInfo("----------------------");
					return;
				}
				else
				{
					// next possible adrenalinerush = painkiller + (tunnelvision) + cooldown duration

					duration = AdrenalineConfig.getDuration();
					cooldown = Time.time + AdrenalineConfig.getCooldown() + duration;
					//if (getDebug();)
					Logger.LogInfo("--- ADRENALINERUSH ---");
					Logger.LogInfo("new rush at " + Time.time + " with cooldown " + cooldown);
					Logger.LogInfo("----------------------");

					//here the actual painkiller effect is created
					//EBodyPart bodyPart, float? delayTime = null, float? workTime = null, float? residueTime = null, float? strength = null, Action<T> initCallback = null
					try
					{
						effectMethod.MakeGenericMethod(typeof(ActiveHealthControllerClass).GetNestedType("PainKiller", BindingFlags.Instance | BindingFlags.NonPublic)).Invoke(__instance.ActiveHealthController, new object[] { EBodyPart.Head, 0f, duration, 2f, 1f, null });
						//Add TunnelVision after PainKiller
						effectMethod.MakeGenericMethod(typeof(ActiveHealthControllerClass).GetNestedType("TunnelVision", BindingFlags.Instance | BindingFlags.NonPublic)).Invoke(__instance.ActiveHealthController, new object[] { EBodyPart.Head, duration, duration / 2f, 2f, 1f, null });
					}
					catch (Exception ex) { Logger.LogError("AdrenalineRush Exception " + ex); }
				}
			}
		}
	}
}
