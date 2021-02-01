using AmongUsUnknownImpostors.Patches;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.IL2CPP;
using HarmonyLib;
using Reactor;

namespace AmongUsUnknownImpostors
{
    [BepInPlugin(Id)]
    [BepInProcess("Among Us.exe")]
    [BepInDependency(ReactorPlugin.Id)]
    public class UnknownImpostorsPlugin : BasePlugin
    {
        public const string Id = "com.herysia.amongusunkimpostor";
        public static byte rpcSettingsId = 70;

        public Harmony Harmony { get; } = new Harmony(Id);

        public override void Load()
        {
            CustomGameOptionsData.customGameOptions = new CustomGameOptionsData();
            Harmony.PatchAll();
        }
    }
}
