using Microsoft.VisualBasic.FileIO;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using Runtime.Component.Core;
using Runtime.Graphics.Shaders;
using Runtime.Objects;
using Runtime.Scenes;
using System.Numerics;
using System.Runtime.InteropServices;

namespace Runtime.Graphics
{
	public class ParticleSystem : Renderers.Renderer
	{
		~ParticleSystem()
		{
			if (null != storage_buffer)
			{
				compute?.DeleteBuffer(storage_buffer[0]);
				compute?.DeleteBuffer(storage_buffer[1]);
			}
			compute?.DeleteBuffer(element_buffer);
			compute?.DeleteBuffer(genesis_buffer);
			compute?.DeleteBuffer(atomic_buffer);
		}
		// vertexarray hold openGL buffer bindings
		int vertexArray = 0;

		// The number of slots for partile types
		int number_of_slots;

		// 
		bool[]? occupied_slots;

		public int AllocateParticleTypeSlot()
		{
			for (int cx = 0; cx < number_of_slots; cx++)
				if (!occupied_slots![cx])
				{
					occupied_slots[cx] = true;
					return cx;
				}
			return -1;
		}
		public void FreeParticleTypeSlot(int s)
		{
			if (s >= 0 && s < number_of_slots)
				occupied_slots![s] = false;
		}
		// shader program to draw the particles
		ShaderProgram? shader;

		// compute shader to step the particles forward in time
		ComputeShaderProgram? compute;
		int property_texture = 0;
		public override void Render()
		{
			int[] _viewport = new int[4];
			unsafe
			{
				fixed (int* viewport = _viewport)
				{
					GL.GetIntegerv(GetPName.Viewport, viewport);
				}
			}

			// activate our vertexArray
			GL.BindVertexArray(vertexArray);
			// Enables setting point sizes from a vertex program
			GL.Enable(EnableCap.ProgramPointSize);
			// And we want to blend the particles
			GL.Enable(EnableCap.Blend);
			GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

			shader?.Use();
			if (null != Camera.main)
			{
				shader?.SetMatrix4("uView", Camera.main.GetViewMatrix());
				shader?.SetMatrix4("uProjection", Camera.main.GetProjectionMatrix());
			}
			else
			{
				shader?.SetMatrix4("uView", Matrix4.Identity);
				shader?.SetMatrix4("uProjection", Matrix4.Identity);
			}
			Transform? transform = GetComponent<Transform>();
			if (null != transform)
			{
				shader?.SetMatrix4("uModel", transform.GetMatrix());
			}
			else
			{
				shader?.SetMatrix4("uModel", Matrix4.Identity);
			}
			GL.Uniform4i(1, _viewport[0], _viewport[1], _viewport[2], _viewport[3]);
			// Buffer of particle states
			GL.BindBuffer(BufferTarget.ArrayBuffer, storage_buffer![1-pingpong]);
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
			GL.Uniform1f(0, 0.5f / number_of_slots);
			// just be sure all calculations are done
			GL.MemoryBarrier(MemoryBarrierMask.ShaderStorageBarrierBit);

			GL.ActiveTexture(TextureUnit.Texture0);
			GL.BindTexture(TextureTarget.Texture2d, property_texture);
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
			public vec4 lifetime; // current, delta, texture offset, friction
			public vec4 padding;
		};

		// maximum number of particles
		uint count = 16 * 1024;

		// the state buffer pingpongs between two buffers, this 
		uint pingpong = 0;

		// names of the two buffers that hold the state
		int[]? storage_buffer = null;

		// name of the buffer that will hold the indices of the active particles
		int element_buffer = 0;

		// name of the buffer that holds the particles that are new and not in the active particle buffer yet
		int genesis_buffer = 0;

		// name of the buffer that will hold atomic counters
		int atomic_buffer = 0;

		// local copy of the counters, index 1 will hold the number of active particles (used to transfer to and from the GPU)
		uint[]? atomics_mirror = null;

		// Queue of particles to add to the system.
		System.Collections.Generic.Queue<state_t> queue = new Queue<state_t>();
		/// <summary>
		/// Add a particle to the queue, needs to be extended to be useful
		/// </summary>
		public void AddParticle(System.Numerics.Vector3 x, System.Numerics.Vector3 v, ParticleType type)
		{
			state_t item = new state_t();
			item.position = new vec4(x.X, x.Y, x.Z);
			item.prev_pos = new vec4(x.X - v.X / 100f, x.Y - v.Y / 100f, x.Z - v.Z / 100f);
			item.lifetime.x = 1;
			item.lifetime.y = 0.01f / type.GetLifetime();
			item.lifetime.z = (type.GetSlot() + 0.5f) / number_of_slots;
			item.lifetime.w = 1f-type.GetFriction();
			queue.Enqueue(item);
		}

		public override void OnLoad()
		{
			atomics_mirror = new uint[2];

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

			state_t[]? state_buffer = new state_t[count];
			for (int cx = 0; cx < count; cx++)
			{
				state_buffer[cx].lifetime.x = -1;
			}
			compute.SetComputeBufferData<state_t>(storage_buffer[0], 0, state_buffer);
			compute.SetComputeBufferData<state_t>(storage_buffer[1], 0, state_buffer);

			state_buffer = null;

			element_buffer = compute.GenerateComputeBuffer(count * sizeof(uint));
			genesis_buffer = compute.GenerateComputeBuffer(count * compute.SizeOf<state_t>());
			atomic_buffer = compute.GenerateAtomicBuffer(2);

			compute.BindComputeBuffer(genesis_buffer, 2);
			compute.BindComputeBuffer(element_buffer, 3);

			compute.BindAtomicBuffer(atomic_buffer, 0);

			// This texture can be used to set properties per particle type x lifetime

			number_of_slots = 1024;
			occupied_slots = new bool[number_of_slots];

			for (int cx = 0; cx < number_of_slots; cx++)
				occupied_slots[cx] = false;

			property_texture = GL.GenTexture();
			GL.BindTexture(TextureTarget.Texture2d, property_texture);
			GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
			GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureMinFilter, (int)TextureMagFilter.Nearest);
			compute.Check();
			GL.TexImage2D(TextureTarget.Texture2d, 0, InternalFormat.Rgba16f, 256, 2 * (int)number_of_slots, 0, PixelFormat.Rgba, PixelType.Byte, (nint)0);
			compute.Check();
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
			int steps = Math.Clamp((int)(0.5 + 100 * ((float)Runtime.Calc.Time.deltaTime + time_to_compensate)),0, 20);
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
				uint particles_to_add = Math.Min((uint)queue.Count, space_left); // Add a maximum of max_genesis_size particles per loop

				state_t[] genesis_data = new state_t[particles_to_add];				
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
		public void UpdateParticleType(ParticleType pt)
		{
			vec4[] buffer1 = new vec4[256];
			vec4[] buffer2 = new vec4[256];
			for (int cx = 0; cx < 256; cx++)
			{
				float a = (float)cx / 256;

				System.Numerics.Vector4 c = pt.GetColor(1f-a);
				float s = pt.GetSize(1f - a);
				buffer1[cx].x = c.X;
				buffer1[cx].y = c.Y;
				buffer1[cx].z = c.Z;
				buffer1[cx].w = c.W;

				buffer2[cx].x = s;
			}
			
			GL.TexSubImage2D(TextureTarget.Texture2d, 0, 0, 2 * pt.slot, 256, 1, PixelFormat.Rgba, PixelType.Float, buffer1);
			GL.TexSubImage2D(TextureTarget.Texture2d, 0, 0, 2 * pt.slot + 1, 256, 1, PixelFormat.Rgba, PixelType.Float, buffer2);
		}
	}

	/// <summary>
	/// 
	/// </summary>
	public abstract class ParticleType
	{
		public int slot = -1;
		public ParticleType() 
		{
			// Allocate a slot for the type
			int s = Scene.main.GetParticleSystem().AllocateParticleTypeSlot();
			if (s >= 0)
				slot = s;
			else
				Logging.Debug.Warning("Could not allocate slot for particle type");
		}
		~ParticleType()
		{
			// Deallocate the slot
			Scene.main.GetParticleSystem().FreeParticleTypeSlot(slot);
		}
		public float GetSlot()
		{
			return (float)slot;
		}

		public abstract System.Numerics.Vector4 GetColor(float age /* 0 - 1*/);
		public abstract float GetSize(float age);
		public abstract float GetLifetime();
		public virtual float GetFriction() {return 0.0f; }
	};

	/// <summary>
	/// Interface for the particle system
	/// </summary>
	public class ParticleEmitter: IComponent
	{
		public void AddParticle(System.Numerics.Vector3 position, System.Numerics.Vector3 velocity, ParticleType type)
		{
			ParticleSystem? sys = Scene.main.GetParticleSystem();

			if (null != sys)
			{
				sys.AddParticle(position, velocity, type);
			}

		}
	}
}