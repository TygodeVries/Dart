using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common.Input;
using Runtime.Component.Core;
using Runtime.Calc;
using Runtime.Graphics.Materials;
using Runtime.Graphics.Renderers;
using Runtime.Graphics.Shaders;
using Runtime.Logging;
using Runtime.Scenes;
using Runtime.Objects;
using Runtime.Component.Test;
using OpenTK.Graphics.Vulkan.VulkanVideoCodecH264stdEncode;
using Runtime.Component.Lighting;
using Editor.ImGuiEditor;
using Runtime.Asm;
using System.Reflection;

namespace Runtime.Graphics.Pipeline
{
    public class DefaultGraphicsPipeline : IGraphicsPipeline
    {  
        public void Initialize()
        {
            Debug.Log("Initializing...");
            GL.ClearColor(0, 0, 0, 0);

            customRenderPasses.Add(new ImGuiRenderPass());


            EnableCap[] caps = new EnableCap[]
            {
                EnableCap.LineSmooth,
                EnableCap.PolygonSmooth,
                EnableCap.Multisample
            };
            Debug.Log("Turning on OpenGL features...");
            string features = "";
            foreach(EnableCap enableCap in caps)
            {
                features += enableCap;
                GL.Enable(enableCap);
            }

            Debug.Log($"Enabled features ({features})!");

            Debug.Log("Calling custom render pass start.");
            foreach (RenderPass renderPass in customRenderPasses)
            {
                renderPass.Start();
            }

            Mesh mesh = Mesh.FromFileObj("assets/Sphere.obj");

            ShaderProgram shaderProgram = ShaderProgram.FromFile("assets/shaders/lit.vert", "assets/shaders/lit.frag");
            
            Material material = new Material(shaderProgram);

            material.SetTexture("u_Texture", new Texture("assets/textures/moss/color.png"), 0);
            material.SetTexture("u_NormalMap", new Texture("assets/textures/moss/normal.png"), 1);
            material.SetTexture("u_Rough", new Texture("assets/textures/moss/rough.png"), 2);

            material.EnableLightData();

            Scene.main.Instantiate(new GameObjectFactory()
                .AddComponent(new TextRenderer("What is your point?\nA triangle has 3.", TextSpace.World))
                .AddComponent(new Transform()
                {
                    position = new Vector3(-0.5f, 0.8f, 0)
                })
                .Build());

            Scene.main.Instantiate(new GameObjectFactory()
                .AddComponent(new Camera())
                .AddComponent(new Transform())
                .AddComponent(new TestCameraControls())
                .Build());

            Scene.main.Instantiate(new GameObjectFactory()
                .AddComponent(new MeshRenderer(material, mesh))
                .AddComponent(new Transform()
                {
                    position = new Vector3(0, -2, 0)
                })
                .Build());

            Scene.main.Instantiate(new GameObjectFactory()
                .AddComponent(new PointLight()
                {
                    intensity = 2,
                    color = new Vector3(1, 1, 1)
                })
                .Build());
        }
       
        public void AddRenderer(Renderer renderer)
        {
            renderers.Add(renderer);
            Debug.Log("Added renderer: " + renderers.Count);
        }

        // Anything that needs to be renderered by this graphics pipeline
        List<Renderer> renderers = new List<Renderer>();

        // Any custom passes we might need to do (ui?)
        public List<RenderPass> customRenderPasses = new List<RenderPass>();

        bool sendNoCameraIssue;
        public void Render()
        {
            Scene.main.GetLightManager().UploadAll();
            
            if(Camera.main == null)
            {
                GL.ClearColor(1, 0, 0, 1);
                if(!sendNoCameraIssue) Debug.Error("No camera is rendering to the screen, you must have at least 1 camera to render from.");
                sendNoCameraIssue = true;
                return;
            }
            else
            {
                // Set to camera background color   
                GL.ClearColor(Camera.main.backgroundColor.X, Camera.main.backgroundColor.Y, Camera.main.backgroundColor.Z, 1);
            }

            Camera renderCamera = Camera.main;
            Matrix4 view = renderCamera.GetViewMatrix();
            Matrix4 projection = renderCamera.GetProjectionMatrix();

            foreach(Renderer renderer in renderers)
            {
                Material? material = renderer.GetMaterial();

                if (material != null && material.matrixEnabled)
                {
                    material.SetMatrix4("uView", view);
                    material.SetMatrix4("uProjection", projection);

                    Matrix4 model;
                    Transform? transform = renderer.GetComponent<Transform>();
                    if (transform == null)
                    {
                        // If no transform, just go to the default
                        model = Matrix4.CreateTranslation(0, 0, 0);
                    }
                    else
                    {
                        model = transform.GetMatrix();
                    }

                    material.SetMatrix4("uModel", model);
                }

                renderer.Render();
            }

            foreach(RenderPass renderPass in customRenderPasses)
            {
                renderPass.Pass();
            }
        }
    }
}