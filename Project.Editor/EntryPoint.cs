using Project.Editor;
using Project.Editor.UI;
using Project.Editor.UI.FileSystem;
using Project.Editor.UI.Inspectors;
using Runtime;
using Runtime.Component.Core;
using Runtime.Component.Physics;
using Runtime.Component.Test;
using Runtime.Data;
using Runtime.DearImGUI.Gui;
using Runtime.Graphics.Materials;
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

            Debug.Log("Overriding asset database to use open project instead.");

            Game.SetAssetDatabase(new AssetDatabase(EditorUtils.projectPath));
            Game.GetAssetDatabase().Start();


            AssetDatabase assets = EditorUtils.GetAssetDatabase();
            Mesh? mesh = Mesh.FromFileObj(assets.GetAsset("assets/models/modelpreview_sphere.obj"));
            Mesh? mesh2 = Mesh.FromFileObj(assets.GetAsset("assets/models/modelpreview_box.obj"));

            Material? material = new Material(
               Runtime.Graphics.Shaders.ShaderProgram.FromFile(assets.GetAsset("assets\\shaders\\previews\\model_untextured.vert"), assets.GetAsset("assets\\shaders\\previews\\model_untextured.frag")));

            GameObject cam;
            Scene.main.Instantiate(cam = new Runtime.Objects.GameObjectFactory()
               .AddComponent<Camera>()
               .AddComponent<Transform>()
               .AddComponent<FlightCamera>().Build());

            cam.GetComponent<Camera>().SetAsMain();

            GameObject t = new GameObjectFactory()
               .AddComponent(new MeshRenderer(material) { mesh = mesh2 })
               .AddComponent<Transform>().Build();

            Scene.main.Instantiate(t);

            Scene.main.Instantiate(new Runtime.Objects.GameObjectFactory()
               .AddComponent(
               new SphereCollider() { radius = 1 }
               ).AddComponent(new MeshRenderer(material)
               {
                   mesh = mesh
               }).AddComponent(new RaycastTester(t)).Build());

            Game.GetAssetDatabase().Refresh();

            TextureMaterialField.fallback = DefaultsTextures.GetFallbackTexture();
        }
    }
}
