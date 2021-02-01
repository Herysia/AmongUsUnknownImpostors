using System;
using System.IO;
using System.Linq;
using Hazel;
using LobbyOptionsAPI;

namespace AmongUsUnknownImpostors.Patches
{
    public class CustomGameOptionsData : LobbyOptions
    {
        private byte settingsVersion = 1;
        public static CustomGameOptionsData customGameOptions;

        public CustomGameOptionsData() : base(UnknownImpostorsPlugin.Id, UnknownImpostorsPlugin.rpcSettingsId)
        {
            unkImpostor = AddOption(false, "Unk impostor");
            impoVision = AddOption(2.0f, "Impostor light off vision", 0.25f, 5.0f, 0.25f, "x");
        }

        public CustomToggleOption unkImpostor;
        public CustomNumberOption impoVision;

        public override void SetRecommendations()
        {
            unkImpostor.value = false;
            impoVision.value = 2.0f;
        }

        public override void Serialize(BinaryWriter writer)
        {
            writer.Write(this.settingsVersion);
            writer.Write(unkImpostor.value);
            writer.Write(impoVision.value);
        }

        public override void Deserialize(BinaryReader reader)
        {
            try
            {
                SetRecommendations();
                byte b = reader.ReadByte();
                unkImpostor.value = reader.ReadBoolean();
                impoVision.value = reader.ReadSingle();
            }
            catch
            {
            }
        }

        public override void Deserialize(MessageReader reader)
        {
            try
            {
                SetRecommendations();
                byte b = reader.ReadByte();
                unkImpostor.value = reader.ReadBoolean();
                impoVision.value = reader.ReadSingle();
            }
            catch
            {
            }
        }

        public override string ToHudString()
        {
            settings.Length = 0;

            try
            {
                settings.AppendLine();
                settings.AppendLine($"Unk impostor: {unkImpostor.value}");
                if (unkImpostor.value)
                {
                    settings.Append("Impostor light off vision: ");
                    settings.Append(impoVision.value);
                    settings.Append("x");
                    settings.AppendLine();
                }
            }
            catch
            {
            }

            return settings.ToString();
        }
    }
}