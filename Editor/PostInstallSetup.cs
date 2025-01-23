using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;

namespace VisualPinball.Engine.Mpf.Unity.Editor
{
    public class PostInstallSetup
    {
        private const string PackageName = "org.visualpinball.engine.missionpinball";

        [InitializeOnLoadMethod]
        private static void OnLoad()
        {
            Events.registeredPackages += RegisteredPackagesEventHandler;
        }

        private static void RegisteredPackagesEventHandler(PackageRegistrationEventArgs args)
        {
            var packageInfo = args.added.FirstOrDefault(package => package.name == PackageName);
            if (packageInfo != default)
                Setup(packageInfo.resolvedPath);
        }

        private static void Setup(string path)
        {
            // Copy sample machine folder to streaming assets
            var sourcePath = Path.Combine(path, "SampleMachineFolder");
            var destPath = Path.Combine(Application.streamingAssetsPath, "MpfMachineFolder");
            CopyDirectory(sourcePath, destPath, recursive: true);
        }

        // Source: https://learn.microsoft.com/en-us/dotnet/standard/io/how-to-copy-directories
        private static void CopyDirectory(string sourceDir, string destinationDir, bool recursive)
        {
            var dir = new DirectoryInfo(sourceDir);

            if (!dir.Exists)
                throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");

            DirectoryInfo[] dirs = dir.GetDirectories();
            Directory.CreateDirectory(destinationDir);

            foreach (FileInfo file in dir.GetFiles())
            {
                string targetFilePath = Path.Combine(destinationDir, file.Name);
                file.CopyTo(targetFilePath, overwrite: false);
            }

            if (recursive)
            {
                foreach (DirectoryInfo subDir in dirs)
                {
                    string newDestinationDir = Path.Combine(destinationDir, subDir.Name);
                    CopyDirectory(subDir.FullName, newDestinationDir, true);
                }
            }
        }
    }
}
