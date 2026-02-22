using Runtime.Calc;
using Runtime.Components.Core;
using Runtime.Data;
using Runtime.Graphics;
using Runtime.Graphics.Pipeline;
using Runtime.Graphics.Renderers;
using Runtime.Logging;
using Runtime.Objects;
using Runtime.Physics;
using System.Text.Json;

namespace Runtime.Scenes
{
    [AssetReference(new string[] { ".scene" }, nameof(LoadFromAsset))]
    public class Scene : AssetReference
    {
        private bool hasBeenLoaded = false;
        public static void Load(Scene scene)
        {
            if (UserCode.Unloading())
            {
                Debug.Warning("Can not load a scene while unloading user code.");
                Scene.main?.Unload();
                Scene.main = null;
                return;
            }

            if (Scene.main != null && Scene.main != scene)
                Scene.main?.Unload();

            Scene.main = scene;
            scene.Load();
        }

        public static void LoadDefault()
        {
            LoadFromAsset(Game.GetAssetDatabase().GetAsset(GameSettings.GetGameSettings()?.StartScene!));
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
            if (asset == null)
                return;

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
                    defaultGraphics.ClearCameras();
                }
            }

            foreach (GameObject gameObject in gameObjects)
            {
                gameObject.Unload();
            }

            hasBeenLoaded = false;
            gameObjects.Clear();
            Camera.main = null;
        }

        public void DestroyObject(GameObject gameObject)
        {
            MainThread.Run(() =>
            {
                gameObjects.Remove(gameObject);
            });

            gameObject?.Unload();
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
            {
                MainThread.Run(() =>
                {
                    game.OnLoad();
                }); // #TODO quick fix, since scene loading happends to an already loaded scene, loading orders did not work.
            }
        }



        private LightManager defaultLightManager = new LightManager();
        public LightManager GetLightManager()
        {
            return defaultLightManager;
        }

        // Implicitly make the main scene an empty scene
        public static Scene main { get; private set; } = new Scene();


        public void SetSkybox(SkyboxRenderer? skyboxRenderer)
        {
            this.skyboxRenderer = skyboxRenderer;
        }

        private SkyboxRenderer? skyboxRenderer = null;
        public SkyboxRenderer? GetSkybox() { return skyboxRenderer; }

        public PhysicsSolver physicsSolver = new PhysicsSolver();

        private List<IManager> managers = new List<IManager>();
        private List<GameObject> gameObjects = new List<GameObject>();
        public List<GameObject> GetGameObjects()
        {
            return gameObjects;
        }

        public void AddManager<T>(T manager) where T : IManager
        {
            managers.Add(manager);
            manager.Load();
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


        public T[] FindAllComponentOfType<T>() where T : Component
        {
            List<T> list = new List<T>();
            foreach (GameObject go in gameObjects)
            {
                T? cmp = go.GetComponent<T>();
                if (null != cmp)
                    list.Add(cmp);

            }
            return list.ToArray();
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


                for (int i = 0; i < gameObjects.Count; i++)
                {
                    GameObject obj = gameObjects[i];
                    obj.Update();
                }
            }
            else
            {
                Debug.Error("Main scene has not been loaded yet! I have no idea how this would be possible, nice.");
            }
        }


        public T? FindAnyComponentOfType<T>() where T : Component
        {
            foreach (GameObject gameObject in gameObjects)
            {
                foreach (Component component in gameObject.GetComponents())
                {
                    if (component is T t)
                    {
                        return t;
                    }
                }
            }

            return null;
        }

        public List<T>? FindAllComponentsOfType<T>() where T : Component
        {
            List<T> list = new List<T>();
            foreach (GameObject gameObject in gameObjects)
            {
                foreach (Component component in gameObject.GetComponents())
                {
                    if (component is T t)
                    {
                        list.Add(t);
                    }
                }
            }

            return list;
        }
    }
}
