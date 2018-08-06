using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using NuGet;
using Squirrel;

namespace VisualStudioLauncher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            var version = new SemanticVersion(Assembly.GetExecutingAssembly().GetName().Version);
            // Switch to AssemblyInformationalVersionAttribute when Squirrel supports SemVer2

            Title += " - " + version;
            VersionTextBlock.Text = version.ToString();

#if RELEASE
            var updateFrom = "https://devdiv.blob.core.windows.net/vsl";
#else
            var updateFrom = Assembly.GetExecutingAssembly().GetCustomAttributes<AssemblyMetadataAttribute>()
                .Where(x => x.Key == "ReleasesPath")
                .Select(x => x.Value)
                .First();
#endif

            Task.Delay(200).ContinueWith(async x =>
            {
                using (var updater = new UpdateManager(updateFrom))
                {
                    var result = await updater.UpdateApp(i => Dispatcher.Invoke(() => status.Text = $"Updating {i}...", DispatcherPriority.Background));
                    if (result == null)
                        return;

                    if (result.Version > version)
                    {
                        Dispatcher.Invoke(() => status.Text = $"Restart to update to version {result.Version}.");
                    }
                    else if (result.Version <= version)
                    {
                        Dispatcher.Invoke(() => status.Text = $"Running latest version.");
                    }
                }
            });
        }
    }
}
