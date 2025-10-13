using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runtime.Objects
{
    public class GameObject
    {
        private Dictionary<Type, IComponent> componentMap = new();
        private List<IComponent> components = new();

        public T GetComponent<T>() where T : IComponent
        {
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

        public void Update()
        {
            foreach (IComponent component in components)
            {
                component.Update();
            }
        }
    }
}
