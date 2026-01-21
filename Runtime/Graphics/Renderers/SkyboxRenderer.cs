using OpenTK.Graphics.OpenGL;

using Runtime.Calc;
using Runtime.Components.Core;
using Runtime.Data;
using Runtime.Graphics.Shaders;
using Runtime.Objects;
using Runtime.Scenes;

namespace Runtime.Graphics.Renderers
{
    public class SkyboxRenderer : MeshRenderer
    {
        private ShaderProgram skyboxShader;
        [Inspectable] public CubemapTexture? cubemapTexture;


        public SkyboxRenderer(ShaderProgram skyboxShader, CubemapTexture? cubemapTexture)
        {
            this.Order = 10000000; // Render us LAST
            this.skyboxShader = skyboxShader;
            this.cubemapTexture = cubemapTexture;
            this.SetMesh(PrimativeMesh.CreateCubeMesh());
        }

        public SkyboxRenderer() : this(SkyboxRendererShader.CreateShader(), null)
        {

        }

        public override void Unload()
        {
            Scene.main.SetSkybox(null);
        }

        public override void Render(bool useMaterial = true)
        {
            if (mesh == null)
            {
                this.SetMesh(PrimativeMesh.CreateCubeMesh());
            }

            if (cubemapTexture != null)
            {
                Camera? camera = Camera.main;
                if (camera == null)
                    return;

                Scene.main.SetSkybox(this);
                GL.DepthFunc(DepthFunction.Lequal);
                GL.DepthMask(false);

                skyboxShader.Use();
                cubemapTexture.Use(TextureUnit.Texture0);

                Matrix4 view = camera.GetViewMatrix();

                // Remove translation
                view.m30 = 0f;
                view.m31 = 0f;
                view.m32 = 0f;

                skyboxShader.SetMatrix4("view", view);
                skyboxShader.SetMatrix4("projection", camera.GetProjectionMatrix());

                base.Render(false);

                GL.DepthMask(true);
                GL.DepthFunc(DepthFunction.Less);
            }
            else
            {
                Scene.main.SetSkybox(null);
            }
        }
    }

    public class SkyboxRendererShader
    {
        public static ShaderProgram CreateShader()
        {
            return new ShaderProgram("#version 330 core\r\nlayout (location = 0) in vec3 aPosition;\r\n\r\nout vec3 TexCoords;\r\n\r\nuniform mat4 view;\r\nuniform mat4 projection;\r\n\r\nvoid main()\r\n{\r\n    TexCoords = aPosition;\r\n    vec4 pos = projection * view * vec4(aPosition, 1.0);\r\n    gl_Position = pos.xyww; // forces depth to 1.0\r\n}\r\n", "#version 330 core\r\nin vec3 TexCoords;\r\nout vec4 FragColor;\r\n\r\nuniform samplerCube skybox;\r\n\r\nvoid main()\r\n{\r\n    FragColor = texture(skybox, TexCoords);\r\n}\r\n");
        }
    }
}
