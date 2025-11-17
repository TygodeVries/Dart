using Runtime.Physics;
using Runtime.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Runtime.Component.Lighting;
using Runtime.Graphics;

namespace Runtime.Scenes
{
	public class Scene
	{
		List<GameObject> gameObjects = new List<GameObject>();
		public void Instantiate(GameObject game)
		{
			gameObjects.Add(game);
			game.OnLoad();
		}

		LightManager defaultLightManager = new LightManager();
		public LightManager GetLightManager()
		{
			return defaultLightManager;
		}

		// Implicitly make the main scene an empty scene
		public static Scene main = new Scene();

		public ParticleSystem? particleSystem = null;

		public ParticleSystem GetParticleSystem()
		{
			if (null == particleSystem)
			{
				particleSystem = new ParticleSystem();
				particleSystem.OnLoad();
			}
			return particleSystem;
		}
		public PhysicsSolver physicsSolver = new PhysicsSolver();
		public Scene()
		{
		}

		public void Update()
		{
			if (null != particleSystem)
			{
				particleSystem.Update();
			}
			foreach (GameObject obj in gameObjects)
			{
				obj.Update();
			}
		}
	}
}
