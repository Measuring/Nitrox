using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using NitroxModel.Platforms.Exceptions;
using NitroxModel.Platforms.Interfaces;
using NitroxModel.System;
using NitroxModel.System.Debug;
using NitroxModel.System.Windows;

namespace NitroxModel.Platforms
{
    public sealed class Steam : IGamePlatform
    {
        private static Steam instance;
        public static Steam Instance => instance ??= new Steam();

        public bool OwnsGame(string gameDirectory)
        {
            return File.Exists(Path.Combine(gameDirectory, "steam_api64.dll"));
        }

        public async Task<ProcessEx> StartPlatformAsync()
        {
            ProcessEx steam = ProcessEx.GetFirstProcess("steam", p => p.MainModuleDirectory != null && File.Exists(Path.Combine(p.MainModuleDirectory, "steamclient.dll")));
            if (steam != null)
            {
                return steam;
            }
            
            // Steam is not running, start it.
            string exe = GetExeFile();
            if (exe == null)
            {
                return null;
            }
            steam = new ProcessEx(Process.Start(new ProcessStartInfo
            {
                WorkingDirectory = Path.GetDirectoryName(exe) ?? Directory.GetCurrentDirectory(),
                FileName = exe,
                WindowStyle = ProcessWindowStyle.Minimized,
                Arguments = "-silent" // Don't show Steam window
            }));

            // Wait for Steam to get ready.
            await RegistryEx.CompareAsync<int>(@"SOFTWARE\Valve\Steam\ActiveProcess",
                                               "pid",
                                               v => v == steam.Id,
                                               TimeSpan.FromSeconds(45));
            return steam;
        }

        public string GetExeFile()
        {
            string steamPath = RegistryEx.Read<string>(@"Software\Valve\Steam", "SteamPath");
            string exe = Path.Combine(steamPath, "steam.exe");
            return File.Exists(exe) ? Path.GetFullPath(exe) : null;
        }

        public async Task<ProcessDebugger> StartGameAsync(string pathToGameExe, int steamAppId, bool disposeProcess = true)
        {
            try
            {
                using ProcessEx steam = await StartPlatformAsync();
                if (steam == null)
                {
                    throw new PlatformException(GamePlatform.STEAM, "Steam is not running and could not be found.");
                }
            }
            catch (OperationCanceledException ex)
            {
                throw new PlatformException(GamePlatform.STEAM, "Timeout reached while waiting for platform to start. Try again once platform has finished loading.", ex);
            }

            return ProcessEx.StartWithDebugger(pathToGameExe,
                                               new[]
                                               {
                                                   ("SteamGameId", steamAppId.ToString()), 
                                                   ("SteamAppID", steamAppId.ToString()),
                                                   ("NITROX_LAUNCHER_PATH", Path.GetDirectoryName(Assembly.GetEntryAssembly().Location)),
                                               },
                                               Path.GetDirectoryName(pathToGameExe),
                                               disposeProcess: disposeProcess
            );
        }
    }
}
