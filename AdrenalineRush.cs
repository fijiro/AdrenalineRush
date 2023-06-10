using EFT;
using System;
using System.Linq;
using System.Reflection;
using Aki.Reflection.Patching;
using static Adrenaline.Configs.AdrenalineConfig;
using UnityEngine;

namespace Adrenaline
{
	public class AdrenalinePatch : ModulePatch
	{
		static float cooldown;
		//i have no clue what this does :D
		static MethodInfo effectMethod = typeof(ActiveHealthControllerClass).GetMethods(BindingFlags.Instance | BindingFlags.NonPublic).First(m =>
			m.GetParameters().Length == 6
			&& m.GetParameters()[0].Name == "bodyPart"
			&& m.GetParameters()[5].Name == "initCallback"
			&& m.IsGenericMethod
		);
		//or this
		protected override MethodBase GetTargetMethod() => typeof(Player).GetMethod("ReceiveDamage", BindingFlags.Instance | BindingFlags.NonPublic);

		[PatchPostfix]

		static void Postfix(ref Player __instance, EDamageType type)
		{
			if (EnableMod.Value == false) return;

			float duration = AdrenalineDuration.Value;
			// next possible adrenalinerush = painkiller + tunnelvision + cooldown duration
			cooldown = Time.time + 1.5f * duration + AdrenalineCooldown.Value;

			if (type == EDamageType.Bullet || type == EDamageType.Explosion || type == EDamageType.Sniper || type == EDamageType.Landmine || type == EDamageType.GrenadeFragment)
			{
				try
				{
					//checks cooldown and all effects on head for painkiller effect. if there are, dont give adrenaline
					if (Time.time > cooldown && !__instance.ActiveHealthController.BodyPartEffects.Effects[EBodyPart.Head].Any(eff => eff.Key == "PainKiller"))
					{
						//here the actual painkiller effect is created. Cooldown when?
						//{BodyPart, start time, duration, fade out duration, ??, ??}
						effectMethod.MakeGenericMethod(typeof(ActiveHealthControllerClass).GetNestedType("PainKiller", BindingFlags.Instance | BindingFlags.NonPublic)).Invoke(__instance.ActiveHealthController, new object[] { EBodyPart.Head, 0f, duration, 2f, 1f, null });
						//Add Tunnelvision after painkiller
						effectMethod.MakeGenericMethod(typeof(ActiveHealthControllerClass).GetNestedType("TunnelVision", BindingFlags.Instance | BindingFlags.NonPublic)).Invoke(__instance.ActiveHealthController, new object[] { EBodyPart.Head, duration, 5 + duration / 2f, 5f, 100f, null });
					}
				}
				catch (Exception)
				{
					Logger.LogWarning("AdrenalineRush Exception");
				}
			}
		}
	}
}