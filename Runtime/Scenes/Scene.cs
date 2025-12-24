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
		List<IManager> managers = new List<IManager>();
		List<GameObject> gameObjects = new List<GameObject>();
		public void Instantiate(GameObject game)
		{
			gameObjects.Add(game);
			game.OnLoad();
		}
		public void AddManager<T>(T manager) where T : IManager
		{
			managers.Add(manager);
			manager.OnLoad();
		}
		public T? GetManager<T>() where T : IManager
		{
            foreach (IManager item in managers)
            {
                if (item.GetType() == typeof(T))
						return (T)item;
            }
				return default(T);
        }

		LightManager defaultLightManager = new LightManager();
		public LightManager GetLightManager()
		{
			return defaultLightManager;
		}

		// Implicitly make the main scene an empty scene
		public static Scene main = new Scene();

		public PhysicsSolver physicsSolver = new PhysicsSolver();
		public Scene()
		{
		}

		public void Update()
		{
			foreach (IManager manager in managers)
			{
				if (manager is IUpdatableManager updatable)
					updatable.Update();
			}
			foreach (GameObject obj in gameObjects)
			{
				obj.Update();
			}
		}
	}
}
