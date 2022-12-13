using System;
using System.IO;

namespace StarboundModsCopy {
    class Program {

        static void Main(string[] args) {
            try {
                var stemn = new SteamApps();
                Console.WriteLine("Find stemn");

                var starbound = stemn.GetGameInfo("Starbound");
                if(starbound.info == null) {
                    throw new DirectoryNotFoundException("Unable to find Starbound on your computer.");
                }
                var appid = SteamApps.GetValue(starbound.info, "appid");
                var installdir = SteamApps.GetValue(starbound.info, "installdir");
                var workshop = starbound.library + "/workshop/content/" + appid;
                var mods = starbound.library + "/common/" + installdir + "/mods";

                Console.WriteLine("Find Starbound");
                Console.WriteLine("=======================================");
                Console.WriteLine("Press any key to update mods");
                Console.ReadKey();

                while(true) {
                    Console.WriteLine("=======================================");


                    // Clear mods
                    Console.WriteLine("Clear old mods:");
                    var modsDir = new DirectoryInfo(mods);
                    foreach(var file in modsDir.GetFiles()) {
                        Console.WriteLine("     Delete " + installdir + "/mods/" + file.Name);
                        file.Delete();
                    }
                    Console.WriteLine();

                    // Copy mods
                    Console.WriteLine("Copy workshop mods:");
                    var dd = Directory.GetDirectories(workshop);
                    foreach(var dir in Directory.GetDirectories(workshop)) {
                        var contents = dir + "/contents.pak";
                        var modName = Path.GetFileName(Path.GetDirectoryName(contents));
                        Console.WriteLine("     Copy workshop/" + modName + "/contents.pak");
                        File.Copy(contents, mods + "/" + modName + ".pak");
                    }
                    Console.WriteLine();

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Successfully!");
                    Console.ResetColor();

                    Console.WriteLine();
                    Console.WriteLine("Press any key to update mods again");

                    Console.ReadKey();
                }

            } catch(Exception ex) {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\nMessage ---\n{0}", ex.Message);
                Console.WriteLine("\nHelpLink ---\n{0}", ex.HelpLink);
                Console.WriteLine("\nSource ---\n{0}", ex.Source);
                Console.WriteLine("\nStackTrace ---\n{0}", ex.StackTrace);
                Console.WriteLine("\nTargetSite ---\n{0}", ex.TargetSite);
                Console.ResetColor();
            }

            Console.ReadKey();
        }


    }
}
