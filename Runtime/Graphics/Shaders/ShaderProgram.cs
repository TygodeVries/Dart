using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using Runtime.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runtime.Graphics.Shaders
{
    public class ShaderProgram
    {
        bool compiled = false;
        private int shaderProgramId;
        public void Use()
        {
            if (!compiled)
                Compile();

            GL.UseProgram(shaderProgramId);
        }

        public static ShaderProgram FromFile(string vertex, string fragment)
        {
            string vertexContent = File.ReadAllText(vertex);
            string fragmentContent = File.ReadAllText(fragment);

            return new ShaderProgram(vertexContent, fragmentContent);
        }

        string vertexSource;
        string fragmentSource;
        public ShaderProgram(string vertexShader, string fragmentShader)
        {
            if(vertexShader.Length < 20)
            {
                Debug.Log($"VertexShader does not look like source code, please be aware. {vertexShader}");
            }

            if (fragmentShader.Length < 20)
            {
                Debug.Log($"FragmentShader does not look like source code, please be aware. {fragmentShader}");
            }


            this.vertexSource = vertexShader;
            this.fragmentSource = fragmentShader;
        }

        public void Compile()
        {
            int vertex = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertex, vertexSource);
            GL.CompileShader(vertex);

            int fragment = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragment, fragmentSource);
            GL.CompileShader(fragment);

            int program = GL.CreateProgram();
            GL.AttachShader(program, vertex);
            GL.AttachShader(program, fragment);
            GL.LinkProgram(program);
            GL.ValidateProgram(program);

            if (GL.GetError() != 0)
            {
                Debug.Error("Shader complication resulted in an error!" + GL.GetError());
            }
            GL.DeleteShader(vertex);
            GL.DeleteShader(fragment);

            shaderProgramId = program;
            compiled = true;

            string infoLog;
            GL.GetShaderInfoLog(fragment, out infoLog);
            if (infoLog.Length > 3)
                Debug.Error($"Compiling fragment shader resulted an an error: {infoLog}");

            GL.GetShaderInfoLog(vertex, out infoLog);
            if (infoLog.Length > 3)
                Debug.Error($"Compiling vertex shader resulted an an error: {infoLog}");
        }


        public void SetFloat(string field, float f)
        {
            int mvpLocation = GetUniformLocation(field);
            GL.Uniform1f(mvpLocation, f);
        }

        public void SetVector3(string field, Vector3 vector3)
        {
            int mvpLocation = GetUniformLocation(field);
            GL.Uniform3f(mvpLocation, vector3.X, vector3.Y, vector3.Z);
        }


        public void SetMatrix4(string field, Matrix4 matrix4)
        {
            int mvpLocation = GetUniformLocation(field);
            GL.UniformMatrix4f(mvpLocation, 1, false, ref matrix4);
        }

        public void SetInt(string field, int i)
        {
            int mvpLocation = GetUniformLocation(field);
            GL.Uniform1i(mvpLocation, i);
        }

        public void SetTextureId(string field, int id)
        {
            int mvpLocation = GetUniformLocation(field);
            GL.Uniform1i(mvpLocation, id);
        }

        private Dictionary<string, int> uniformLocations = new Dictionary<string, int>();

        private int GetUniformLocation(string name)
        {
            if (uniformLocations.TryGetValue(name, out int location))
                return location;

            location = GL.GetUniformLocation(shaderProgramId, name);
            if (location == -1)
                Debug.Error($"Error: Value '{name}' not found in shader!");

            uniformLocations[name] = location;
            return location;
        }

        public void SetVector3Array(string uniformName, Vector3[] vectors)
        {
            for (int i = 0; i < vectors.Length; i++)
            {
                string elementName = $"{uniformName}[{i}]";
                int location = GL.GetUniformLocation(shaderProgramId, elementName);
                if (location != -1)
                {
                    GL.Uniform3f(location, vectors[i].X, vectors[i].Y, vectors[i].Z);
                }
            }
        }

        public void SetIntArray(string uniformName, int[] values)
        {
            int location = GL.GetUniformLocation(shaderProgramId, uniformName);
            if (location == -1) return;
            GL.Uniform1i(location, values.Length, values);
        }

        public void SetMatrix4Array(string uniformName, Matrix4[] matrices)
        {
            for (int i = 0; i < matrices.Length; i++)
            {
                string elementName = $"{uniformName}[{i}]";
                int location = GL.GetUniformLocation(shaderProgramId, elementName);
                if (location != -1)
                {
                    GL.UniformMatrix4f(location, 1, false, ref matrices[i]);
                }
            }
        }
    }
}
