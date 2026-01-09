using Runtime.Data;

namespace Runtime.Objects
{
    public class GameObjectFactory
    {
        GameObject gameObject;
        Asset? asset;
        public GameObjectFactory(Asset? asset = null)
        {
            gameObject = new GameObject();
            gameObject.SetAsset(asset);
            this.asset = asset;
        }
        public GameObjectFactory AddComponent(Component component)
        {
            gameObject.AddComponent(component);
            return this;
        }
        public GameObjectFactory AddComponent<T>() where T : Component, new()
        {
            gameObject.AddComponent(new T());
            return this;
        }

        public GameObject Build()
        {
            return gameObject;
        }
    }
}
