using Runtime.Data;
using Runtime.Logging;
using System.Reflection;

namespace Runtime.Objects.Prefabs
{
    public class PrefabComponent
    {
        public string type { get; set; }
        public List<ValueRecord> overrides { get; set; }

        public static PrefabComponent FromComponent(Component component)
        {
            PrefabComponent prefab = new PrefabComponent();
            prefab.type = component.GetType().AssemblyQualifiedName;
            prefab.overrides = new List<ValueRecord>();

            FieldInfo[] infos = component.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
            foreach (FieldInfo info in infos)
            {
                if (!Attribute.IsDefined(info, typeof(InspectableAttribute)))
                    continue;

                object? value = info.GetValue(component);

                prefab.overrides.Add(new ValueRecord(info.Name, value));
            }

            PropertyInfo[] propertyInfos = component.GetType().GetProperties(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
            foreach (PropertyInfo info in propertyInfos)
            {
                if (!Attribute.IsDefined(info, typeof(InspectableAttribute)))
                    continue;

                object? value = info.GetValue(component);

                prefab.overrides.Add(new ValueRecord(info.Name, value));
            }

            return prefab;
        }

        public Component? GetComponent()
        {
            Type? typeInstance = UserCode.GetTypeOf(type);
            if (typeInstance == null)
            {
                Debug.Error($"Failed to create component of type {type}, the type was null");
                return null;
            }

            try
            {
                Component comp = (Component)Activator.CreateInstance(typeInstance);
                ObjectTracker.Track(comp);

                PropertyInfo[] propertyInfos = comp.GetType().GetProperties();
                FieldInfo[] infos = comp.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
                foreach (ValueRecord valueRecord in overrides)
                {
                    object? val = valueRecord.GetValue();

                    FieldInfo? field = infos.FirstOrDefault(f => f.Name == valueRecord.Name);
                    if (field != null)
                    {
                        Type fieldType = field.FieldType;

                        // Handle nulls for value types
                        if (val == null && fieldType.IsValueType && Nullable.GetUnderlyingType(fieldType) == null)
                        {
                            val = Activator.CreateInstance(fieldType);
                        }

                        field.SetValue(comp, val);
                        continue;
                    }

                    PropertyInfo? prop = propertyInfos.FirstOrDefault(p => p.Name == valueRecord.Name);
                    if (prop != null)
                    {
                        Type propType = prop.PropertyType;

                        if (val == null && propType.IsValueType && Nullable.GetUnderlyingType(propType) == null)
                        {
                            val = Activator.CreateInstance(propType);
                        }

                        if (prop.CanWrite)
                        {
                            prop.SetValue(comp, val);
                        }
                        else
                        {
                            Debug.Error($"Property {prop.Name} on {comp.GetType()} is read-only.");
                        }

                        continue;
                    }

                    Debug.Error($"Could not find field or property named '{valueRecord.Name}' on {comp.GetType()}");
                }


                return comp;
            }

            catch (Exception e)
            {
                Debug.Error($"Could not load component of type: {typeInstance} because {e}\nThe component was not loaded with the object.");
            }

            return null;
        }
    }
}
