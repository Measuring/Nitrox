using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using NitroxModel.Platforms;
using NitroxModel.Platforms.Gaming;
using NitroxModel.Platforms.OS.Windows.Structs;

namespace NitroxModel.Discovery.InstallationFinders
{
    public class SteamGameRegistryFinder : IFindGameInstallation
    {
        public string FindGame(List<string> errors = null)
        {
            string steamPath = RegistryEx.Read<string>("Software\\Valve\\Steam", "SteamPath");
            if (string.IsNullOrEmpty(steamPath))
            {
                errors?.Add("It appears you don't have Steam installed.");
                return null;
            }

            string appsPath = Path.Combine(steamPath, "steamapps");
            if (File.Exists(Path.Combine(appsPath, $"appmanifest_{GameInfo.Subnautica.SteamId}.acf")))
            {
                return Path.Combine(appsPath, "common", GameInfo.Subnautica.Name);
            }

            string path = SearchAllInstallations(Path.Combine(appsPath, "libraryfolders.vdf"), GameInfo.Subnautica.SteamId, GameInfo.Subnautica.Name);
            if (string.IsNullOrEmpty(path))
            {
                errors?.Add($"It appears you don't have {GameInfo.Subnautica.Name} installed anywhere. The game files are needed to run the server.");
            }
            else
            {
                return path;
            }

            return null;
        }

        /// <summary>
        ///     Finds game install directory by iterating through all the steam game libraries configured and finding the appid
        ///     that matches <see cref="GameInfo.Subnautica" />.
        /// </summary>
        /// <param name="libraryfolders"></param>
        /// <param name="appid"></param>
        /// <param name="gameName"></param>
        /// <returns></returns>
        private static string SearchAllInstallations(string libraryfolders, int appid, string gameName)
        {
            if (!File.Exists(libraryfolders))
            {
                return null;
            }

            using StreamReader file = new StreamReader(libraryfolders);
            string line;
            while ((line = file.ReadLine()) != null)
            {
                line = Regex.Unescape(line.Trim().Trim('\t'));
                Match regMatch = Regex.Match(line, @"""(.*)""\t*""(.*)""");
                string key = regMatch.Groups[1].Value;
                string value = regMatch.Groups[2].Value;
                if (!int.TryParse(key, out int _))
                {
                    continue;
                }
                if (!File.Exists(Path.Combine(value, $"steamapps/appmanifest_{appid}.acf")))
                {
                    continue;
                }

                return Path.Combine(value, "steamapps/common", gameName);
            }

            return null;
        }
    }
}
