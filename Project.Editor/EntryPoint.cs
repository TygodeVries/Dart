using Project.Editor;
using Project.Editor.Code;
using Project.Editor.UI;
using Project.Editor.UI.Assets;
using Project.Editor.UI.FileSystem;
using Project.Editor.UI.Inspectors;
using Project.Editor.UI.Styles;
using Runtime;
using Runtime.Calc;
using Runtime.Components.Core;
using Runtime.Components.Test;
using Runtime.Data;
using Runtime.DearImGUI.Gui;
using Runtime.Graphics;
using Runtime.Graphics.Renderers;
using Runtime.Logging;
using Runtime.Objects;
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
            GuiWindow.Enable(new AssetBrowser());

            Debug.Log("Overriding asset database to use open project instead.");

            RenderCanvas.main.SetTargetFPS(60);
            Game.SetAssetDatabase(new AssetDatabase(EditorUtils.projectPath));
            Asset settingAsset = Game.GetAssetDatabase().GetAsset("gamesettings.json");
            GameSettings? gameSettings = Files.Load<GameSettings>(settingAsset.GetSystemPath()); ;
            gameSettings.asset = settingAsset;

            Game.GetAssetDatabase().Start();

            Compiler.Generate();


            MainThread.Run(() =>
            {
                Camera camera = new Camera();
                camera.SetAsMain();
                Scene.main.Instantiate(new GameObjectFactory()
                    .AddComponent(camera)
                    .AddComponent(new FlightCamera())
                    .AddComponent(new Transform())
                    .Build());

                Scene.main.Instantiate(new GameObjectFactory()
                    .AddComponent(new MeshRenderer()
                    {
                        mesh = PrimativeMesh.CreateCubeMesh()
                    })
                    .Build());
            });
        }
    }
}
