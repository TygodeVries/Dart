using Runtime.Logging;

namespace Runtime.Objects
{
    public class GameObject
    {
        private Dictionary<Type, IComponent> componentMap = new();
        private List<IComponent> components = new();

        public void Unload()
        {
            foreach (var component in components)
            {
                component.Unload();
            }
        }

        public void RemoveComponent<T>()
        {
            componentMap.Remove(typeof(T));

            components.RemoveAll((c) =>
            {
                return c.GetType() == typeof(T);
            });
        }

        public void RemoveComponent(Type type)
        {
            componentMap.Remove(type);

            components.RemoveAll((c) =>
            {
                return c.GetType() == type;
            });

            Debug.Log("Removed Component!");
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
        }

        public void OnLoad()
        {
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
