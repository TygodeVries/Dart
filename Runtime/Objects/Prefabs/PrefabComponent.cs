using Runtime.Data;
using Runtime.Logging;
using System.Reflection;

namespace Runtime.Objects.Prefabs
{
    public class PrefabComponent
    {
        public string type { get; set; }
        public List<ValueRecord> overrides { get; set; }

        public static PrefabComponent FromComponent(IComponent component)
        {
            PrefabComponent prefab = new PrefabComponent();
            prefab.type = component.GetType().AssemblyQualifiedName;
            prefab.overrides = new List<ValueRecord>();

            FieldInfo[] infos = component.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (FieldInfo info in infos)
            {
                if (!Attribute.IsDefined(info, typeof(InspectableAttribute)))
                    continue;

                object? value = info.GetValue(component);

                prefab.overrides.Add(new ValueRecord(info.Name, value));
            }

            PropertyInfo[] propertyInfos = component.GetType().GetProperties();
            foreach (PropertyInfo info in propertyInfos)
            {
                if (!Attribute.IsDefined(info, typeof(InspectableAttribute)))
                    continue;

                object? value = info.GetValue(component);

                prefab.overrides.Add(new ValueRecord(info.Name, value));
            }

            return prefab;
        }

        public IComponent GetComponent()
        {
            Type? typeInstance = Type.GetType(type);
            if (typeInstance == null)
            {
                Debug.Error($"Failed to create component of type {type}, the type was null");
            }

            IComponent comp = (IComponent)Activator.CreateInstance(typeInstance);

            PropertyInfo[] propertyInfos = comp.GetType().GetProperties();
            FieldInfo[] infos = comp.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (ValueRecord value in overrides)
            {
                FieldInfo? v = infos.FirstOrDefault((e) =>
                {
                    return e.Name == value.Name;
                }, null);

                if (v != null)
                {
                    v.SetValue(comp, value.GetValue());
                }
                else
                {
                    PropertyInfo? p = propertyInfos.FirstOrDefault((e) =>
                    {
                        return e.Name == value.Name;
                    }, null);

                    if (p != null)
                    {
                        p.SetValue(comp, value.GetValue());
                    }
                    else
                    {
                        Debug.Error($"Could not find field or property named {value.Name} on {comp.GetType()}");
                    }
                }
            }

            return comp;
        }
    }
}
