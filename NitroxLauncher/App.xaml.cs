using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using NitroxModel.Logger;
using NitroxModel.Platforms.Gaming.Exceptions;

namespace NitroxLauncher
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            Log.Setup();
            
            // Set default style for all windows to the style with the target type 'Window' (in App.xaml).
            FrameworkElement.StyleProperty.OverrideMetadata(typeof(Window),
                new FrameworkPropertyMetadata
                {
                    DefaultValue = FindResource(typeof(Window))
                });
            FrameworkElement.StyleProperty.OverrideMetadata(typeof(Page),
                new FrameworkPropertyMetadata
                {
                    DefaultValue = FindResource(typeof(Page))
                });

            // Error if running from a temporary directory because Nitrox Launcher won't be able to write files directly to zip/rar
            // Tools like WinRAR do this to support running EXE files while it's still zipped.
            if (Directory.GetCurrentDirectory().StartsWith(Path.GetTempPath(), StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show("Nitrox launcher should not be executed from a temporary directory. Install Nitrox launcher properly by extracting ALL files and moving these to a dedicated location on your PC.",
                    "Invalid working directory",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                Environment.Exit(1);
            }

            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            MainWindow window = (MainWindow)Current.MainWindow;
            window?.CloseInternalServerAndRemovePatchAsync();

            base.OnExit(e);
        }

        private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            // Do not crash the app on certain exceptions.
            e.Handled = e.Exception switch
            {
                PlatformException _ => true,
                OperationCanceledException _ => true,
                _ => false
            };

            // If something went seriously wrong. Close embedded server too.
            if (!e.Handled)
            {
                MainWindow window = (MainWindow)Current.MainWindow;
                window?.CloseInternalServerAndRemovePatchAsync();
            }
            
            Log.Error(e.Exception.ToString());
            MessageBox.Show(GetExceptionMessageByEnvironment(e.Exception), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private string GetExceptionMessageByEnvironment(Exception e)
        {
#if RELEASE
            return e.Message;
#else
            return e.ToString();
#endif
        }
    }
}
