using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Threading;
using Microsoft.VisualStudio.Setup.Configuration;

namespace VisualStudioLauncher
{
    public class MainViewModel
    {
        Dispatcher dispatcher;

        public MainViewModel()
        {
            dispatcher = Dispatcher.CurrentDispatcher;

            Launch = new RelayCommand<SetupInstance>(OnLaunch, OnCanLaunch);

            Task.Run(Initialize);
        }

        public RelayCommand<SetupInstance> Launch { get; }

        public ObservableCollection<SetupInstance> Instances { get; set; } = new ObservableCollection<SetupInstance>();

        private bool OnCanLaunch(SetupInstance instance) => instance?.IsLaunchable == true;

        private void OnLaunch(SetupInstance instance) => Process.Start(instance.ProductPath);

        private void Initialize()
        {
            var path = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86),
                @"Microsoft Visual Studio\Installer\vswhere.exe");

            // -all -prerelease -format json
            var process = Process.Start(new ProcessStartInfo(path, "-all -prerelease -format json")
            {
                CreateNoWindow = true,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
            });

            var json = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            var instances = SetupInstance.FromJson(json);

            // We may need to use the API to get additional data, 
            // in this to get IsLaunchable, although that may be 
            // inferred some other way?
            var setup = (ISetupConfiguration2)new SetupConfigurationClass();
            var all = setup.EnumAllInstances();
            var count = 0;
            var enumerated = new ISetupInstance[1];
            do
            {
                all.Next(1, enumerated, out count);
                if (count != 0)
                {
                    var instance = (ISetupInstance2)enumerated[0];
                    var model = instances.First(x => x.InstanceId == instance.GetInstanceId());
                    model.IsLaunchable = instance.IsLaunchable();

                    dispatcher.Invoke(() => Instances.Add(model));
                }
            } while (count != 0);
        }
    }
}
