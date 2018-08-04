using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
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

            Title += " - " + Assembly.GetExecutingAssembly().GetName().Version;

#if RELEASE
            var updateFrom = "https://devdiv.blob.core.windows.net/vsl";
#else
            var updateFrom = Assembly.GetExecutingAssembly().GetCustomAttributes<AssemblyMetadataAttribute>()
                .Where(x => x.Key == "ReleasesPath")
                .Select(x => x.Value)
                .First();
#endif

            Task.Delay(1000).ContinueWith(async x =>
            {
                using (var updater = new UpdateManager(updateFrom))
                {
                    await updater.UpdateApp(i => Dispatcher.Invoke(() => $"Updating {i}...", DispatcherPriority.Background));
                }
            });
        }
    }
}
