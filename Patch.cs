using EFT;
using System;
using System.Linq;
using System.Reflection;
using Aki.Reflection.Patching;
using static Adrenaline.Configs.AdrenalineConfig;

namespace Adrenaline
{
	public class AdrenalinePatch : ModulePatch
	{
		static MethodInfo effectMethod = typeof(ActiveHealthControllerClass).GetMethods(BindingFlags.Instance | BindingFlags.NonPublic).First(m =>
			m.GetParameters().Length == 6
			&& m.GetParameters()[0].Name == "bodyPart"
			&& m.GetParameters()[5].Name == "initCallback"
			&& m.IsGenericMethod
		);
		protected override MethodBase GetTargetMethod() => typeof(Player).GetMethod("ReceiveDamage", BindingFlags.Instance | BindingFlags.NonPublic);

		[PatchPostfix]
		static void Postfix(ref Player __instance, EDamageType type)
		{
			if (EnableMod.Value == false) return;

			float duration = AdrenalineDuration.Value;

			if (type == EDamageType.Bullet || type == EDamageType.Explosion || type == EDamageType.Sniper || type == EDamageType.Landmine || type == EDamageType.GrenadeFragment)
			{
				try
				{
					//checks all effects on head for painkiller effect. if there are, dont give adrenaline
					if (__instance.ActiveHealthController.BodyPartEffects.Effects[EBodyPart.Head].Any(eff => eff.Key == "PainKiller"))
					{
						return;
					}

					//here the actual painkiller effect is created
					duration = AdrenalineDuration.Value;
					effectMethod.MakeGenericMethod(typeof(ActiveHealthControllerClass).GetNestedType("PainKiller", BindingFlags.Instance | BindingFlags.NonPublic)).Invoke(__instance.ActiveHealthController, new object[] { EBodyPart.Head, 0f, duration, 5f, 1f, null });

				}
				catch (Exception) { }
			}
		}
	}
}