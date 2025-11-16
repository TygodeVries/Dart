using OpenTK.Graphics.OpenGL;
using Runtime.Graphics.Shaders;
using Runtime.Objects;
using System.Runtime.InteropServices;

namespace FeatureTestProject
{
	public class ParticleSystem : Runtime.Graphics.Renderers.Renderer
	{
		// vertexarray hold openGL buffer bindings
		int vertexArray;

		// shader program to draw the particles
		ShaderProgram? shader;

		// compute shader to step the particles forward in time
		ComputeShaderProgram? compute;
		public override void Render()
		{
			// activate our vertexArray
			GL.BindVertexArray(vertexArray);
			// Enables setting point sizes from a vertex program
			GL.Enable(EnableCap.ProgramPointSize);
			// And we want to blend the particles
			GL.Enable(EnableCap.Blend);
			GL.BlendFunc(BlendingFactor.OneMinusSrcAlpha, BlendingFactor.SrcAlpha);

			shader.Use();
			// Buffer of particle states
			GL.BindBuffer(BufferTarget.ArrayBuffer, storage_buffer[1-pingpong]);
			compute.Check();
			// Buffer for the particle indices
			GL.BindBuffer(BufferTarget.ElementArrayBuffer, element_buffer);
			compute.Check();
			// Enable attribute arrays
			GL.EnableVertexAttribArray(0);
			GL.EnableVertexAttribArray(1);
			compute.Check();
			
			// attributes use same buffer, but different offsets
			GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, true, (int)compute.SizeOf<state_t>(), 0);
			GL.VertexAttribPointer(1, 1, VertexAttribPointerType.Float, true, (int)compute.SizeOf<state_t>(), 32);
			compute.Check();

			// just be sure all calculations are done
			GL.MemoryBarrier(MemoryBarrierMask.ShaderStorageBarrierBit);

			// and go for it! Draw the stuff
			GL.DrawElements(PrimitiveType.Points, (int)atomics_mirror[1], DrawElementsType.UnsignedInt, 0);
			compute.Check();
			GL.BindVertexArray(0);
		}
		// vec4 like in the shader
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
		// state of a particle, must consist of vec4's
		[StructLayout(LayoutKind.Sequential, Pack = 16)]
		struct state_t
		{
			public vec4 position;
			public vec4 prev_pos;
			public vec4 lifetime;
			public vec4 padding;
		};

		// maximum number of particles
		uint count = 16 * 1024;

		// the state buffer pingpongs between two buffers, this 
		uint pingpong = 0;

		// names of the two buffers that hold the state
		int[] storage_buffer;

		// name of the buffer that will hold the indices of the active particles
		int element_buffer;

		// name of the buffer that holds the particles that are new and not in the active particle buffer yet
		int genesis_buffer;

		// name of the buffer that will hold atomic counters
		int atomic_buffer;

		// local copy of the counters, index 1 will hold the number of active particles (used to transfer to and from the GPU)
		uint[] atomics_mirror;

		// local copy of the particles that still need to be added (used to transfer to the gpu)
		state_t[] genesis_data;

		// Queue of particles to add to the system.
		System.Collections.Generic.Queue<state_t> queue = new Queue<state_t>();
		/// <summary>
		/// Add a particle to the queue, needs to be extended to be useful
		/// </summary>
		public void AddParticle()
		{
			// This needs to change...
			float a = MathF.Sin(100 * t) / 3f;
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

			// create and use the vertexarray
			vertexArray = GL.CreateVertexArray();
			GL.BindVertexArray(vertexArray);

			// load some shaders
			shader = ShaderProgram.FromFile("assets/shaders/fixed.vert", "assets/shaders/fixed.frag");

			Runtime.Graphics.RenderCanvas.main?.GetGraphicsPipeline()?.AddRenderer(this);

			// load compute shader
			compute = ComputeShaderProgram.FromFile("assets/shaders/particle_step.compute");
			compute.Use();
			// allocate some buffers
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
		/// <summary>
		/// Get the number of active particles on the GPU (last loop)
		/// </summary>
		/// <returns>number of active particles</returns>
		public uint GetActiveParticles()
		{
			return atomics_mirror[1];
		}
		// Since the particle simulation uses fixed timestep, some compensation needs to be done
		float time_to_compensate = 0;
		public override void Update()
		{
			t += (float)Runtime.Calc.Time.deltaTime;
			// update the time, used for generateing a particle for now

			// Number of steps to take (one step is 10 ms)
			int steps = Math.Clamp((int)(100 * ((float)Runtime.Calc.Time.deltaTime + time_to_compensate)),0, 20);
			// How many time is going to be simulator
			float simulated_time = (float)steps / 100f;
			// Time to compensate in the next loop
			time_to_compensate += (float)Runtime.Calc.Time.deltaTime - simulated_time; 

			// save the current vertexarray, to set it back later
			int last_va = GL.GetInteger(GetPName.VertexArray);

			// And use our own
			GL.BindVertexArray(vertexArray);

			compute!.Use();

			for (int step = 0; step < steps; step++)
			{
				uint space_left = count - GetActiveParticles(); // space left in the GPU particle buffer
				uint particles_to_add = Math.Min((uint)queue.Count, Math.Min(16, space_left)); // Add a maximum of 16 particles per loop
				
				// queue some data for new particles to send to the GPU
				for (int cx = 0; cx < particles_to_add; cx++)
				{
					genesis_data[cx] = queue.Dequeue();
				}

				GL.Uniform1i(0, (int)particles_to_add); // Tell the shader how many new particles to add
				
				// bind the output and input buffers
				compute.BindComputeBuffer(storage_buffer[pingpong], 0);
				compute.BindComputeBuffer(storage_buffer[1 - pingpong], 1);

				// actually send the data to the GPU
				compute.SetComputeBufferData(genesis_buffer, 0, genesis_data);

				// set the atomic counters to zero
				atomics_mirror[0] = 0;
				atomics_mirror[1] = 0;
				// and send to the GPU
				compute.SetAtomicBufferData(atomic_buffer, 0, atomics_mirror);

				// run the shader
				compute.Dispatch(count);
				// ping-pong the in- and output buffers
				pingpong = 1 - pingpong;

				// read the atomic counters from the GPU
				compute.ReadAtomicBufferData(atomic_buffer, 0, atomics_mirror);
			}
			GL.BindVertexArray(last_va);
		}
	}

	/// <summary>
	/// Generatate a particle every 100 ms, just for testing
	/// </summary>
	public class ParticleEmitter: IComponent
	{
		float next_particle = 0;

		public override void Update()
		{
			ParticleSystem sys = GetComponent<ParticleSystem>();
			next_particle -= (float)Runtime.Calc.Time.deltaTime;
			if (next_particle < 0)
			{
				uint nparts = sys!.GetActiveParticles();
				if (nparts < 1000)
					sys.AddParticle();
				next_particle += 0.100f;
			}
		}
	}
}