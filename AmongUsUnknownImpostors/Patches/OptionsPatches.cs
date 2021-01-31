using HarmonyLib;
using Hazel;
using Il2CppSystem.IO;
using System;
using System.Linq;
using Il2CppDumper;
using UnhollowerBaseLib;
using UnityEngine;
using Reactor;

namespace AmongUsUnknownImpostors.Patches
{
    //Code from : https://github.com/Galster-dev/CrowdedSheriff/blob/master/src/OptionsPatches.cs <3
    internal class OptionsPatches
    {
        public static bool unkImpostor = false;
        public static float impoVision = 1.0f;

        const StringNames unkImpostorTitle = (StringNames) 2000;
        const StringNames impoVisionTitle = (StringNames) 2001;

        [HarmonyPatch(typeof(TranslationController), nameof(TranslationController.GetString),
            new Type[] {typeof(StringNames), typeof(Il2CppReferenceArray<Il2CppSystem.Object>)})]
        static class TranslationController_GetString
        {
            public static bool Prefix(StringNames HKOIECMDOKL, ref string __result)
            {
                switch (HKOIECMDOKL)
                {
                    case unkImpostorTitle:
                        __result = "Unk impostor";
                        break;
                    case impoVisionTitle:
                        __result = "Impostor light off vision";
                        break;
                    default:
                        return true;
                }

                return false;
            }
        }

        [HarmonyPatch(typeof(GameOptionsMenu), nameof(GameOptionsMenu.Start))]
        static class GameOptionsMenu_Start
        {
            public static void OnValueChanged(OptionBehaviour option)
            {
                if (!AmongUsClient.Instance || !AmongUsClient.Instance.AmHost) return;
                switch (option.Title)
                {
                    case unkImpostorTitle:
                        unkImpostor = option.GetBool();
                        break;
                    case impoVisionTitle:
                        impoVision = option.GetFloat();
                        break;
                }

                if (PlayerControl.GameOptions.isDefaults)
                {
                    PlayerControl.GameOptions.isDefaults = false;
                    UnityEngine.Object.FindObjectOfType<GameOptionsMenu>().Method_16(); //RefreshChildren
                }

                var local = PlayerControl.LocalPlayer;
                if (local != null)
                {
                    local.RpcSyncSettings(PlayerControl.GameOptions);
                }
            }

            static float GetLowestConfigY(GameOptionsMenu __instance)
            {
                return __instance.GetComponentsInChildren<OptionBehaviour>()
                    .Min(option => option.transform.localPosition.y);
            }

            [HarmonyPriority(Priority.Normal - 2)]
            static void Postfix(ref GameOptionsMenu __instance)
            {
                var lowestY = GetLowestConfigY(__instance);

                var toggleOption = UnityEngine.Object.Instantiate(__instance.GetComponentsInChildren<ToggleOption>()[1],
                    __instance.transform);
                toggleOption.transform.localPosition = new Vector3(toggleOption.transform.localPosition.x,
                    lowestY - 0.5f, toggleOption.transform.localPosition.z);
                toggleOption.Title = unkImpostorTitle;
                toggleOption.CheckMark.enabled = unkImpostor;
                var str = "";
                TranslationController_GetString.Prefix(toggleOption.Title, ref str);
                toggleOption.TitleText.Text = str;
                toggleOption.OnValueChanged = new Action<OptionBehaviour>(OnValueChanged);
                toggleOption.gameObject.AddComponent<OptionBehaviour>();

                var countOption = UnityEngine.Object.Instantiate(
                    GameObject.FindObjectsOfType<NumberOption>().ToList()
                        .First(x => x.TitleText.Text == "Impostor Vision"),
                    __instance.transform);
                countOption.transform.localPosition = new Vector3(countOption.transform.localPosition.x,
                    lowestY - 1f, countOption.transform.localPosition.z);
                countOption.Title = impoVisionTitle;
                countOption.Value = impoVision;
                var str3 = "";
                TranslationController_GetString.Prefix(countOption.Title, ref str3);
                countOption.TitleText.Text = str3;
                countOption.OnValueChanged = new Action<OptionBehaviour>(OnValueChanged);
                countOption.gameObject.AddComponent<OptionBehaviour>();
                countOption.Increment = 0.25f;
                countOption.ValidRange.min = 0.25f;
                countOption.ValidRange.max = 5.0f;

                __instance.GetComponentInParent<Scroller>().YBounds.max += 1.0f;
            }
        }

        [HarmonyPatch(typeof(GameSettingMenu), nameof(GameSettingMenu.OnEnable))]
        static class GameSettingsMenu_OnEnable
        {
            static void Prefix(ref GameSettingMenu __instance)
            {
                __instance.HideForOnline = new Il2CppReferenceArray<Transform>(0);
            }
        }

        [HarmonyPatch(typeof(NumberOption), nameof(NumberOption.OnEnable))]
        static class NumberOption_OnEnable
        {
            static bool Prefix(ref NumberOption __instance)
            {
                if (__instance.Title == impoVisionTitle)
                {
                    string smh = "";
                    TranslationController_GetString.Prefix(__instance.Title, ref smh);
                    __instance.TitleText.Text = smh;
                    __instance.OnValueChanged = new Action<OptionBehaviour>(GameOptionsMenu_Start.OnValueChanged);
                    __instance.Value = impoVision;
                    __instance.enabled = true;

                    return false;
                }

                return true;
            }
        }

        [HarmonyPatch(typeof(ToggleOption), nameof(ToggleOption.OnEnable))]
        static class ToggleOption_OnEnable
        {
            static bool Prefix(ref ToggleOption __instance)
            {
                if (__instance.Title == unkImpostorTitle)
                {
                    string str = "";
                    TranslationController_GetString.Prefix(__instance.Title, ref str);
                    __instance.TitleText.Text = str;
                    __instance.CheckMark.enabled = unkImpostor;
                    __instance.OnValueChanged = new Action<OptionBehaviour>(GameOptionsMenu_Start.OnValueChanged);
                    __instance.enabled = true;

                    return false;
                }

                return true;
            }
        }

        [HarmonyPatch(typeof(GameOptionsData), nameof(GameOptionsData.Method_24))]
        static class GameOptionsData_ToHudString
        {
            [HarmonyPriority(Priority.Normal - 2)]
            static void Postfix(ref string __result)
            {
                var builder = new System.Text.StringBuilder(__result);
                builder.AppendLine();
                builder.AppendLine($"Unk impostor: {unkImpostor}");
                if (unkImpostor)
                {
                    builder.Append("Impostor light off vision: ");
                    builder.Append(impoVision);
                    builder.Append("x");
                    builder.AppendLine();
                }
                __result = builder.ToString();
            }
        }

        [HarmonyPatch(typeof(GameOptionsData), nameof(GameOptionsData.Method_65), typeof(BinaryReader))]
        static class GameOptionsData_Deserialize
        {
            static void Postfix(BinaryReader ALMCIJKELCP)
            {
                try
                {
                    unkImpostor = ALMCIJKELCP.ReadBoolean();
                }
                catch
                {
                    unkImpostor = false;
                }

                try
                {
                    impoVision = System.BitConverter.ToSingle(ALMCIJKELCP.ReadBytes(4).ToArray(), 0);
                }
                catch
                {
                    impoVision = 0.5f;
                }
            }
        }

        [HarmonyPatch(typeof(GameOptionsData), nameof(GameOptionsData.Method_7), typeof(MessageReader))]
        static class GameOptionsData_DeserializeM
        {
            static void Postfix(MessageReader ALMCIJKELCP)
            {
                try
                {
                    unkImpostor = ALMCIJKELCP.ReadBoolean();
                }
                catch
                {
                    unkImpostor = false;
                }

                try
                {
                    impoVision = System.BitConverter.ToSingle(ALMCIJKELCP.ReadBytes(4).ToArray(), 0);
                }
                catch
                {
                    impoVision = 1.0f;
                }
            }
        }

        [HarmonyPatch(typeof(GameOptionsData), nameof(GameOptionsData.Method_53),
            new Type[] {typeof(BinaryWriter), typeof(byte)})]
        static class GameOptionsData_Serialize
        {
            static void Postfix(BinaryWriter AGLJMGAODDG)
            {
                AGLJMGAODDG.Write(unkImpostor);
                AGLJMGAODDG.Write(impoVision);
            }
        }
    }
}