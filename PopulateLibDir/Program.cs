using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using Microsoft.Win32;

namespace PopulateLibDir
{
    /*
     * Compiling the mods only works when referencing DLLs from the Timberborn game.
     * To avoid illegal distribution of these DLLs, I have written this application
     * which copies them from your Timberborn install directory into the appropriate
     * project lib directory. Only works on Windows.
     */

    class Program
    {
        static string GetTimberbornInstallDirectoryFromRegistry()
        {
            RegistryKey[] keys = {
                Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Uninstall"),
                Registry.LocalMachine.OpenSubKey(@"Software\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall")
            };
            foreach (var key in keys)
            {
                foreach (var subkey_name in key.GetSubKeyNames())
                {
                    var subkey = key.OpenSubKey(subkey_name);
                    if ((string)subkey.GetValue("DisplayName") == "Timberborn")
                    {
                        return (string)key.OpenSubKey(subkey_name).GetValue("InstallLocation");
                    }
                }
            }

            throw new Exception("Failed to find the Timberborn install directory!");
        }

        static string FindLibDestinationPath()
        {
            // We know that we're inside the PopulateLibDir tree somewhere. Roll up the tree till we get to the parent.
            var cwd = Directory.GetCurrentDirectory();
            var solution_dir = cwd.Substring(0, cwd.LastIndexOf("PopulateLibDir"));
            return Path.Combine(solution_dir, "Libs");
        }

        static void ClearOldLibs(string libs_dir)
        {
            // First run
            if (!Directory.Exists(libs_dir))
            {
                Directory.CreateDirectory(libs_dir);
                return;
            }

            // Subsequent runs
            new DirectoryInfo(libs_dir).Delete(true);
            Directory.CreateDirectory(libs_dir);
            return;
        }

        static void CopyLibs(string timberborn_dlls_dir, string libs_dir)
        {
            string[] file_names = {
                "Timberborn.CameraSystem.dll",
                "Timberborn.Common.dll",
                "UnityEngine.dll",
                "UnityEngine.CoreModule.dll",
            };
            foreach (var file_name in file_names)
            {
                var file = new FileInfo(Path.Combine(timberborn_dlls_dir, file_name));
                file.CopyTo(Path.Combine(libs_dir, file_name), true);
            }
        }

        static void DownloadBepInExDlls(string libs_dir)
        {
            var bepinex_zip_path = Path.Combine(libs_dir, "bepinexpack.zip");
            using (var client = new WebClient())
            {
                client.DownloadFile(
                    "https://timberborn.thunderstore.io/package/download/BepInEx/BepInExPack_Timberborn/5.4.15/",
                    bepinex_zip_path);
            }

            string[] zip_paths = {
                "BepInExPack_Timberborn/BepInEx/core/0Harmony.dll",
                "BepInExPack_Timberborn/BepInEx/core/BepInEx.dll",
            };
            using (ZipArchive archive = ZipFile.Open(bepinex_zip_path, ZipArchiveMode.Read))
            {
                foreach (var zip_path in zip_paths)
                {
                    var entry = archive.GetEntry(zip_path).Open();
                    using (var file = File.Create(Path.Combine(libs_dir, Path.GetFileName(zip_path))))
                    {
                        entry.CopyTo(file);
                    }
                }
            }
        }

        static void Main()
        {
            var timberborn_dlls_dir = Path.Combine(GetTimberbornInstallDirectoryFromRegistry(), @"Timberborn_Data\Managed");
            Console.WriteLine($"Found timberborn DLLs at {timberborn_dlls_dir}");
            var libs_dir = FindLibDestinationPath();
            Console.WriteLine($"Found destination dir at {libs_dir}");
            ClearOldLibs(libs_dir);
            CopyLibs(timberborn_dlls_dir, libs_dir);
            Console.WriteLine("Timberborn DLLs copied");

            Console.WriteLine("Downloading BepInExPack 5.4.15");
            DownloadBepInExDlls(libs_dir);
            Console.WriteLine("BepInExPack DLLs Copied");
            Console.WriteLine("Libs dir population complete.");
        }
    }
}
