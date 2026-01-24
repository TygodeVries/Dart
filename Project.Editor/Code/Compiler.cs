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
                Build(() =>
                {
                    Runtime.Logging.Debug.Log("Auto recompiled code.");
                });
        }

        static Job? waitJob;

        public static void Build(Action onComplete)
        {

            waitJob?.Done();
            // Run dotnet build on the script dir
            Generate();

            Job compile = new Job("Compiling code...");
            Runtime.Logging.Debug.Log("Compiling code...");
            Thread thr = new Thread(() =>
            {
                try
                {
                    Process prc = Process.Start("CMD.exe", $"/C dotnet build \"{EditorUtils.projectPath}/scripts/Game.csproj\" --artifacts-path \"{EditorUtils.projectPath}/compile\"");
                    File.Delete($"{EditorUtils.projectPath}/Game.dll");
                    prc.WaitForExit();
                    File.Copy($"{EditorUtils.projectPath}/compile/bin/Game/debug/Game.dll", $"{EditorUtils.projectPath}/Game.dll");
                    Directory.Delete($"{EditorUtils.projectPath}/compile", recursive: true);
                    compile.Done();

                    MainThread.Run(onComplete);
                }
                catch (Exception ex)
                {

                    compile.Done();
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
