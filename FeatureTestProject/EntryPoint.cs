
using FeatureTestProject.Components;
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
using Runtime;
using Runtime.Component.Core;
using Runtime.Component.Lighting;
using Runtime.Component.Test;
using Runtime.DearImGUI.Gui;
using Runtime.Graphics;
using Runtime.Graphics.Materials;
using Runtime.Graphics.Renderers;
using Runtime.Graphics.Shaders;
using Runtime.Objects;
using Runtime.Plugins;
using Runtime.Scenes;
using System.Runtime.CompilerServices;

namespace FeatureTestProject
{
	[DartEntryPoint("Main")]
	public class EntryPoint
	{
		public EntryPoint()
		{

		}

		public static void Main()
		{
            /*
             *  LETS FIRST LOAD SOME ASSETS
             */

            // Opening windows
			GuiWindow.Enable(new AudioTestWindow());
			GuiWindow.Enable(new GUIPerformanceWindow());

            // Load in our lit shader
            ShaderProgram litShader = ShaderProgram.FromFile("assets/shaders/lit.vert", "assets/shaders/lit.frag");

            // Get our scene
            Scene scene = Scene.main;

            // Load a mesh
            Mesh sphereMesh = Mesh.FromFileObj("assets/insane.obj");

            // Make a material
            Material mossMaterial = new Material(litShader);

            // Load in some texture
            mossMaterial.SetTexture("u_Texture", new Texture("assets/textures/moss/color.png"), 0); // Albedo
            mossMaterial.SetTexture("u_NormalMap", new Texture("assets/textures/moss/normal.png"), 1); // Normal
            mossMaterial.SetTexture("u_Rough", new Texture("assets/textures/moss/rough.png"), 2); // Rough
            
            // Give us LIGHT
            mossMaterial.EnableLightData();

            // Add some text
            Scene.main.Instantiate(new GameObjectFactory()
                .AddComponent(new TextRenderer("Hello, World!", TextSpace.World))
                .AddComponent(new Transform()
                {
                    position = new Vector3(-0.5f, 0.8f, 0)
                })
                .Build());

            /*
             *  HERE WE START BUILDING THE SCENE
             */

            // Create a camera
            Scene.main.Instantiate(new GameObjectFactory()
                .AddComponent(new Camera())
                .AddComponent(new Transform())
                .AddComponent(new TestCameraControls())
                .AddComponent(new CursorCapture())
                .Build());

            // Load a sphere
            Scene.main.Instantiate(new GameObjectFactory()
                .AddComponent(new MeshRenderer(mossMaterial, sphereMesh))
                .AddComponent(new Transform()
                {
                    position = new Vector3(0, -2, 0)
                })
                .Build());

            // Create a light
            Scene.main.Instantiate(new GameObjectFactory()
                .AddComponent(new PointLight()
                {
                    intensity = 2,
                    color = new Vector3(1, 1, 1)
                })
                .Build());

        }
	}
}