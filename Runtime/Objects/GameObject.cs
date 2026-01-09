using Runtime.Calc;
using Runtime.Data;
using Runtime.Logging;
using Runtime.Objects.Prefabs;

namespace Runtime.Objects
{
    [AssetReference(new string[] { ".prefab" }, nameof(LoadFromFile))]
    public class GameObject : AssetReference
    {

        public bool HasComponent(Component component)
        {
            foreach (Component c in components)
            {
                if (c == component)
                    return true;
            }

            return false;
        }
        public static GameObject? LoadFromFile(Asset asset)
        {
            GameObject? gameObject = PrefabGameObject.FromFile(asset)?.GetGameObject();
            return gameObject;
        }

        private Dictionary<Type, Component> componentMap = new();
        private List<Component> components = new();


        /// <summary>
        /// Calls unload for all the components, does not remove object from scene.
        /// Use Scene.Destroy() instead.
        /// </summary>
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
            MainThread.Run(() =>
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
            });
        }


        public List<Component> GetComponents()
        {
            return new List<Component>(components);
        }

        public T? GetComponent<T>() where T : Component
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

        public void AddComponent(Component component)
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
                component.Load();
        }

        public bool IsActive()
        {
            return hasBeenLoaded;
        }
        bool hasBeenLoaded = false;
        public void OnLoad()
        {
            hasBeenLoaded = true;
            foreach (Component component in components)
            {
                component.Load();
            }
        }

        public bool enableUpdates = true;
        public void Update()
        {


            foreach (Component component in components)
            {
                if (!enableUpdates && !component.AlwaysUpdate)
                {
                    continue;
                }

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
