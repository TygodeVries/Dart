using ImGuiNET;
using Project.Editor;
using Project.Editor.Code;
using Project.Editor.EditorModes;
using Project.Editor.UI.FileSystem.FileInspectors;
using Project.Editor.UI.Styles;
using Runtime;
using Runtime.Data;
using Runtime.Graphics;
using Runtime.Logging;

namespace Editor
{
    [Runtime.Plugins.DartEntryPoint(nameof(Main))]
    public class EntryPoint
    {
        static EntryPoint()
        {

        }

        public static void Main()
        {

            Style.Apply();
            Debug.Log("Loading Editor...");
            string[] args = Environment.GetCommandLineArgs();

            for (int cx = 0; cx < args.Length - 1; cx++)
            {
                if (args[cx] == "-p")
                {
                    Project.Editor.EditorUtils.projectPath = args[cx + 1];
                    Debug.Log($"Set project path {args[cx + 1]}");
                }
                if (args[cx] == "-e")
                {
                    Project.Editor.EditorUtils.exeLocation = args[cx + 1];
                    Debug.Log($"Set runtime location {args[cx + 1]}");
                }
            }

            EditorUtils.LoadAssetDatabase();

            ImGui.GetIO().ConfigFlags |= ImGuiConfigFlags.ViewportsEnable;
            ImGui.GetIO().ConfigFlags |= ImGuiConfigFlags.DockingEnable;

            Debug.Log("Overriding asset database to use open project instead.");

            RenderCanvas.main.SetTargetFPS(60);
            Game.SetAssetDatabase(new AssetDatabase(EditorUtils.projectPath));
            Asset settingAsset = Game.GetAssetDatabase().GetAsset("gamesettings.json");
            GameSettings? gameSettings = Files.Load<GameSettings>(settingAsset.GetSystemPath()); ;
            gameSettings.asset = settingAsset;

            Game.GetAssetDatabase().Start();
            AssetManager.Init();

            Compiler.Generate();
            Compiler.StartAutoCompile();
            UserCode.Load();
            EditorMode.Init();
            EditorMode.SetMode(Mode.Edit);
        }
    }
}
