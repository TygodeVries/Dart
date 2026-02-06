using OpenTK.Graphics.OpenGL;
using Runtime.Calc;
using Runtime.Components.Core;
using Runtime.Graphics.Materials;
using Runtime.Graphics.Renderers;
using Runtime.Logging;
using Runtime.Objects;
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

            AddRenderPass(GizmoRenderPass.GetInstance());

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


        /// <summary>
        /// Add a renderer to be rendered starting the next frame
        /// </summary>
        /// <param name="renderer"></param>
        public void AddRenderer(Renderer renderer)
        {
            renderers.Add(renderer);
        }

        /// <summary>
        /// Remove a renderer from rendering.
        /// </summary>
        /// <param name="renderer"></param>
        public void RemoveRenderer(Renderer renderer)
        {
            renderers.Remove(renderer);
        }

        /// <summary>
        /// Returns the amount of renderers that are rendered every frame (Scene count, not drawcall count)
        /// </summary>
        /// <returns></returns>
        public int GetRendererCount()
        {
            return renderers.Count;
        }

        // Anything that needs to be rendered by this graphics pipeline
        List<Renderer> renderers = new List<Renderer>();

        // Any custom passes we might need to do (ui?)
        List<RenderPass> customRenderPasses = new List<RenderPass>();

        // Different perspectives to render from
        List<Camera> cameras = new List<Camera>();

        public void AddRenderPass(RenderPass renderPass)
        {
            customRenderPasses.Add(renderPass);
        }

        /// <summary>
        /// Remove all renderers of a spesific scene from the rendering process
        /// </summary>
        /// <param name="scene"></param>
        public void ClearRenderersOfScene(Scene scene)
        {
            for (int i = renderers.Count - 1; i >= 0; i--)
            {
                Renderer renderer = renderers[i];

                foreach (GameObject gameObject in scene.GetGameObjects())
                {
                    if (gameObject.HasComponent(renderer))
                    {
                        renderers.RemoveAt(i);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Add a camera to be rendererd
        /// </summary>
        /// <param name="camera"></param>
        public void AddCamera(Camera camera)
        {
            cameras.Add(camera);
        }

        /// <summary>
        /// Fallback if there are no cameras rendering
        /// </summary>
        private void NoCameras()
        {
            GL.ClearColor(0.1f, 0.1f, 0.1f, 1);
        }

        /// <summary>
        /// Render all the rendereres in openGL
        /// </summary>
        /// <param name="view"></param>
        /// <param name="projection"></param>
        private void RenderRenderers(Matrix4 view, Matrix4 projection)
        {
            foreach (Renderer renderer in renderers)
            {
                if (!renderer.Visible)
                    continue;

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
        }

        public void Render()
        {
            Scene.main.GetLightManager().UploadAll();

            Matrix4 view = Matrix4.MultiplicativeIdentity;
            Matrix4 projection = Matrix4.MultiplicativeIdentity;

            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Less);

            if (cameras.Count == 0)
            {
                NoCameras();
            }

            // Presort render order
            renderers.Sort((a, b) => a.Order - b.Order);

            // For every camera currently loaded
            foreach (Camera camera in cameras)
            {
                camera.startRender?.Invoke();
                // Set render texture
                if (camera.renderTexture != null)
                    camera.renderTexture.Bind();
                else
                {
                    GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0); // Reset if nothing, aka draw to the screen
                    GL.Viewport(0, 0, RenderCanvas.main!.Size.X, RenderCanvas.main!.Size.Y);
                }

                // Set background color
                GL.ClearColor(camera.backgroundColor.x, camera.backgroundColor.y, camera.backgroundColor.z, 1);

                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

                // Set Matrixes for rendering
                view = camera.GetViewMatrix();
                projection = camera.GetProjectionMatrix();

                RenderRenderers(view, projection);

                // Reset render texture
                if (camera.renderTexture != null)
                    camera.renderTexture.Unbind(RenderCanvas.main!.Size.X, RenderCanvas.main!.Size.Y);
                camera.endRender?.Invoke();
            }

            // Back to main
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.Viewport(0, 0, RenderCanvas.main!.FramebufferSize.X, RenderCanvas.main!.FramebufferSize.Y);

            foreach (RenderPass renderPass in customRenderPasses)
            {
                renderPass.Pass();
            }
        }
    }
}