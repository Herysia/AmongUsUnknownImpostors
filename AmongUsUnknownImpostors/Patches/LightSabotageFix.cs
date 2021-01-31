using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using UnityEngine;

namespace AmongUsUnknownImpostors.Patches
{
    class LightSabotageFix
    {
        [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.CalculateLightRadius))]
        public static class ShipStatus_CalculateLightRadius
        {
            public static bool Prefix(ShipStatus __instance, GameData.PlayerInfo IIEKJBMPELC, ref float __result)
            {
                if (!OptionsPatches.unkImpostor) return true;
                var player = IIEKJBMPELC;
                if (player == null || player.IsDead)
                {
                    __result = __instance.MaxLightRadius;
                    return false;
                }

                SwitchSystem switchSystem = __instance.Systems[SystemTypes.Electrical].Cast<SwitchSystem>();
                float num = (float) switchSystem.Value / 255f;
                if (player.IsImpostor)
                {
                    __result = Mathf.Lerp(__instance.MinLightRadius, __instance.MaxLightRadius, num) *
                               Mathf.Lerp(OptionsPatches.impoVision, PlayerControl.GameOptions.ImpostorLightMod, num);
                    return false;
                }

                __result = Mathf.Lerp(__instance.MinLightRadius, __instance.MaxLightRadius, num) *
                           PlayerControl.GameOptions.CrewLightMod;
                return false;
            }
        }
    }
}