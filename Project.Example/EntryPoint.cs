
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
			Runtime.Scenes.Scene.main.Instantiate(emitter = 
				new GameObjectFactory()
					.AddComponent<ParticleEmitter>()
					.AddComponent<ParticleTest>()
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
			Scene.main.GetParticleSystem().UpdateParticleType(pt);
		}
		int num = 0;
		public override void Update()
		{
			if (num>0)
			{
				float t = (float)num / 100 - 0.5f;
				Vector3 v = new Vector3(1.25f, Sin(8f*t)/3, 0);
				GetComponent<ParticleEmitter>()?.AddParticle(
					new Vector3(0, 0, -10),
					v,
					pt!
					);
				num--;
			}
		}
		public void Fire()
		{
			num = 100;
		}
	}
}

