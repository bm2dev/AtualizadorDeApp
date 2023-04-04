using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AtualizadorDeApp {
    class Program {
        static void Main(string[] args) {
            Console.WriteLine("Bem vindo ao atualizador de apps!");

            Console.WriteLine("Cole o caminho que está localizado a pasta com \"app.json\" base:");
            string appJsonPath = Console.ReadLine() ?? "";
            Console.WriteLine("Cole o caminho que está localizado a pasta de parceiros:");
            string foldersPath = Console.ReadLine() ?? "";

            try {
                AtualizaApps(Path.Combine(appJsonPath, "app.json"), foldersPath);
            } catch (Exception ex) {
                Console.WriteLine(ex);
            }

            Console.ReadKey();
        }

        public static void AtualizaApps(string appJsonBasePath, string foldersPath) {

            JObject appJsonBase = JObject.Parse(File.ReadAllText(appJsonBasePath));

            JObject expo = (JObject)appJsonBase.SelectToken("expo");
            JProperty extraProperty = expo.Property("extra");
            if (extraProperty != null) {
                extraProperty.Remove();
            }

            string[] folders = Directory.GetDirectories(foldersPath).Where(d => !d.EndsWith(".git")).ToArray();

            Console.WriteLine("Atualizando...");

            foreach (var folder in folders) {
                try {
                    string currentFilePath = Path.Combine(folder, "app.json");

                    if (!File.Exists(currentFilePath)) { continue; }

                    JObject currentAppJson = JObject.Parse(File.ReadAllText(currentFilePath));
                    if (currentAppJson == null) { continue; }

                    string name = (string)currentAppJson["expo"]["name"];
                    string slug = (string)currentAppJson["expo"]["slug"];
                    string backgroundColor = (string)currentAppJson["expo"]["splash"]["backgroundColor"];
                    string bundleIdentifier = (string)currentAppJson["expo"]["ios"]["bundleIdentifier"];
                    string package = (string)currentAppJson["expo"]["android"]["package"];

                    currentAppJson = appJsonBase;
                    currentAppJson["expo"]["name"] = name;
                    currentAppJson["expo"]["slug"] = slug;
                    currentAppJson["expo"]["splash"]["backgroundColor"] = backgroundColor;
                    currentAppJson["expo"]["android"]["adaptiveIcon"]["backgroundColor"] = backgroundColor;
                    currentAppJson["expo"]["ios"]["bundleIdentifier"] = bundleIdentifier;
                    currentAppJson["expo"]["android"]["package"] = package;

                    File.WriteAllText(currentFilePath, currentAppJson.ToString(Formatting.Indented));

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"OK | {Path.GetFileNameWithoutExtension(folder)}");
                    Console.ForegroundColor = ConsoleColor.White;
                } catch(Exception ex) {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("");
                    Console.WriteLine($"ERRO | {Path.GetFileNameWithoutExtension(folder)}");
                    Console.WriteLine(ex);
                    Console.WriteLine("");

                    Console.ForegroundColor= ConsoleColor.White;
                }
            }

            Console.WriteLine("Apps Atualizados");
        }
    }
}
