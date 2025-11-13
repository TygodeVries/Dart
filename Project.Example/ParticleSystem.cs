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
using System.Security.Cryptography.X509Certificates;
using static Runtime.Logging.Debug;

namespace FeatureTestProject
{
	public class ParticleSystem : Runtime.Graphics.Renderers.Renderer
	{
		int vertexArray;
		ShaderProgram? shader;
		ComputeShaderProgram? compute;
		public override void Render()
		{
			GL.BindVertexArray(vertexArray);
			GL.Enable(EnableCap.ProgramPointSize);
			GL.Enable(EnableCap.Blend);
			
			GL.BlendFunc(BlendingFactor.OneMinusSrcAlpha, BlendingFactor.SrcAlpha);
			GL.PointSize(5);
			shader.Use();
			GL.BindBuffer(BufferTarget.ArrayBuffer, storage_buffer[1-pingpong]);
			compute.Check();
			GL.BindBuffer(BufferTarget.ElementArrayBuffer, element_buffer);
			compute.Check();
			GL.EnableVertexAttribArray(0);
			GL.EnableVertexAttribArray(1);
			compute.Check();
			GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, true, (int)compute.SizeOf<state_t>(), 0);
			GL.VertexAttribPointer(1, 1, VertexAttribPointerType.Float, true, (int)compute.SizeOf<state_t>(), 32);
			compute.Check();
			GL.MemoryBarrier(MemoryBarrierMask.ShaderStorageBarrierBit);

			GL.DrawElements(PrimitiveType.Points, (int)atomics_mirror[1], DrawElementsType.UnsignedInt, 0);
			compute.Check();
			GL.BindVertexArray(0);
		}
		[StructLayout(LayoutKind.Sequential, Pack = 4)]
		struct vec4
		{
			public vec4() { }
			public vec4(float x, float y, float z, float w)
			{
				this.x = x;
				this.y = y;
				this.z = z;
				this.w = w;
			}
			public vec4(float x, float y, float z)
			{
				this.x = x;
				this.y = y;
				this.z = z;
				this.w = 1;
			}
			public vec4(float x, float y)
			{
				this.x = x;
				this.y = y;
				this.z = 0;
				this.w = 1;
			}
			public vec4(float x)
			{
				this.x = x;
				this.y = this.z = 0;
				this.w = 1;
			}
			public float x;
			public float y;
			public float z;
			public float w;
		};
		[StructLayout(LayoutKind.Sequential, Pack = 16)]
		struct state_t
		{
			public vec4 position;
			public vec4 prev_pos;
			public vec4 lifetime;
			public vec4 padding;
		};
		uint count = 16 * 1024;
		uint pingpong = 0;
		int[] storage_buffer;
		int element_buffer;
		int genesis_buffer;
		int atomic_buffer;
		uint[] atomics_mirror;
		state_t[] genesis_data;

		System.Collections.Generic.Queue<state_t> queue = new Queue<state_t>();
		public void AddParticle()
		{
			float a = MathF.Sin(100 * t);
			float dx = MathF.Sin(a);
			float dy = MathF.Cos(a);
			state_t item = new state_t();
			item.position = new vec4(0, 0, 0);
			item.prev_pos = new vec4(-dx / 1000f, -dy / 1000f, 0);
			item.lifetime.x = 10;
			queue.Enqueue(item);
		}
		public override void OnLoad()
		{
			atomics_mirror = new uint[2];
			genesis_data = new state_t[16];

			vertexArray = GL.CreateVertexArray();
			GL.BindVertexArray(vertexArray);
			shader = ShaderProgram.FromFile("assets/shaders/fixed.vert", "assets/shaders/fixed.frag");

			Runtime.Graphics.RenderCanvas.main?.GetGraphicsPipeline()?.AddRenderer(this);

			compute = ComputeShaderProgram.FromFile("assets/shaders/particle_step.compute");
			compute.Use();

			storage_buffer = new int[2]
				{
					compute.GenerateComputeBuffer(count * compute.SizeOf<state_t>()),
					compute.GenerateComputeBuffer(count * compute.SizeOf<state_t>())
				};
			element_buffer = compute.GenerateComputeBuffer(count * sizeof(uint));
			genesis_buffer = compute.GenerateComputeBuffer(16 * compute.SizeOf<state_t>());
			atomic_buffer = compute.GenerateAtomicBuffer(2);

			compute.BindComputeBuffer(genesis_buffer, 2);
			compute.BindComputeBuffer(element_buffer, 3);

			compute.BindAtomicBuffer(atomic_buffer, 0);
		}
		float t = 0;
		public uint GetActiveParticles()
		{
			return atomics_mirror[1];
		}
		public override void Update()
		{
			t += (float)Runtime.Calc.Time.deltaTime;
			
			int steps = Math.Min(20, 1 + (int)(100 * Runtime.Calc.Time.deltaTime));
			int last_va = GL.GetInteger(GetPName.VertexArray);

			compute!.Use();
			GL.BindVertexArray(vertexArray);
			Debug.Log($"NSteps: {steps}");
			// Needs more balancing
			for (int step = 0; step < steps; step++)
			{
				uint space_left = count - GetActiveParticles();
				uint particles_to_add = Math.Min((uint)queue.Count, Math.Min(16, space_left));
				for (int cx = 0; cx < particles_to_add; cx++)
				{
					genesis_data[cx] = queue.Dequeue();
				}
				GL.Uniform1i(0, (int)particles_to_add);
				compute.BindComputeBuffer(storage_buffer[pingpong], 0);
				compute.BindComputeBuffer(storage_buffer[1 - pingpong], 1);

				compute.SetComputeBufferData(genesis_buffer, 0, genesis_data);
				atomics_mirror[0] = 0;
				atomics_mirror[1] = 0;
				compute.SetAtomicBufferData(atomic_buffer, 0, atomics_mirror);

				compute.Dispatch(count);
				compute.ReadAtomicBufferData(atomic_buffer, 0, atomics_mirror);

				pingpong = 1 - pingpong;
			}
			GL.BindVertexArray(last_va);
		}
	}
	public class ParticleEmitter: IComponent
	{
		public override void Update()
		{
			ParticleSystem sys = GetComponent<ParticleSystem>();
			uint nparts = sys!.GetActiveParticles();
			if (nparts < 1000)
				sys.AddParticle();
		}
	}
}