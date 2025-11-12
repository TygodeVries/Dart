using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Runtime.Component.Physics;
using Runtime.Graphics.Pipeline;
using Runtime.Graphics.Shaders;
using Runtime.Logging;
using Runtime.Objects;
using System.Net.NetworkInformation;
using System.Numerics;
using System.Runtime.InteropServices;
using static Runtime.Logging.Debug;

namespace FeatureTestProject
{
	public class Box2D : IComponent
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
	public class Box2DCollider : IComponent
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
		ComputeShaderProgram? compute;
		float angle = 0;
		public override void Render()
		{
			GL.BindVertexArray(vertexArray);
			GL.PointSize(5);
			shader.Use();
			GL.BindBuffer(BufferTarget.ArrayBuffer, storage_buffer[1-pingpong]);
			compute.Check();
			GL.BindBuffer(BufferTarget.ElementArrayBuffer, element_buffer);
			compute.Check();
			GL.EnableVertexAttribArray(0);
			compute.Check();
			GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, true, (int)compute.SizeOf<state_t>(), 0);
			compute.Check();
			GL.MemoryBarrier(MemoryBarrierMask.ShaderStorageBarrierBit);

			GL.DrawElements(PrimitiveType.Points, (int)atomics_mirror[1], DrawElementsType.UnsignedInt, 0);
			compute.Check();
			GL.BindVertexArray(0);

			/*
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
			*/
		}
		[StructLayout(LayoutKind.Explicit, Pack = 1)]
		struct vec3
		{
			public vec3() { }
			public vec3(float x, float y, float z)
			{
				this.x = x;
				this.y = y;
				this.z = z;
			}
			[FieldOffset(0)]
			public float x;
			[FieldOffset(4)]
			public float y;
			[FieldOffset(8)]
			public float z;
			[FieldOffset(12)]
			public float dummy;
		};
		[StructLayout(LayoutKind.Explicit, Pack = 1)]
		struct state_t
		{
			[FieldOffset(0)]
			public vec3 position;
			[FieldOffset(16)]
			public vec3 prev_pos;
			[FieldOffset(32)]
			public vec3 lifetime;
			[FieldOffset(48)]
			public vec3 padding;
		};
		uint count = 16 * 1024;
		uint pingpong = 0;
		int[] storage_buffer;
		int element_buffer;
		int genesis_buffer;
		int atomic_buffer;
		uint[] atomics_mirror;
		state_t[] genesis_data;
		public override void OnLoad()
		{
			
			vertexArray = GL.CreateVertexArray();
			GL.BindVertexArray(vertexArray);
			shader = ShaderProgram.FromFile("assets/shaders/fixed.vert", "assets/shaders/fixed.frag");

			/*
			vertexBuffer = GL.CreateBuffer();
			GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBuffer);
			GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsage.StaticDraw);
			GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);
			GL.BindVertexArray(0);
			*/
			Runtime.Graphics.RenderCanvas.main?.GetGraphicsPipeline()?.AddRenderer(this);

			compute = ComputeShaderProgram.FromFile("assets/shaders/particle_step.compute");
			compute.Compile();

			storage_buffer = new int[2]
				{
					compute.GenerateComputeBuffer(count * compute.SizeOf<state_t>()),
					compute.GenerateComputeBuffer(count * compute.SizeOf<state_t>())};
			element_buffer = compute.GenerateComputeBuffer(count * sizeof(uint));
			genesis_buffer = compute.GenerateComputeBuffer(16 * compute.SizeOf<state_t>());
			atomic_buffer = compute.GenerateAtomicBuffer(2);

			compute.BindComputeBuffer(genesis_buffer, 2);
			compute.BindComputeBuffer(element_buffer, 3);

			compute.BindAtomicBuffer(atomic_buffer, 0);
			atomics_mirror = new uint[2];
			genesis_data = new state_t[16];
		}
		float t = 0;
		public override void Update()
		{
			t += (float)Runtime.Calc.Time.deltaTime;
			int last_va = GL.GetInteger(GetPName.VertexArray);
			GL.BindVertexArray(vertexArray);
			compute.Use();
			for (int cx = 0; cx < 16; cx++)
			{
				state_t tmp = new state_t();
				float r = (2 + MathF.Cos(100 * t)) / 10000f;
				tmp.lifetime.x = 10;
				tmp.position = new vec3(MathF.Cos(t)*r, MathF.Sin(t)*r,0);
				tmp.prev_pos = new vec3(0,0,0);
				genesis_data[cx] = tmp;
			}

			GL.Uniform1i(0, 16);
			compute.BindComputeBuffer(storage_buffer[pingpong], 0);
			compute.BindComputeBuffer(storage_buffer[1-pingpong], 1);

			compute.SetComputeBufferData(genesis_buffer, 0, genesis_data);
			atomics_mirror[0] = 0;
			atomics_mirror[1] = 0;
			compute.SetAtomicBufferData(atomic_buffer, 0, atomics_mirror);

			compute.Dispatch(count);
			compute.ReadAtomicBufferData(atomic_buffer, 0, atomics_mirror);


			Debug.Log($"Active: {atomics_mirror[1]} genesis: {atomics_mirror[0]}");
			pingpong = 1 - pingpong;
			GL.BindVertexArray(last_va);
		}
	}
}