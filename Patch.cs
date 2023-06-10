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
        static Type effectType = typeof(ActiveHealthControllerClass).GetNestedTypes().First(t => t.GetProperty("Strength") != null);
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
                        // creates cooldown for getting another painkiller effect! pk is the painkiller effect
                         var pk = typeof(ActiveHealthControllerClass).GetMethod("FindActiveEffect", BindingFlags.Instance | BindingFlags.Public).MakeGenericMethod(typeof(ActiveHealthControllerClass).GetNestedType("PainKiller", BindingFlags.Instance | BindingFlags.NonPublic)).Invoke(__instance.ActiveHealthController, new object[] { EBodyPart.Head });

                         if ((int)effectType.GetProperty("TimeLeft").GetValue(pk) < duration) 
                             effectType.GetMethod("AddWorkTime").Invoke(pk, new object[] { duration, false });
                         
                        return;
                    }
                    
                    MethodInfo method = typeof(ActiveHealthControllerClass).GetMethod("method_15", BindingFlags.Instance | BindingFlags.NonPublic);
                    //here the actual painkiller effect is created
                    //object asdf = ActiveHealthControllerClass.GClass2102.Create(0, "PainKiller", 0, );

                    duration = AdrenalineDuration.Value;

                    effectMethod.MakeGenericMethod(typeof(ActiveHealthControllerClass).GetNestedType("PainKiller", BindingFlags.Instance | BindingFlags.NonPublic)).Invoke(__instance.ActiveHealthController, new object[] { EBodyPart.Head, 0f, duration, 5f, 1f, null });

                }
                catch (Exception) { }
            }
        }
    }
}