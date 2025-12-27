using Runtime.Data;
using System.Text.Json;

namespace Runtime.Objects.Prefabs
{
    public class PrefabGameObject
    {
        public List<PrefabComponent> components { get; set; }

        public GameObjectFactory GetGameObjectAsFactory()
        {
            GameObjectFactory factory = new GameObjectFactory(asset);
            foreach (PrefabComponent component in components)
            {
                factory.AddComponent(component.GetComponent());
            }
            return factory;
        }

        public GameObject GetGameObject()
        {
            return GetGameObjectAsFactory().Build();
        }

        public static PrefabGameObject FromFile(Asset asset)
        {
            if (asset == null)
                throw new NullReferenceException();
            PrefabGameObject prefab = FromJson(File.ReadAllText(asset.GetSystemPath()));
            prefab.asset = asset;
            return prefab;
        }

        public Asset? asset = null;

        private static PrefabGameObject FromJson(string json)
        {
            return JsonSerializer.Deserialize<PrefabGameObject>(json);
        }

        public string ToJson(bool pretty = false)
        {
            JsonSerializerOptions options = new JsonSerializerOptions();
            options.WriteIndented = pretty;

            return JsonSerializer.Serialize(this, options);
        }

        public static PrefabGameObject FromGameObject(GameObject gameObject)
        {
            List<IComponent> components = gameObject.GetComponents();
            PrefabGameObject prefab = new PrefabGameObject();
            prefab.components = new List<PrefabComponent>();
            foreach (IComponent component in components)
            {
                prefab.components.Add(PrefabComponent.FromComponent(component));
            }

            return prefab;
        }
    }
}
