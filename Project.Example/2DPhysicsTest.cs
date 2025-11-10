using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Graphics.OpenGL;
using Runtime.Graphics.Pipeline;
using Runtime.Objects;
using static Runtime.Logging.Debug;
using Runtime.Logging;
using Runtime.Graphics.Shaders;

using Runtime.Component.Physics;

using System.Numerics;

namespace FeatureTestProject
{
	public class Box2D: IComponent
	{
		public Vector2 position;
		public Vector2 size;
		public float angle;
		public Vector3 color;
		public override void OnLoad()
		{
			position = new Vector2(0, 0);
			size = new Vector2(0.2f, 0.3f);
			color = new Vector3(1, 0.5f, 1);
			angle = 0;
		}
		public override void Update()
		{
		}
	}
	public class Box2DRigidBody : IComponent
	{
		public Box2D? box;
		public Vector2 linear_velocity;
		public float angular_velocity;
		public override void OnLoad()
		{
			linear_velocity = new Vector2(0, 0.1f);
			angular_velocity = 0;
			box = GetComponent<Box2D>();
		}
		public override void Update()
		{
			if (null != box)
			{
				box.position += (float)Runtime.Calc.Time.deltaTime * linear_velocity;
				box.angle += (float)Runtime.Calc.Time.deltaTime * angular_velocity;
			}
		}
	}
	public class Box2DCollider: IComponent
	{
	}
	public class Box2DRenderer : Runtime.Graphics.Renderers.Renderer
	{
		int vertexArray;
		int vertexBuffer;

		float[] vertices =
{
				-1f,-1f,0,
				1f,-1f,0,
				-1f,1f,0,
				1f,1f,0
			};
		ShaderProgram? shader;
		float angle = 0;
		public override void Render()
		{
			angle += 0.1f;
			shader?.Use();
			Box2D? box = GetComponent<Box2D>();
			if (box == null)
			{
				Error("Box renderer without box");
				return;
			}
			GL.BindVertexArray(vertexArray);
			GL.EnableVertexAttribArray(0);
			GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBuffer);
			GL.VertexAttrib3f(1, in box.color); // Color
			GL.VertexAttrib1f(2, in box.angle); // Angle
			GL.VertexAttrib2f(3, in box.size); // Size
			GL.VertexAttrib2f(4, in box.position); // Position
			GL.DrawArrays(PrimitiveType.TriangleStrip, 0, 4);
			GL.DisableVertexAttribArray(0);
		}
		public override void OnLoad()
		{
			vertexArray = GL.CreateVertexArray();
			GL.BindVertexArray(vertexArray);
			vertexBuffer = GL.CreateBuffer();
			GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBuffer);
			GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsage.StaticDraw);
			GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);
			GL.BindVertexArray(0);
			shader = ShaderProgram.FromFile("assets/shaders/fixed.vert", "assets/shaders/fixed.frag");
			Runtime.Graphics.RenderCanvas.main?.GetGraphicsPipeline()?.AddRenderer(this);
		}
		public override void Update()
		{
		}
	}
}