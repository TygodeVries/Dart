using OpenTK.Graphics.OpenGL;
using Runtime.Calc;
using Runtime.Data;
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

        public static ShaderProgram FromFile(Asset vertex, Asset fragment)
        {
            try
            {
                Debug.Log($"Loading from shader path: {vertex.GetSystemPath()}");
                string vertexContent = File.ReadAllText(vertex.GetSystemPath());
                string fragmentContent = File.ReadAllText(fragment.GetSystemPath());

                return new ShaderProgram(vertexContent, fragmentContent);
            }
            catch (Exception e)
            {
                Debug.Error($"Could not load ShaderProgram from files {vertex.GetPath()} & {fragment.GetPath()}! Because: {e} at {e.StackTrace}");
                return new ShaderProgram("error", "error");
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

            Debug.Log($"Vertex shader is: {vertexShader}");
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

            GL.GetShaderi(vertex, ShaderParameterName.CompileStatus, out int vStatus);
            if (vStatus == 0)
            {
                GL.GetShaderInfoLog(vertex, out string vLog);
                Debug.Error($"Vertex shader compile error:\n{vLog}");
                return;
            }

            int fragment = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragment, CleanDartTokens(fragmentSource));
            GL.CompileShader(fragment);

            GL.GetShaderi(fragment, ShaderParameterName.CompileStatus, out int fStatus);
            if (fStatus == 0)
            {
                GL.GetShaderInfoLog(fragment, out string fLog);
                Debug.Error($"Fragment shader compile error:\n{fLog}");
                return;
            }

            int program = GL.CreateProgram();
            GL.AttachShader(program, vertex);
            GL.AttachShader(program, fragment);
            GL.LinkProgram(program);

            GL.GetProgrami(program, ProgramProperty.LinkStatus, out int linkStatus);
            if (linkStatus == 0)
            {
                GL.GetProgramInfoLog(program, out string pLog);
                Debug.Error($"Program link error:\n{pLog}");
                return;
            }

            GL.DeleteShader(vertex);
            GL.DeleteShader(fragment);

            shaderProgramId = program;
            compiled = true;

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
    public class ComputeShaderProgram
    {
        bool compiled = false;
        private int shaderProgramId;
        string sourceContent;
        public void Use()
        {
            if (!compiled)
                Compile();

            GL.UseProgram(shaderProgramId);
        }
        public static ComputeShaderProgram FromFile(string source)
        {
            string sourceContent = File.ReadAllText(source);

            return new ComputeShaderProgram(sourceContent);
        }

        public ComputeShaderProgram(string source)
        {
            if (source.Length < 20)
            {
                Debug.Log($"ComputeShader does not look like source code, please be aware. {source}");
            }



            this.sourceContent = source;
        }

        public void Compile()
        {
            int compute = GL.CreateShader(ShaderType.ComputeShader);
            GL.ShaderSource(compute, sourceContent);
            GL.CompileShader(compute);

            int program = GL.CreateProgram();
            GL.AttachShader(program, compute);
            GL.LinkProgram(program);
            GL.ValidateProgram(program);

            if (GL.GetError() != 0)
            {
                Debug.Error("Shader complication resulted in an error!" + GL.GetError());
            }

            shaderProgramId = program;
            compiled = true;

            string infoLog;
            GL.GetShaderInfoLog(compute, out infoLog);
            if (infoLog.Length > 3)
                Debug.Error($"Compiling fragment shader resulted an an error: {infoLog}");

            GL.DeleteShader(compute);
        }
        public void Check()
        {
            ErrorCode error = GL.GetError();
            if (ErrorCode.NoError != error)
            {
                Debug.Error($"OpenGL error {error}");
            }
        }
        public void Barrier()
        {
            GL.MemoryBarrier(MemoryBarrierMask.ShaderStorageBarrierBit | MemoryBarrierMask.AtomicCounterBarrierBit);
        }
        public void Dispatch(uint x)
        {
            Dispatch(x, 1, 1);
        }
        public void Dispatch(uint x, uint y)
        {
            Dispatch(x, y, 1);
        }
        public void Dispatch(uint x, uint y, uint z)
        {
            Use();
            GL.DispatchCompute(x, y, z);
            Check();
        }
        public int GenerateComputeBuffer(uint size_in_bytes)
        {
            int buffer = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ShaderStorageBuffer, buffer);
            Check();
            byte[] dummy = new byte[size_in_bytes];
            GL.BufferData(BufferTarget.ShaderStorageBuffer, (nint)size_in_bytes, dummy, BufferUsage.DynamicCopy);
            Check();
            return buffer;
        }
        public int GenerateAtomicBuffer(uint count_of_uints)
        {
            int buffer = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.AtomicCounterBuffer, buffer);
            Check();
            GL.BufferData(BufferTarget.AtomicCounterBuffer, (nint)count_of_uints * sizeof(uint), 0, BufferUsage.StaticRead);
            Check();
            return buffer;
        }
        public void BindComputeBuffer(int buffer, uint bind_point)
        {
            Use();
            GL.BindBufferBase(BufferTarget.ShaderStorageBuffer, bind_point, buffer);
        }
        public void BindAtomicBuffer(int buffer, uint bind_point)
        {
            Use();
            GL.BindBufferBase(BufferTarget.AtomicCounterBuffer, bind_point, buffer);
        }

        public unsafe void SetComputeBufferData<T>(int buffer, int offset, T[] data) where T : struct
        {
            GL.BindBuffer(BufferTarget.ShaderStorageBuffer, buffer);
            fixed (T* ptr = data)
            {
                GL.BufferSubData(BufferTarget.ShaderStorageBuffer, offset * sizeof(T), data.Length * sizeof(T), ptr);
            }
        }

        public void SetAtomicBufferData(int buffer, int offset, uint[] data)
        {
            GL.BindBuffer(BufferTarget.AtomicCounterBuffer, buffer);
            GL.BufferSubData(BufferTarget.AtomicCounterBuffer, offset * sizeof(uint), data.Length * sizeof(uint), data);
        }
        public unsafe void ReadComputeBufferData<T>(int buffer, int offset, T[] data) where T : struct
        {
            GL.MemoryBarrier(MemoryBarrierMask.ShaderStorageBarrierBit);
            GL.BindBuffer(BufferTarget.ShaderStorageBuffer, buffer);
            fixed (T* ptr = data)
            {
                GL.GetBufferSubData(BufferTarget.ShaderStorageBuffer, offset * sizeof(T), data.Length * sizeof(T), ptr);
            }
        }
        public void ReadAtomicBufferData(int buffer, int offset, uint[] data)
        {
            GL.MemoryBarrier(MemoryBarrierMask.AtomicCounterBarrierBit);
            Check();
            GL.BindBuffer(BufferTarget.AtomicCounterBuffer, buffer);
            Check();
            GL.GetBufferSubData(BufferTarget.AtomicCounterBuffer, offset * sizeof(uint), data.Length * sizeof(uint), data);
            Check();
        }
        public void DeleteBuffer(int buffer)
        {
            if (buffer != 0)
                GL.DeleteBuffer(buffer);
        }
        public unsafe uint SizeOf<T>()
        {
            return (uint)sizeof(T);
        }
    }
}
