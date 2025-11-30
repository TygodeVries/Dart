using Project.Editor.UI;
using Project.Editor.UI.FileSystem;
using Project.Editor.UI.Inspectors;
using Runtime.Component.Core;
using Runtime.Component.Physics;
using Runtime.Component.Test;
using Runtime.DearImGUI.Gui;
using Runtime.Graphics.Materials;
using Runtime.Graphics.Renderers;
using Runtime.Graphics.Shaders;
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


            float backgroundGrayness = 30;
            Camera camera = new Camera()
            {
                backgroundColor = new OpenTK.Mathematics.Vector3(backgroundGrayness / 255f, backgroundGrayness / 255f, backgroundGrayness / 255f)
            };

            camera.SetAsMain();
            Scene.main.Instantiate(new GameObjectFactory().
                AddComponent(camera)
                .AddComponent(new Transform()
                {
                    position = new OpenTK.Mathematics.Vector3(0, 0, 3),
                    rotation = new OpenTK.Mathematics.Vector3(180, 0, 0)
                })
                .AddComponent(new TestCameraControls())
                .Build());

            Mesh cube = Mesh.FromFileObj("assets/models/cube.obj");
            Material material = new Material(ShaderProgram.FromFile("assets/shaders/previews/model_untextured.vert", "assets/shaders/previews/model_untextured.frag"));

            Scene.main.Instantiate(new GameObjectFactory()
                .AddComponent(new MeshRenderer(material)
                {
                    mesh = cube
                })
                .AddComponent(new AABBBoxCollider()
                {
                    size = new OpenTK.Mathematics.Vector3(2, 2, 2)
                })
                .AddComponent(new Transform()
                {
                    position = new OpenTK.Mathematics.Vector3(0, 0, 0)
                })
                .AddComponent(new RaycastTester())
                .Build());
        }
    }
}
