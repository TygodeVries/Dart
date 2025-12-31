using Runtime.Data;
using Runtime.Logging;
using Runtime.Objects.Prefabs;

namespace Runtime.Objects
{
    [AssetReference(new string[] { ".prefab" }, nameof(LoadFromFile))]
    public class GameObject : AssetReference
    {

        public bool HasComponent(IComponent component)
        {
            foreach (IComponent c in components)
            {
                if (c == component)
                    return true;
            }

            return false;
        }
        public static GameObject LoadFromFile(Asset asset)
        {
            GameObject gameObject = PrefabGameObject.FromFile(asset).GetGameObject();
            return gameObject;
        }

        private Dictionary<Type, IComponent> componentMap = new();
        private List<IComponent> components = new();

        public void Unload()
        {
            foreach (var component in components)
            {
                component.Unload();
            }

            hasBeenLoaded = false;
        }

        public void RemoveComponent<T>()
        {
            RemoveComponent(typeof(T));
        }

        public void RemoveComponent(Type type)
        {
            components.RemoveAll(c =>
            {
                if (type.IsAssignableFrom(c.GetType()))
                {
                    c.Unload();
                    return true;
                }
                return false;
            });

            componentMap.Remove(type);
            Debug.Log($"Removed Component {type.Name}");
        }


        public List<IComponent> GetComponents()
        {
            return new List<IComponent>(components);
        }

        public T? GetComponent<T>() where T : IComponent
        {
            if (null == this)
                return null;

            // Try to get by exact type first
            if (componentMap.TryGetValue(typeof(T), out var exactMatch))
            {
                return exactMatch as T;
            }

            foreach (var component in components)
            {
                if (component is T match)
                {
                    return match;
                }
            }

            return null;
        }

        public void AddComponent(IComponent component)
        {
            var type = component.GetType();
            if (componentMap.ContainsKey(type))
            {
                Console.WriteLine($"There is already a {type} attached to this object.");
            }

            components.Add(component);
            componentMap[type] = component;
            component.gameObject = this;

            if (hasBeenLoaded)
                component.OnLoad();
        }


        bool hasBeenLoaded = false;
        public void OnLoad()
        {
            hasBeenLoaded = true;
            foreach (IComponent component in components)
            {
                component.OnLoad();
            }
        }

        public bool EnableUpdates = true;
        public void Update()
        {
            if (!EnableUpdates)
                return;

            foreach (IComponent component in components)
            {
                try
                {
                    component.Update();
                }
                catch (Exception e)
                {
                    Debug.Error($"Failed to update {component.GetType()}! " + e);
                }
            }
        }
    }
}
