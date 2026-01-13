using Runtime;
using Runtime.Data;
using Runtime.Plugins;
using System.Text.Json;

namespace Project.Editor.Code
{
    public class Compiler
    {
        public static void Build()
        {
            // Run dotnet build on the script dir
            Generate();

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

            file += footer;

            Asset asset = Game.GetAssetDatabase().GetAsset("scripts/Game.csproj");
            File.WriteAllText(asset.GetSystemPath(), file);
        }
    }
}
