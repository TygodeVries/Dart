using Project.Editor;
using Project.Editor.UI;
using Project.Editor.UI.FileSystem;
using Project.Editor.UI.Inspectors;
using Runtime;
using Runtime.Component.Core;
using Runtime.Data;
using Runtime.DearImGUI.Gui;
using Runtime.Logging;
using Runtime.Objects;
using Runtime.Objects.Prefabs;
using Runtime.Scenes;

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

            Debug.Log("Overriding asset database to use open project instead.");

            Game.SetAssetDatabase(new AssetDatabase(EditorUtils.projectPath));
            Game.GetAssetDatabase().Start();

            Scene.main.Instantiate(new GameObjectFactory().AddComponent(new Camera()).Build());

            Scene scene = new Scene();
            PrefabGameObject game = PrefabGameObject.FromFile(Game.GetAssetDatabase().GetAsset("assets/untitled.prefab"));

            Scene.Load(scene);
            scene.Instantiate(game.GetGameObject());

            /*
            Scene.Load(new Scene());
            Scene.Load(scene);
            */
        }
    }
}
