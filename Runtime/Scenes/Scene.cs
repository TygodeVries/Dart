using Runtime.Calc;
using Runtime.Component.Core;
using Runtime.Data;
using Runtime.Graphics;
using Runtime.Graphics.Pipeline;
using Runtime.Logging;
using Runtime.Objects;
using Runtime.Physics;
using System.Text.Json;

namespace Runtime.Scenes
{
    [AssetReference(new string[] { ".scene" }, nameof(LoadFromAsset))]
    public class Scene : AssetReference
    {
        bool hasBeenLoaded = false;
        public static void Load(Scene scene)
        {
            if (Scene.main != null && Scene.main != scene)
                Scene.main?.Unload();

            Scene.main = scene;
            scene.Load();
        }

        public static Scene? LoadFromAsset(Asset asset)
        {
            SceneFile? file = JsonSerializer.Deserialize<SceneFile>(File.ReadAllText(asset.GetSystemPath()));
            if (file == null)
            {
                Debug.Error("Could not load scene from file asset! file is null!");
                return null;
            }
            Debug.Log($"Loading scene from database: {asset.GetDatabase().GetFolder()}");
            Scene scene = new Scene();
            scene.SetAsset(asset);
            Scene.Load(scene);

            file.LoadSceneAssets(asset.GetDatabase(), scene);

            return scene;
        }

        public void Save()
        {
            Asset? asset = GetAsset();
            if (asset == null)
            {
                Debug.Error("Can not use Save() on a scene that was not loaded from a file in the first place. Use SaveToFile() instead.");
                return;
            }
            SaveToFile(asset);
        }

        public void SaveToFile(Asset asset)
        {
            SceneFile sceneFile = new SceneFile();
            foreach (GameObject gameObject in gameObjects)
            {
                if (!gameObject.IsActive())
                    continue;

                Asset? gameObjectAsset = gameObject.GetAsset();
                if (gameObjectAsset == null) // instance
                    continue;

                Transform? transform = gameObject.GetComponent<Transform>();

                if (transform != null)
                {
                    Vector3 position = transform.position;
                    Vector3 rotation = transform.rotation;

                    sceneFile.sceneObjects.Add(new SceneObject()
                    {
                        path = gameObjectAsset.GetPath(),
                        position = position.ToString(),
                        rotation = rotation.ToString()
                    });
                }
                else
                {
                    sceneFile.sceneObjects.Add(new SceneObject()
                    {
                        path = gameObjectAsset.GetPath()
                    });
                }
            }

            Debug.Log(asset.GetSystemPath());
            File.WriteAllText(asset.GetSystemPath(), JsonSerializer.Serialize(sceneFile));
        }

        public void Unload()
        {
            if (RenderCanvas.main != null)
            {
                if (RenderCanvas.main!.GetGraphicsPipeline() is DefaultGraphicsPipeline defaultGraphics)
                {
                    defaultGraphics.ClearRenderersOfScene(this);
                }
            }

            foreach (GameObject gameObject in gameObjects)
            {
                gameObject.Unload();
            }

            hasBeenLoaded = false;
        }

        public void DestroyObject(GameObject gameObject)
        {
            MainThread.Run(() =>
            {
                gameObjects.Remove(gameObject);
            });

            gameObject.Unload();
            Scene.main.Save();
        }

        private void Load()
        {
            if (hasBeenLoaded)
            {
                Debug.Warning("Object was already loaded!");
                return;
            }

            foreach (GameObject gameObject in gameObjects)
            {
                gameObject.OnLoad();
            }

            hasBeenLoaded = true;
        }

        public void Instantiate(GameObject game)
        {
            gameObjects.Add(game);

            if (hasBeenLoaded)
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
        public List<GameObject> GetGameObjects()
        {
            return gameObjects;
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

        public Scene()
        {
        }

        public void Update()
        {
            if (hasBeenLoaded)
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
            else
            {
                Debug.Error("Main scene has not been loaded yet! I have no idea how this would be possible, nice.");
            }
        }
    }
}
