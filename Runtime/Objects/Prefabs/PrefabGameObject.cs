namespace Runtime.Objects.Prefabs
{
    public class PrefabGameObject
    {
        public List<PrefabComponent> components { get; set; }

        public GameObjectFactory GetGameObjectAsFactory()
        {
            GameObjectFactory factory = new GameObjectFactory();
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
