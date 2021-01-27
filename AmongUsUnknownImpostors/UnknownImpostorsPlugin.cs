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

        public Harmony Harmony { get; } = new Harmony(Id);

        public static ConfigEntry<bool> unkImpostor { get; private set; }

        public override void Load()
        {
            unkImpostor = Config.Bind("AmongUsUnknownImpostors", "unkImpostor", true);

            Harmony.PatchAll();
        }
    }
}
