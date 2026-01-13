using Project.Editor;
using Project.Editor.Code;
using Project.Editor.UI;
using Project.Editor.UI.Assets;
using Project.Editor.UI.FileSystem;
using Project.Editor.UI.Inspectors;
using Project.Editor.UI.Styles;
using Runtime;
using Runtime.Data;
using Runtime.DearImGUI.Gui;
using Runtime.Graphics;
using Runtime.Logging;

namespace Editor
{
    [Runtime.Plugins.DartEntryPoint("Main")]
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
                }
                if (args[cx] == "-e")
                {
                    Project.Editor.EditorUtils.exeLocation = args[cx + 1];
                }
            }

            EditorUtils.LoadAssetDatabase();

            GuiWindow.Enable(new Headerbar());
            GuiWindow.Enable(new ProjectWindow());
            GuiWindow.Enable(new InspectorWindow());
            GuiWindow.Enable(new AssetBrowser());

            Debug.Log("Overriding asset database to use open project instead.");

            RenderCanvas.main.SetTargetFPS(60);
            Game.SetAssetDatabase(new AssetDatabase(EditorUtils.projectPath));
            GameSettings? gameSettings = Files.Load<GameSettings>(Game.GetAssetDatabase().GetAsset("gamesettings.json").GetSystemPath()); ;
            Game.GetAssetDatabase().Start();

            Compiler.Generate();
        }
    }
}
