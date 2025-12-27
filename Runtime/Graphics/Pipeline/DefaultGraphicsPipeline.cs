using OpenTK.Graphics.OpenGL;
using Runtime.Calc;
using Runtime.Component.Core;
using Runtime.Graphics.Materials;
using Runtime.Graphics.Renderers;
using Runtime.Logging;
using Runtime.Scenes;

namespace Runtime.Graphics.Pipeline
{
    public class DefaultGraphicsPipeline : IGraphicsPipeline
    {
        GLDebugProc? GLDebugProc;
        public void Initialize()
        {
            Debug.Log("Initializing...");

            GLDebugProc += (DebugSource source, DebugType type, uint id,
                DebugSeverity severity, int length, nint message, nint ud) =>
            {
                unsafe
                {
                    string str = new string((sbyte*)message);
                    switch (severity)
                    {
                        case DebugSeverity.DebugSeverityNotification:
                            break;
                        default:
                            Debug.Log($"OpenGL:" + str);
                            break;
                    }
                }
            };
            GL.DebugMessageCallback(GLDebugProc, 0);

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

        public void RemoveRenderer(Renderer renderer)
        {
            renderers.Remove(renderer);

        }

        public int GetRendererCount()
        {
            return renderers.Count;
        }

        // Anything that needs to be rendered by this graphics pipeline
        List<Renderer> renderers = new List<Renderer>();

        // Any custom passes we might need to do (ui?)
        List<RenderPass> customRenderPasses = new List<RenderPass>();

        public void AddRenderPass(RenderPass renderPass)
        {
            customRenderPasses.Add(renderPass);
        }

        public void ClearRenderers()
        {
            renderers.Clear();
        }
        public void Render()
        {
            Scene.main.GetLightManager().UploadAll();

            Matrix4 view = Matrix4.MultiplicativeIdentity;
            Matrix4 projection = Matrix4.MultiplicativeIdentity;

            if (Camera.main != null)
            {
                // Set to camera background color   
                GL.ClearColor(Camera.main.backgroundColor.x, Camera.main.backgroundColor.y, Camera.main.backgroundColor.z, 1);
                Camera renderCamera = Camera.main;
                view = renderCamera.GetViewMatrix();
                projection = renderCamera.GetProjectionMatrix();
            }
            else
            {
                GL.ClearColor(0.5f, 0, 0, 1);
            }

            foreach (Renderer renderer in renderers)
            {
                Material? material = renderer.GetMaterial();

                if (material != null && material.matrixEnabled)
                {
                    material.SetMatrix4("u_View", view);
                    material.SetMatrix4("u_Projection", projection);

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

                    material.SetMatrix4("u_Model", model);
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