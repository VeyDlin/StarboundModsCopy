using Microsoft.Win32;
using SteamKit2;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace StarboundModsCopy {
    public class SteamApps {
        public readonly string SteamPath;

        public SteamApps() {
            var key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Valve\\Steam") ??
                      RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey("SOFTWARE\\Valve\\Steam");

            if(key != null && key.GetValue("SteamPath") is string steamPath) {
                SteamPath = steamPath;
            }

            if(SteamPath == default) {
                throw new DirectoryNotFoundException("Unable to find Steam on your computer.");
            }

            SteamPath = Path.GetFullPath(SteamPath);
        }


        private IEnumerable<string> GetLibraries() {
            var steamApps = SteamPath + "/steamapps";
            var libraryFoldersPath = steamApps + "/libraryfolders.vdf";
            var libraryFoldersKv = KeyValue.LoadAsText(libraryFoldersPath);
            var libraryFolders = new List<string> { steamApps };

            if(libraryFoldersKv != null) {
                libraryFolders.AddRange(libraryFoldersKv.Children
                    .Where(libraryFolder => int.TryParse(libraryFolder.Name, out _))
                    .Select(x => x.Value + "/steamapps")
                );
            }

            return libraryFolders;
        }


        public static string GetValue(KeyValue list, string key) {
            foreach(var i in list.Children) {
                if(i.Name == key) {
                    return i.Value;
                }
            }

            return null;
        }


        private KeyValue SearthGame(string gameName, string library) {
            string[] manifestList = Directory.GetFiles(library, "appmanifest_*.acf");

            foreach(var manifest in manifestList) {
                var info = KeyValue.LoadAsText(manifest);
                if(GetValue(info, "name") == gameName) {
                    return info;
                }
            }

            return null;
        }


        public KeyValue GetGameInfo(string gameName) {
            foreach(var library in GetLibraries()) {
                var game = SearthGame(gameName, library);
                if(game != null) {
                    return game;
                }
            }

            return null;
        }
    }
}
