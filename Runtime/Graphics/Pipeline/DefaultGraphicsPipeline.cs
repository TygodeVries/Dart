using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using Runtime.Component.Core;
using Runtime.Graphics.Materials;
using Runtime.Graphics.Renderers;
using Runtime.Logging;
using Runtime.Scenes;

namespace Runtime.Graphics.Pipeline
{
    public class DefaultGraphicsPipeline : IGraphicsPipeline
    {
        public void Initialize()
        {
            Debug.Log("Initializing...");
            GL.ClearColor(0, 0, 0, 0);

            EnableCap[] caps = new EnableCap[]
            {
                     EnableCap.Multisample
            };
            Debug.Log("Turning on OpenGL features...");
            string features = "";
            foreach (EnableCap enableCap in caps)
            {
                features += $"- {enableCap}";
                GL.Enable(enableCap);
            }

            Debug.Log($"Enabled features ({features})!");

            Debug.Log("Calling custom render pass start.");
            foreach (RenderPass renderPass in customRenderPasses)
            {
                renderPass.Start();
            }
        }

        public void AddRenderer(Renderer renderer)
        {
            renderers.Add(renderer);
            Debug.Log("Added renderer: " + renderers.Count);
        }

        public int GetRendererCount()
        {
            return renderers.Count;
        }

        // Anything that needs to be renderered by this graphics pipeline
        List<Renderer> renderers = new List<Renderer>();

        // Any custom passes we might need to do (ui?)
        List<RenderPass> customRenderPasses = new List<RenderPass>();

        public void AddRenderPass(RenderPass renderPass)
        {
            customRenderPasses.Add(renderPass);
        }

        bool sendNoCameraIssue;
        public void Render()
        {
            Scene.main.GetLightManager().UploadAll();

            Matrix4 view = Matrix4.MultiplicativeIdentity;
            Matrix4 projection = Matrix4.MultiplicativeIdentity;

            if (Camera.main != null)
            {
                // Set to camera background color   
                GL.ClearColor(Camera.main.backgroundColor.X, Camera.main.backgroundColor.Y, Camera.main.backgroundColor.Z, 1);
                Camera renderCamera = Camera.main;
                view = renderCamera.GetViewMatrix();
                projection = renderCamera.GetProjectionMatrix();
            }


            foreach (Renderer renderer in renderers)
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

            foreach (RenderPass renderPass in customRenderPasses)
            {
                renderPass.Pass();
            }
        }
    }
}