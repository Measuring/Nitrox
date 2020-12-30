using System.Collections.Generic;
using System.IO;
using NitroxModel.Platforms;
using NitroxModel.Platforms.Gaming;

namespace NitroxModel.Discovery.InstallationFinders
{
    public class GameInCurrentDirectoryFinder : IFindGameInstallation
    {
        public string FindGame(List<string> errors = null)
        {
            string currentDirectory = Directory.GetCurrentDirectory();
            if (File.Exists(Path.Combine(currentDirectory, GameInfo.Subnautica.ExeName)))
            {
                return currentDirectory;
            }

            return null;
        }
    }
}
