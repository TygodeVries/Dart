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

namespace Runtime.Graphics.Pipeline
{
    public class DefaultGraphicsPipeline : IGraphicsPipeline
    {  
        public void Initialize()
        {
            Debug.Log("Initializing...");
            GL.ClearColor(0, 0, 0, 0);

            Debug.Log("Calling custom render pass start.");
            foreach (RenderPass renderPass in customRenderPasses)
            {
                renderPass.Start();
            }

            Scene.main.Instantiate(new GameObjectFactory()
                .AddComponent(new TextRenderer("Hello, World!"))
                .Build());

            Scene.main.Instantiate(new GameObjectFactory()
                .AddComponent(new Camera())
                .AddComponent(new Transform())
                .AddComponent(new TestCameraControls())
                .Build());
        }
       
        public void AddRenderer(Renderer renderer)
        {
            renderers.Add(renderer);
            Console.WriteLine("Added renderer: " + renderers.Count);
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
                Material material = renderer.GetMaterial();

                if (material != null)
                {
                    material.SetMatrix4("uView", view);
                    material.SetMatrix4("uProjection", projection);

                    Matrix4 model;
                    Transform transform = renderer.GetComponent<Transform>();
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