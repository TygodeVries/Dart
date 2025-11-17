
using Runtime;
using Runtime.Component.Core;
using Runtime.DearImGUI.Gui;
using Runtime.Objects;
using Runtime.Graphics;
using System.Runtime.CompilerServices;
using Runtime.Scenes;
using System.Numerics;

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
			GuiWindow.Enable(new AudioTestWindow());
			GuiWindow.Enable(new GUIPerformanceWindow());

			Runtime.Scenes.Scene.main.Instantiate(
				new GameObjectFactory().AddComponent<Camera>().Build());

			Runtime.Scenes.Scene.main.Instantiate(
				new GameObjectFactory()
					.AddComponent<ParticleEmitter>()
					.AddComponent<ParticleTest>()
					.Build()
			);
		}
	}
	class ParticleTest: IComponent
	{
		float next_particle = 0;
		ParticleType? pt;
		public override void OnLoad()
		{
			pt = new ParticleType();
			Scene.main.GetParticleSystem().UpdateParticleType(pt);
		}
		double t = 0;
		public override void Update()
		{
			t += Runtime.Calc.Time.deltaTime;
			ParticleSystem? sys = Scene.main.GetParticleSystem();
			next_particle -= (float)Runtime.Calc.Time.deltaTime;
			if (next_particle < 0 && null != sys)
			{
				uint nparts = sys.GetActiveParticles();
				if (nparts < 1000)
					GetComponent<ParticleEmitter>()?.AddParticle(
						new Vector3(0,0,0),
						new Vector3(0.1f * MathF.Cos((float)t),0.1f * MathF.Sin((float)t),0),
						pt!
						);
				next_particle += 0.010f;
			}
		}
	}
}

