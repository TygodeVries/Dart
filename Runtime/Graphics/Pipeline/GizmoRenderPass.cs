using OpenTK.Graphics.OpenGL;
using Runtime.Calc;
using Runtime.Components.Core;
using Runtime.Graphics.Shaders;

namespace Runtime.Graphics.Pipeline
{
    public class GizmoRenderPass : RenderPass
    {
        List<Vector4> rawLines = new List<Vector4>();
        List<Vector4> rawLinesColors = new List<Vector4>();
        List<Vector4> camLines = new List<Vector4>();
        List<Vector4> camLinesColors = new List<Vector4>();
        int vertexArrayObject, positionBufferObject, colorBufferObject;
        static GizmoRenderPass instance = new GizmoRenderPass();
        static public GizmoRenderPass GetInstance()
        {
            return instance;
        }
        public override void Start()
        {
            vertexArrayObject = GL.GenVertexArray();
            positionBufferObject = GL.GenBuffer();
            colorBufferObject = GL.GenBuffer();
            gizmoCamShader = new ShaderProgram(@"
#version 330 core
                
layout(location = 0) in vec4 aPosition;
layout(location = 1) in vec4 aColor;

uniform mat4 u_View;
uniform mat4 u_Projection;
out vec4 Color;

void main()
{
    gl_Position = u_Projection * u_View * aPosition;
    Color = aColor;
}", @"
#version 330 core

in vec4 Color;
out vec4 FragColor;

void main()
{
   FragColor = Color;
}");
            gizmoRawShader = new ShaderProgram(@"
#version 330 core
                
layout(location = 0) in vec4 aPosition;
layout(location = 1) in vec4 aColor;

out vec4 Color;

void main()
{
    gl_Position = aPosition;
    Color = aColor;
}", @"
#version 330 core

in vec4 Color;
out vec4 FragColor;

void main()
{
   FragColor = Color;
}");

            gizmoCamShader.Compile();
            gizmoRawShader.Compile();
        }
        ShaderProgram gizmoCamShader;
        ShaderProgram gizmoRawShader;
        public override void Pass()
        {
         GL.Disable(EnableCap.DepthTest);
            GL.BindVertexArray(vertexArrayObject);
            if (Camera.main != null)
            {
                // Set to camera background color   
                GL.ClearColor(Camera.main.backgroundColor.x, Camera.main.backgroundColor.y, Camera.main.backgroundColor.z, 1);
                Camera renderCamera = Camera.main;
                Matrix4 view = renderCamera.GetViewMatrix();
                Matrix4 projection = renderCamera.GetProjectionMatrix();
                gizmoCamShader.Use();

                gizmoCamShader.SetMatrix4("u_View", view);
                gizmoCamShader.SetMatrix4("u_Projection", projection);

                Vector4[] camLinesArray = camLines.ToArray();
                Vector4[] camColorsArray = camLinesColors.ToArray();
                if (0 < camLinesArray.Length)
                    unsafe
                    {
                        fixed (void* position = &camLinesArray[0])
                        fixed (void* colors = &camColorsArray[0])
                        {
                            GL.BindBuffer(BufferTarget.ArrayBuffer, positionBufferObject);
                            GL.BufferData(BufferTarget.ArrayBuffer, 4 * 4 * camLinesArray.Length, position, BufferUsage.StaticDraw);
                            GL.VertexAttribPointer(0, 4, VertexAttribPointerType.Float, false, 0, 0);
                            GL.BindBuffer(BufferTarget.ArrayBuffer, colorBufferObject);
                            GL.BufferData(BufferTarget.ArrayBuffer, 4 * 4 * camColorsArray.Length, colors, BufferUsage.StaticDraw);
                            GL.VertexAttribPointer(1, 4, VertexAttribPointerType.Float, false, 0, 0);

                            GL.EnableVertexAttribArray(0);
                            GL.EnableVertexAttribArray(1);

                            GL.DrawArrays(PrimitiveType.Lines, 0, camLinesArray.Length);
                        }
                    }
            }

            rawLines.Clear();
            rawLinesColors.Clear();
            camLines.Clear();
            camLinesColors.Clear();
        }
        public void AddLine(Vector4 a, Vector4 b, Vector4? c = null, Vector4? d = null)
        {
            if (null == c) c = Vector4.One;
            if (null == d) d = Vector4.One;
            camLines.Add(a);
            camLines.Add(b);
            camLinesColors.Add(c.Value);
            camLinesColors.Add(d.Value);
        }
      public void AddRawLine(Vector4 a, Vector4 b, Vector4? c = null, Vector4? d = null)
      {
			if (null == c) c = Vector4.One;
			if (null == d) d = Vector4.One;
			rawLines.Add(a);
			rawLines.Add(b);
			rawLinesColors.Add(c.Value);
			rawLinesColors.Add(d.Value);
		}
	}
}
