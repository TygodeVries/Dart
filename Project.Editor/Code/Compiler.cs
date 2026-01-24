using Project.Editor.UI;
using Runtime;
using Runtime.Calc;
using Runtime.Data;
using Runtime.Plugins;
using System.Diagnostics;
using System.Text.Json;

namespace Project.Editor.Code
{
    public class Compiler
    {
        public static void StartAutoCompile()
        {
            Game.GetAssetDatabase().DatabaseRefreshed += Compiler_DatabaseRefreshed;
        }

        private static void Compiler_DatabaseRefreshed()
        {

            bool scriptChange = false;
            List<string> changes = Game.GetAssetDatabase().changes;
            foreach (string file in changes)
            {
                Asset asset = Asset.FromSystemPath(Game.GetAssetDatabase(), file);

                string change = asset.GetPath(); // #TODO make this better;
                if (change.StartsWith("scripts") && !change.Contains(".vs") && !change.Contains("Game.csproj"))
                {
                    Runtime.Logging.Debug.Log(asset.GetPath());

                    scriptChange = true;
                }
            }

            if (scriptChange)

                ScheduleBuild(() =>
                {
                    Runtime.Logging.Debug.Log("Auto recompiled code.");
                });
        }

        public static void ScheduleBuild(Action onComplete)
        {
            MainThread.Run(() =>
            {
                Build(onComplete);
            });
        }

        private static void Build(Action onComplete)
        {
            // Run dotnet build on the script dir
            Generate();
            Thread thr = new Thread(() =>
            {
                try
                {
                    Job compile = new Job("Compiling code...");
                    // Start working on the build
                    Process prc = Process.Start("CMD.exe", $"/C dotnet build \"{EditorUtils.projectPath}/scripts/Game.csproj\" --artifacts-path \"{EditorUtils.projectPath}/compile\"");

                    // Wait until build is finished
                    prc.WaitForExit();
                    compile.Done();

                    Job unload = new Job("Unloading assets...");
                    // Try to unload
                    UserCode.Unload(() =>
                    {
                        // Delete old file
                        File.Delete($"{EditorUtils.projectPath}/Game.dll");

                        // Put in new file
                        File.Copy($"{EditorUtils.projectPath}/compile/bin/Game/debug/Game.dll", $"{EditorUtils.projectPath}/Game.dll");

                        // Clean up
                        Directory.Delete($"{EditorUtils.projectPath}/compile", recursive: true);
                        unload.Done();

                        Job loading = new Job("Loading...");
                        // Load in new code
                        UserCode.Load();
                        MainThread.Run(onComplete);
                        loading.Done();
                    });
                }
                catch (Exception ex)
                {
                    Runtime.Logging.Debug.Error(ex.ToString());
                }
            });

            thr.Start();
        }

        public static void Generate()
        {
            string header = @"<Project Sdk=""Microsoft.NET.Sdk"">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <OutputType>Library</OutputType>
  </PropertyGroup>

  <ItemGroup>
    <Compile Include=""*.cs"" />
  </ItemGroup>

  <ItemGroup>";

            string footer = @"  </ItemGroup>
</Project>
";

            GameSettings gameSettings = GameSettings.GetGameSettings();
            string[] plugins = gameSettings.Plugins;

            string file = header;
            foreach (string plugin in plugins)
            {
                string path = Game.GetAssetDatabase().GetAsset($"plugins/{plugin}/info.plugin.json").GetSystemPath();
                PluginData pluginData = JsonSerializer.Deserialize<PluginData>(File.ReadAllText(path));
                string pluginSource = $"\n\t<Reference Include=\"{plugin}\">\r\n      <HintPath>../plugins/{plugin}/{pluginData.CoreDll}</HintPath>\r\n      <Private>false</Private>\r\n    </Reference>";
                file += pluginSource;
            }

            file += $"\n\t<Reference Include=\"Runtime\">\r\n      <HintPath>{EditorUtils.exeLocation.Replace(".exe", ".dll")}</HintPath>\r\n     <Private>false</Private>\r\n      </Reference>";

            file += footer;

            Asset asset = Game.GetAssetDatabase().GetAsset("scripts/Game.csproj");
            File.WriteAllText(asset.GetSystemPath(), file);
        }
    }
}
