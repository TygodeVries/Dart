using Project.Editor.Data;
using Project.Editor.UI;
using Project.Editor.UI.FileSystem;
using Project.Editor.UI.Inspectors;
using Runtime.Component.Core;
using Runtime.Component.Physics;
using Runtime.Component.Test;
using Runtime.DearImGUI.Gui;
using Runtime.Graphics.Materials;
using Runtime.Graphics.Renderers;
using Runtime.Logging;
using Runtime.Objects;
using Runtime.Physics.Raycasts;
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
                    Project.Editor.Editor.projectPath = args[cx + 1];
                }
                if (args[cx] == "-e")
                {
                    Project.Editor.Editor.exeLocation = args[cx + 1];
                }
            }

            GuiWindow.Enable(new NavBarUI());
            GuiWindow.Enable(new ProjectWindow());
            GuiWindow.Enable(new InspectorWindow());
            AssetDatabase.Start();


         Mesh? mesh = Mesh.FromFileObj("assets\\models\\modelpreview.obj");
         Mesh? mesh2 = Mesh.FromFileObj("assets\\models\\modelpreview.obj");
         
         Material? material = new Material(
            Runtime.Graphics.Shaders.ShaderProgram.FromFile("assets\\shaders\\previews\\model_untextured.vert", "assets\\shaders\\previews\\model_untextured.frag"));

         GameObject cam;
         Scene.main.Instantiate(cam = new Runtime.Objects.GameObjectFactory()
            .AddComponent<Camera>()
            .AddComponent<Transform>()
            .AddComponent<TestCameraControls>().Build());

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

        }
    }
}
