
using Runtime;
using Runtime.Component.Core;
using Runtime.DearImGUI.Gui;
using Runtime.Objects;
using Runtime.Graphics;
using System.Runtime.CompilerServices;
using Runtime.Scenes;
using System.Numerics;

using static System.MathF;
using Project.Example.Windows;
using Runtime.Component.Test;

namespace FeatureTestProject
{
	[Runtime.Plugins.DartEntryPoint("Main")]
	public class EntryPoint
	{
	
		static EntryPoint()
		{
		}
		public static void Main()
		{

			GameObject emitter;

			Runtime.Scenes.Scene.main.AddManager(new ParticleSystemManager());

			Runtime.Scenes.Scene.main.Instantiate(emitter = 
				new GameObjectFactory()
					.AddComponent<ParticleEmitter>()
					.AddComponent<ParticleTest>()
					.AddComponent<TestCameraControls>()
					.AddComponent<Transform>()
					.Build()
			);

			GuiWindow.Enable(new AudioTestWindow());
			GuiWindow.Enable(new GUIPerformanceWindow());
			GuiWindow.Enable(new FireWindow(emitter));
			Runtime.Scenes.Scene.main.Instantiate(
				new GameObjectFactory().AddComponent<Camera>().Build());
		}
	}
	class ParticleTest: IComponent
	{
		class MyParticleType : ParticleType
		{
			public override Vector4 GetColor(float age)
			{
				float a = PI * age;
				return new Vector4(Cos(a), -Cos(a), Sin(a), Sin(a) / (1+a));
			}

			public override float GetLifetime()
			{
				return 4;
			}

			public override float GetSize(float age)
			{
				float a = PI * age;
				return (1 - age) * Sin(a) * .3f;
			}
			public override float GetFriction()
			{
				return 0.01f;
			}
		};
		MyParticleType? pt;
		public override void OnLoad()
		{
			pt = new MyParticleType();
			Scene.main.GetManager<ParticleSystemManager>()?.UpdateParticleType(pt);
			Transform? tr = GetComponent<Transform>();
			tr.position.Z = -10;
		}
		int num = 0;
		double t = 1;
		System.Numerics.Vector3 TransVec(OpenTK.Mathematics.Vector3 inv)
		{
			return new System.Numerics.Vector3(inv.X, inv.Y, inv.Z);
		}
		public override void Update()
		{
			t -= Runtime.Calc.Time.deltaTime;
			if (t < 0)
			{
				num += 1;
				t = 1;
			}
			if (num>0)
			{
				float t = (float)num / 100 - 0.5f;
				Transform? tr = GetComponent<Transform>();
				if (null != tr)
				{
					Vector3 v = new Vector3(0, 0, 1.25f);

					
					OpenTK.Mathematics.Matrix4 m = tr.GetMatrix();
					OpenTK.Mathematics.Vector4 otk_v = new OpenTK.Mathematics.Vector4(v.X, v.Y, v.Z, 0);
					OpenTK.Mathematics.Vector4 otk_vv = m * otk_v;
					v = TransVec(otk_vv.Xyz);
					GetComponent<ParticleEmitter>()?.AddParticle(
						TransVec(tr.position),
						v,
						pt!
						);
				}
				num--;
			}
		}
		public void Fire()
		{
			num = 100;
		}
	}
}

