using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using NitroxLauncher.Events;
using NitroxLauncher.Pages;
using NitroxLauncher.Patching;
using NitroxModel;
using NitroxModel.Helper;
using NitroxModel.Logger;
using NitroxModel.Platforms;
using NitroxModel.Platforms.Gaming;
using NitroxModel.Platforms.OS.Windows;
using NitroxModel.Platforms.OS.Windows.Debug;

namespace NitroxLauncher
{
    public class LauncherLogic : IDisposable, INotifyPropertyChanged
    {
        public const string RELEASE_PHASE = "ALPHA";
        private ProcessEx gameProcess;
        private bool isEmbedded;
        private NitroxEntryPatch nitroxEntryPatch;
        private Process serverProcess;
        private string subnauticaPath;
        public static string Version => Assembly.GetAssembly(typeof(Extensions)).GetName().Version.ToString();
        public static LauncherLogic Instance { get; private set; }

        public string SubnauticaPath
        {
            get => subnauticaPath;
            private set
            {
                value = Path.GetFullPath(value); // Ensures the path looks alright (no mixed / and \ path separators)
                subnauticaPath = value;
                OnPropertyChanged();
            }
        }

        public bool ServerRunning => !serverProcess?.HasExited ?? false;

        public LauncherLogic()
        {
            Instance = this;
        }

        public event EventHandler<ServerStartEventArgs> ServerStarted;
        public event DataReceivedEventHandler ServerDataReceived;
        public event EventHandler ServerExited;

        public void Dispose()
        {
            gameProcess?.Dispose();
            serverProcess?.Dispose();
            serverProcess = null; // Indicate the process is dead now.
        }

        public async Task WriteToServerAsync(string inputText)
        {
            if (ServerRunning)
            {
                try
                {
                    await serverProcess.StandardInput.WriteLineAsync(inputText);
                }
                catch (Exception)
                {
                    // Ignore errors while writing to process
                }
            }
        }

        [Conditional("RELEASE")]
        public async void CheckNitroxVersion()
        {
            await Task.Factory.StartNew(() =>
                                        {
                                            Version latestVersion = WebHelper.GetNitroxLatestVersion();
                                            Version currentVersion = new Version(Version);

                                            if (latestVersion > currentVersion)
                                            {
                                                MessageBox.Show($"A new version of the mod ({latestVersion}) is available !\n\nPlease check our website to download it",
                                                                "New version available",
                                                                MessageBoxButton.OK,
                                                                MessageBoxImage.Question,
                                                                MessageBoxResult.OK,
                                                                MessageBoxOptions.DefaultDesktopOnly);
                                            }
                                        },
                                        CancellationToken.None,
                                        TaskCreationOptions.None,
                                        TaskScheduler.FromCurrentSynchronizationContext());
        }

        public async Task<string> SetTargetedSubnauticaPath(string path)
        {
            if (SubnauticaPath == path || !Directory.Exists(path))
            {
                return null;
            }
            SubnauticaPath = path;

            return await Task.Factory.StartNew(() =>
                                               {
                                                   PirateDetection.TriggerOnDirectory(path);
                                                   File.WriteAllText("path.txt", path);
                                                   if (nitroxEntryPatch?.IsApplied == true)
                                                   {
                                                       nitroxEntryPatch.Remove();
                                                   }
                                                   nitroxEntryPatch = new NitroxEntryPatch(path);

                                                   if (Path.GetFullPath(path).StartsWith(AppHelper.ProgramFileDirectory, StringComparison.OrdinalIgnoreCase))
                                                   {
                                                       AppHelper.RestartAsAdmin();
                                                   }

                                                   return path;
                                               },
                                               CancellationToken.None,
                                               TaskCreationOptions.None,
                                               TaskScheduler.FromCurrentSynchronizationContext());
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NavigateTo(Type page)
        {
            if (page == null || !page.IsSubclassOf(typeof(Page)) && page != typeof(Page))
            {
                return;
            }
            if (ServerRunning && isEmbedded && page == typeof(ServerPage))
            {
                page = typeof(ServerConsolePage);
            }

            if (Application.Current.MainWindow != null)
            {
                ((MainWindow)Application.Current.MainWindow).FrameContent = Application.Current.FindResource(page.Name);
            }
        }

        public void NavigateTo<TPage>() where TPage : Page
        {
            NavigateTo(typeof(TPage));
        }

        public bool NavigationIsOn<TPage>() where TPage : Page
        {
            MainWindow window = Application.Current.MainWindow as MainWindow;
            if (window == null)
            {
                return false;
            }
            return window.FrameContent?.GetType() == typeof(TPage);
        }

        public bool IsSubnauticaDirectory(string directory)
        {
            if (string.IsNullOrWhiteSpace(directory))
            {
                return false;
            }

            return Directory.EnumerateFileSystemEntries(directory, "*.exe")
                            .Any(file => Path.GetFileName(file)?.Equals(GameInfo.Subnautica.ExeName, StringComparison.OrdinalIgnoreCase) ?? false);
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        internal async Task StartSingleplayerAsync()
        {
#if RELEASE
            if (Process.GetProcessesByName(GameInfo.Subnautica.Name).Length > 0)
            {
                throw new Exception($"An instance of {GameInfo.Subnautica.Name} is already running");
            }
#endif
            gameProcess = await StartSubnauticaAsync();
        }

        internal async Task StartMultiplayerAsync()
        {
#if RELEASE
            if (Process.GetProcessesByName(GameInfo.Subnautica.Name).Length > 0)
            {
                throw new Exception($"An instance of {GameInfo.Subnautica.Name} is already running");
            }
#endif
            // Store path where launcher is in AppData for Nitrox bootstrapper to read
            // string nitroxAppData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Nitrox");
            // Directory.CreateDirectory(nitroxAppData);
            // File.WriteAllText(Path.Combine(nitroxAppData, "launcherpath.txt"), Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
            //
            // // TODO: The launcher should override FileRead win32 API for the Subnautica process to give it the modified Assembly-CSharp from memory 
            // string bootloaderName = "Nitrox.Bootloader.dll";
            // try
            // {
            //     File.Copy(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "lib", bootloaderName), Path.Combine(subnauticaPath, "Subnautica_Data", "Managed", bootloaderName), true);
            // }
            // catch (IOException ex)
            // {
            //     Log.Error(ex, "Unable to move bootloader dll to Managed folder. Still attempting to launch because it might exist from previous runs.");
            // }
            //
            // nitroxEntryPatch.Remove(); // Remove any previous instances first.
            // nitroxEntryPatch.Apply();

            gameProcess = await StartSubnauticaAsync();
        }

        internal Process StartServer(bool standalone)
        {
            if (ServerRunning)
            {
                throw new Exception("An instance of Nitrox Server is already running");
            }

            string launcherDir = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            string serverPath = Path.Combine(launcherDir, "NitroxServer-Subnautica.exe");
            ProcessStartInfo startInfo = new ProcessStartInfo(serverPath);
            startInfo.WorkingDirectory = launcherDir;

            if (!standalone)
            {
                startInfo.UseShellExecute = false;
                startInfo.RedirectStandardOutput = true;
                startInfo.RedirectStandardInput = true;
                startInfo.CreateNoWindow = true;
            }

            serverProcess = Process.Start(startInfo);
            if (serverProcess != null)
            {
                serverProcess.EnableRaisingEvents = true; // Required for 'Exited' event from process.

                if (!standalone)
                {
                    serverProcess.OutputDataReceived += ServerProcessOnOutputDataReceived;
                    serverProcess.BeginOutputReadLine();
                }
                serverProcess.Exited += (sender, args) => OnEndServer();
                OnStartServer(!standalone);
            }
            return serverProcess;
        }

        internal async Task SendServerCommandAsync(string inputText)
        {
            if (!ServerRunning)
            {
                return;
            }

            await WriteToServerAsync(inputText);
        }

        private void OnEndServer()
        {
            ServerExited?.Invoke(serverProcess, EventArgs.Empty);
            isEmbedded = false;
        }

        private void ServerProcessOnOutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            ServerDataReceived?.Invoke(sender, e);
        }

        private async Task<ProcessEx> StartSubnauticaAsync()
        {
            // Start game & gaming platform if needed.
            string subnauticaExe = Path.Combine(subnauticaPath, GameInfo.Subnautica.ExeName);
            using ProcessDebugger debug = GamePlatforms.GetPlatformByGameDir(subnauticaPath) switch
            {
                Steam s => await s.StartGameAsync(subnauticaExe, GameInfo.Subnautica.SteamId, false),
                _ => throw new Exception($"Directory '{subnauticaPath}' is not a valid {GameInfo.Subnautica.Name} game installation or the game's platform is unsupported.")
            };
            
            // TODO: Set breakpoint on CreateFileW to change register values when Assembly-CSharp.dll is accessed.
            debug.EntrypointBreak += (sender, args) =>
            {
                ProcessEx proc = sender.Process;
                IntPtr createFileW = proc.GetAddress("kernel32.dll", "CreateFileW");
                Log.Debug($"kernel32.dll.CreateFileW: 0x{createFileW.ToString("X")}");
                
                sender.AddBreakpoint(createFileW);
            };
            debug.BreakpointHit += (sender, context) =>
            {
                string file = sender.Process.ReadString(context.ThreadContext.Rcx, Encoding.Unicode);
                Log.Debug($"CreateFileW: {file}");
                if (file.IndexOf("Assembly-CSharp.dll", StringComparison.InvariantCultureIgnoreCase) > -1)
                {
                    context.DebuggerOptions.StopDebugging();
                }
            };
            await debug.StartAsync();
            await debug.WaitForDebuggerExit();
            return null;
            
            // if (PlatformDetection.IsEpic(subnauticaPath))
            // {
            //     startInfo.Arguments = "-EpicPortal -vrmode none";
            // }
            // else if (PlatformDetection.IsSteam(subnauticaPath))
            // {
            //     return await Steam.Instance.StartGameAsync(subnauticaExe, SteamGameRegistryFinder.SUBNAUTICA_APP_ID);
            // }
            // else if (PlatformDetection.IsMicrosoftStore(subnauticaPath))
            // {
            //     startInfo.FileName = "ms-xbl-38616e6e:\\";
            // }
            //
            // Process proc = Process.Start(startInfo);
            // while (proc != null && string.IsNullOrEmpty(proc.MainWindowTitle))
            // {
            //     await Task.Delay(100);
            //     proc.Refresh();
            // }
            // return proc;
        }

        private void OnStartServer(bool embedded)
        {
            isEmbedded = embedded;
            ServerStarted?.Invoke(serverProcess, new ServerStartEventArgs(embedded));
        }

        private void OnSubnauticaExited(object sender, EventArgs e)
        {
            try
            {
                nitroxEntryPatch.Remove();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            Log.Info("Finished removing patches!");
        }
    }
}
