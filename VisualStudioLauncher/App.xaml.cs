using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;
using Squirrel;

namespace VisualStudioLauncher
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

#if RELEASE
            Task.Delay(1000).ContinueWith(async x => 
            {
                using (var updater = new UpdateManager("https://devdiv.blob.core.windows.net/vsl"))
                {
                    await updater.UpdateApp();
                }
            });
#else
            var releasesPath = Assembly.GetExecutingAssembly().GetCustomAttributes<AssemblyMetadataAttribute>()
                .Where(x => x.Key == "ReleasesPath")
                .Select(x => x.Value)
                .First();

            Task.Delay(1000).ContinueWith(async x => 
            {
                using (var updater = new UpdateManager(releasesPath))
                {
                    await updater.UpdateApp();
                }
            });
#endif
        }
    }
}
