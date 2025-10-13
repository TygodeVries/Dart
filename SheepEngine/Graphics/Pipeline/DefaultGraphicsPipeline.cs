using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common.Input;
using Runtime.Component.Core;
using Runtime.Component.Lighting;
using Runtime.Calc;
using Runtime.Graphics.Materials;
using Runtime.Graphics.Renderers;
using Runtime.Graphics.Shaders;

namespace Runtime.Graphics.Pipeline
{
    public class DefaultGraphicsPipeline : IGraphicsPipeline
    {

        DefaultLightManager defaultLightManager = new DefaultLightManager();
        public void Initialize()
        {
            // Load the materials
            ShaderProgram program = ShaderProgram.FromFile("assets/Shaders/lit.vert", "assets/Shaders/lit.frag");
            program.Compile();


            ShaderProgram skyboxShader = ShaderProgram.FromFile("assets/Shaders/skybox.vert", "assets/Shaders/skybox.frag");

            Texture rockColor = new Texture("Assets/Textures/Moss/color.png");
            Texture rockNormal = new Texture("Assets/Textures/Moss/normal.png");
            Texture rockRough = new Texture("Assets/Textures/Moss/rough.png");

            sphereMaterial = new Material(program);
            sphereMaterial.SetTexture("u_Texture", rockColor, 0);
            sphereMaterial.SetTexture("u_NormalMap", rockNormal, 1);
            sphereMaterial.SetTexture("u_Rough", rockRough, 2);

            skyboxMaterial = new Material(skyboxShader);
            skyboxMaterial.SetTexture("u_Texture", rockColor, 0);

            // 46, 68, 130
            GL.ClearColor(46 / 255f, 68 / 255f, 130 / 255f, 1);

            defaultLightManager.AddEffected(sphereMaterial);

            foreach (RenderPass renderPass in customRenderPasses)
            {
                renderPass.Start();
            }
        }
        Material sphereMaterial;
        Material skyboxMaterial;

        public void AddRenderer(Renderer renderer)
        {
            renderers.Add(renderer);
            Console.WriteLine("Added renderer: " + renderers.Count);
        }
        float time = 0;
        List<Renderer> renderers = new List<Renderer>();

        public List<RenderPass> customRenderPasses = new List<RenderPass>();
        public void Render()
        {
            defaultLightManager.UploadAll();
            if(Camera.main == null)
            {
                GL.ClearColor(1, 0, 0, 1);
                Console.WriteLine("No camera's rendering");
                return;
            }
            else
            {
                GL.ClearColor(46 / 255f, 68 / 255f, 130 / 255f, 1);
            }

            sphereMaterial.SetVector3("u_camera_pos", Camera.main.GetComponent<Transform>().position);
            skyboxMaterial.SetVector3("u_camera_forwards", Camera.main.GetComponent<Transform>().GetForwards());
            sphereMaterial.SetFloat("u_shininess", 8);
            time += (float)Time.deltaTime * 0.5f;
            float x = MathF.Cos(time * MathF.PI);
            float z = MathF.Sin(time * MathF.PI);
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