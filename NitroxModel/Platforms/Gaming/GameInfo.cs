namespace NitroxModel.Platforms.Gaming
{
    public sealed class GameInfo
    {
        public static readonly GameInfo Subnautica = new()
        {
            Name = "Subnautica",
            FullName = "Subnautica",
            ExeName = "Subnautica.exe",
            SteamId = 264710
        };

        public static readonly GameInfo SubnauticaBelowZero = new()
        {
            Name = "SubnauticaZero",
            FullName = "Subnautica: Below Zero",
            ExeName = "SubnauticaZero.exe",
            SteamId = 848450
        };

        public string Name { get; private init; }
        public string FullName { get; private init; }
        public string ExeName { get; private init; }
        public int SteamId { get; private init; }

        private GameInfo()
        {
        }
    }
}
