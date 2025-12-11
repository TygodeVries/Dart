using OpenTK.Graphics.OpenGL;
using Runtime.Calc;
using Runtime.Logging;

namespace Runtime.Graphics.Shaders
{
    /// <summary>
    /// #TODO add a dispose
    /// </summary>
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
            try
            {
                string vertexContent = File.ReadAllText(vertex);
                string fragmentContent = File.ReadAllText(fragment);

                return new ShaderProgram(vertexContent, fragmentContent);
            }
            catch (Exception e)
            {
                Debug.Error($"Could not load ShaderProgram from files {vertex} & {fragment}! Because: " + e);
                throw new FileNotFoundException();
            }
        }

        List<Uniform> uniforms = new();
        private void AnalyzeSourceForUniforms()
        {
            uniforms.AddRange(UniformFinder.FindUniformsInSource(vertexSource));
            uniforms.AddRange(UniformFinder.FindUniformsInSource(fragmentSource));

            uniforms.Distinct();
        }

        public List<Uniform> GetUniforms()
        {
            return uniforms;
        }

        string vertexSource;
        string fragmentSource;
        public ShaderProgram(string vertexShader, string fragmentShader)
        {
            if (vertexShader.Length < 20)
            {
                Debug.Log($"VertexShader does not look like source code, please be aware. {vertexShader}");
            }

            if (fragmentShader.Length < 20)
            {
                Debug.Log($"FragmentShader does not look like source code, please be aware. {fragmentShader}");
            }


            this.vertexSource = vertexShader;
            this.fragmentSource = fragmentShader;

            AnalyzeSourceForUniforms();
        }

        private string CleanDartTokens(string source)
        {
            return source.Replace("%show", "");
        }

        public void Compile()
        {
            int vertex = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertex, CleanDartTokens(vertexSource));
            GL.CompileShader(vertex);

            int fragment = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragment, CleanDartTokens(fragmentSource));
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
            GL.Uniform3f(mvpLocation, vector3.x, vector3.y, vector3.z);
        }

        public void SetVector4(string field, Vector4 vector)
        {
            int mvpLocation = GetUniformLocation(field);
            GL.Uniform4f(mvpLocation, vector.x, vector.y, vector.z, vector.w);
        }


        public void SetMatrix4(string field, Matrix4 matrix4)
        {
            int mvpLocation = GetUniformLocation(field);

            OpenTK.Mathematics.Matrix4 m = matrix4.ToOpenTK();
            GL.UniformMatrix4f(mvpLocation, 1, false, ref m);
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
            {
                Debug.Error($"Error: Value '{name}' not found in shader!");
            }

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
                    GL.Uniform3f(location, vectors[i].x, vectors[i].y, vectors[i].z);
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
                    OpenTK.Mathematics.Matrix4 matrix = matrices[i].ToOpenTK();
                    GL.UniformMatrix4f(location, 1, false, ref matrix);
                }
            }
        }
    }
}
