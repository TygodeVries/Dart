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

        DefaultLightManager defaultLightManager = new DefaultLightManager();
        public DefaultLightManager GetLightManager()
        {
            return defaultLightManager;
        }

        public static Scene main;

        public PhysicsSolver physicsSolver = new PhysicsSolver();
        public Scene()
        {
            if (main == null)
                main = this;
        }

        public void Update()
        {
            foreach (GameObject obj in gameObjects)
            {
                obj.Update();
            }
        }
    }
}
