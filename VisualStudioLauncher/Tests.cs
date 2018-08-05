using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.Setup.Configuration;

namespace VisualStudioLauncher
{
    public class Tests
    {
        public void when_getting_instances()
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

            Debug.Assert(File.Exists(path));

            var json = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            var loaded = SetupInstance.FromJson(json);

            var setup = (ISetupConfiguration2)new SetupConfigurationClass();
            var all = setup.EnumAllInstances();
            var count = 0;
            var instances = new ISetupInstance[1];
            do
            {
                all.Next(1, instances, out count);
                if (count  != 0)
                {
                    var instance = (ISetupInstance2)instances[0];
                    loaded.First(x => x.InstanceId == instance.GetInstanceId()).IsLaunchable = instance.IsLaunchable();
                }
            } while (count != 0);

        }
    }
}
