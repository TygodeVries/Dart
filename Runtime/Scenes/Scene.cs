using Runtime.Graphics;
using Runtime.Graphics.Pipeline;
using Runtime.Objects;
using Runtime.Physics;

namespace Runtime.Scenes
{
    public class Scene
    {
        public static void Load(Scene scene)
        {
            if (Scene.main != null)
                Scene.main?.Unload();

            Scene.main = scene;
        }

        public void Unload()
        {
            // #Todo make this better
            if (RenderCanvas.main != null)
            {
                if (RenderCanvas.main!.GetGraphicsPipeline() is DefaultGraphicsPipeline defaultGraphics)
                {
                    defaultGraphics.ClearRenderers();
                }
            }

            foreach (GameObject gameObject in gameObjects)
            {
                gameObject.Unload();
            }

            gameObjects.Clear();
        }
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
        public static Scene main { get; private set; } = new Scene();

        public PhysicsSolver physicsSolver = new PhysicsSolver();

        List<IManager> managers = new List<IManager>();
        List<GameObject> gameObjects = new List<GameObject>();


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
