using System.IO;
using NitroxModel.Platforms.Interfaces;

namespace NitroxModel.Platforms
{
    public static class GamePlatforms
    {
        public static readonly IGamePlatform[] AllPlatforms = { Steam.Instance };

        public static IGamePlatform GetPlatformByGameDir(string gameDirectory)
        {
            if (!Directory.Exists(gameDirectory))
            {
                return null;
            }

            foreach (IGamePlatform platform in AllPlatforms)
            {
                if (platform.OwnsGame(gameDirectory))
                {
                    return platform;
                }
            }
            return null;
        }
    }
}
