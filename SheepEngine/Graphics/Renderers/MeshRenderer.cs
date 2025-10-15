using OpenTK.Graphics.OpenGL;
using Runtime.Graphics;
using Runtime.Graphics.Materials;
using Runtime.Graphics.Shaders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Runtime.Graphics.Renderers
{
    public class MeshRenderer : Renderer, IDisposable
    {
        public MeshRenderer()
        {

        }

        public MeshRenderer(Material material)
        {
            this.material = material;
        }

        public MeshRenderer(Material material, Mesh mesh)
        {
            this.material = material;
            this.mesh = mesh;
        }

        private int vao;
        private int vbo;
        private int ebo;

        private int nbo;
        private int uvbo;
        private int tbo;

        private Mesh? _mesh;  // backing field
        public Mesh? mesh
        {
            get => _mesh;
            set => SetMesh(value!);
        }

        public void SetMesh(Mesh mesh)
        {
            Upload(mesh);
            _mesh = mesh;
        }

        private int indexCount;

        public void Upload(Mesh mesh)
        {
            _mesh = mesh;
            // Delete old stuff
            if (vao != 0) GL.DeleteVertexArray(vao);
            if (vbo != 0) GL.DeleteBuffer(vbo);
            if (ebo != 0) GL.DeleteBuffer(ebo);
            if (tbo != 0) GL.DeleteBuffer(tbo);

            vao = GL.GenVertexArray();
            GL.BindVertexArray(vao);

            vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, mesh.vertices!.Length * sizeof(float), mesh.vertices, BufferUsage.StaticDraw);

            ebo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, mesh.indices!.Length * sizeof(uint), mesh.indices, BufferUsage.StaticDraw);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            // Normals
            nbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, nbo);
            GL.BufferData(BufferTarget.ArrayBuffer, mesh.normals!.Length * sizeof(float), mesh.normals, BufferUsage.StaticDraw);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(1);

            // Uvs
            uvbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, uvbo);
            GL.BufferData(BufferTarget.ArrayBuffer, mesh.uvs!.Length * sizeof(float), mesh.uvs, BufferUsage.StaticDraw);
            GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0);
            GL.EnableVertexAttribArray(2);

            // Tangents
            tbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, tbo);
            GL.BufferData(BufferTarget.ArrayBuffer, mesh.tangents!.Length * sizeof(float), mesh.uvs, BufferUsage.StaticDraw);
            GL.VertexAttribPointer(3, 4, VertexAttribPointerType.Float, false, 4 * sizeof(float), 0);
            GL.EnableVertexAttribArray(3);

            GL.BindVertexArray(0);
            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Less);

            indexCount = mesh.indices.Length;
            totalTrisCount += indexCount / 3;
        }

        public static int totalTrisCount;

        public override void Render()
        {
            if (mesh == null)
            {
                Console.WriteLine("Mesh on renderer is null!");
                return;
            }
            material?.Use();

            GL.BindVertexArray(vao);
            GL.DrawElements(PrimitiveType.Triangles, indexCount, DrawElementsType.UnsignedInt, 0);
        }

        public void RenderOveride(ShaderProgram program)
        {
            program.Use();
            GL.BindVertexArray(vao);
            GL.DrawElements(PrimitiveType.Triangles, indexCount, DrawElementsType.UnsignedInt, 0);
        }

        public void Dispose()
        {
            if (vao != 0) GL.DeleteVertexArray(vao);
            if (vbo != 0) GL.DeleteBuffer(vbo);
            if (ebo != 0) GL.DeleteBuffer(ebo);
            if (nbo != 0) GL.DeleteBuffer(nbo);
            if (uvbo != 0) GL.DeleteBuffer(uvbo);
            if (tbo != 0) GL.DeleteBuffer(tbo);

            vao = 0;
            vbo = 0;
            ebo = 0;
            nbo = 0;
            uvbo = 0;
            tbo = 0;
        }
    }
}
