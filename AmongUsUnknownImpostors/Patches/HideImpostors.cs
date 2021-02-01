using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using HarmonyLib;
using UnhollowerBaseLib;
using UnityEngine;

namespace AmongUsUnknownImpostors.Patches
{
    internal class HideImpostors
    {
        [HarmonyPatch(typeof(ChatController), nameof(ChatController.AddChat))]
        public static class ChatController_AddChat
        {
            public static void Postfix(ChatController __instance, ref PlayerControl KMCAKLLFNIM)
            {
                PlayerControl sourcePlayer = KMCAKLLFNIM;

                if (!sourcePlayer || !PlayerControl.LocalPlayer)
                {
                    return;
                }

                GameData.PlayerInfo data = PlayerControl.LocalPlayer.Data;
                GameData.PlayerInfo data2 = sourcePlayer.Data;
                if (data2 == null || data == null || (data2.IsDead && !data.IsDead))
                {
                    return;
                }

                var activeChildren = __instance.chatBubPool.activeChildren;

                ChatBubble chatBubble = activeChildren[activeChildren.Count - 1].Cast<ChatBubble>();

                if (data2.IsImpostor && data2.Object != PlayerControl.LocalPlayer)
                    chatBubble.NameText.Color = UnityEngine.Color.white;
            }
        }

        //Patch that makes impostors along in intro cutscene
        [HarmonyPatch(typeof(IntroCutscene), nameof(IntroCutscene.BeginImpostor))]
        public static class IntroCutscene_BeginImpostor
        {
            public static void Prefix(IntroCutscene __instance,
                ref Il2CppSystem.Collections.Generic.List<PlayerControl> KADFCNPGKLO)
            {
                if (!CustomGameOptionsData.customGameOptions.unkImpostor.value) return;
                var yourTeam = KADFCNPGKLO;
                yourTeam.Clear();
                yourTeam.Add(PlayerControl.LocalPlayer);
            }
        }

        //Patch that hide other impostors in Meeting HUD
        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Method_7))]
        public static class MeetingHud_CreateButton
        {
            public static void Postfix(MeetingHud __instance, GameData.PlayerInfo PPIKPNJEAKJ,
                ref PlayerVoteArea __result)
            {
                if (!CustomGameOptionsData.customGameOptions.unkImpostor.value) return;
                GameData.PlayerInfo playerInfo = PPIKPNJEAKJ;
                if (playerInfo.IsImpostor && playerInfo.Object != PlayerControl.LocalPlayer)
                    __result.NameText.Color = UnityEngine.Color.white;
            }
        }

        //Patch that fixes Kill button between impostors
        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FindClosestTarget))]
        public static class PlayerControl_FindClosestTarget
        {
            private static int ShipAndObjectsMask = LayerMask.GetMask(new string[]
            {
                "Ship",
                "Objects"
            });

            public static bool Prefix(PlayerControl __instance, ref PlayerControl __result)
            {
                if (!CustomGameOptionsData.customGameOptions.unkImpostor.value) return true;
                PlayerControl result = null;
                float num = GameOptionsData.KillDistances[
                    Mathf.Clamp(PlayerControl.GameOptions.KillDistance, 0, 2)];
                if (!ShipStatus.Instance)
                {
                    return true;
                }

                Vector2 truePosition = __instance.GetTruePosition();
                for (int i = 0; i < GameData.Instance.AllPlayers.Count; i++)
                {
                    GameData.PlayerInfo playerInfo = GameData.Instance.AllPlayers[i];
                    if (!playerInfo.Disconnected && playerInfo.PlayerId != __instance.PlayerId &&
                        !playerInfo.IsDead)
                    {
                        PlayerControl @object = playerInfo.Object;
                        if (@object)
                        {
                            Vector2 vector = @object.GetTruePosition() - truePosition;
                            float magnitude = vector.magnitude;
                            if (magnitude <= num && !PhysicsHelpers.AnyNonTriggersBetween(truePosition,
                                vector.normalized, magnitude, ShipAndObjectsMask))
                            {
                                result = @object;
                                num = magnitude;
                            }
                        }
                    }
                }

                __result = result;
                return false;
            }
        }

        //Patch that sets player name color when impostors are chosen
        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.SetInfected))]
        public static class PlayerControl_RpcSetInfected
        {
            public static void Postfix(PlayerControl __instance, Il2CppStructArray<byte> JPGEIBIBJPJ)
            {
                if (!CustomGameOptionsData.customGameOptions.unkImpostor.value) return;
                var infected = JPGEIBIBJPJ;
                for (int j = 0; j < infected.Length; j++)
                {
                    GameData.PlayerInfo playerById2 = GameData.Instance.GetPlayerById(infected[j]);
                    if (playerById2 != null && playerById2.Object != PlayerControl.LocalPlayer)
                    {
                        playerById2.Object.nameText.Color = UnityEngine.Color.white;
                    }
                }
            }
        }
    }
}