using OpenTK.Mathematics;
using Runtime.Data;
using Runtime.Graphics.Materials;
using Runtime.Graphics.Renderers;
using Runtime.Objects;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Runtime.Scenes
{
    public class SceneSettings
    {
        public static SceneSettings LoadFromFile(string path)
        {
            string content = File.ReadAllText(path);
            return JsonSerializer.Deserialize<SceneSettings>(content);
        }

        public GameObjectSettings[] objects { get; set; }

        public Scene GetScene()
        {
            Scene scene = new Scene();
            
            foreach(GameObjectSettings settings in objects)
            {
                scene.Instantiate(settings.GetGameObject());
            }

            return scene;
        }
    }

    public class GameObjectSettings
    {
        public ComponentSettings[] components { get; set; }

        public GameObject GetGameObject()
        {
            GameObject gameObject = new GameObject();
            foreach(ComponentSettings settings in components)
            {
                gameObject.AddComponent(settings.GetComponent());
            }
            return gameObject;
        }
    }

    public class ComponentSettings
    {
        public string type { get; set; }
        public ComponentProperty[] properties { get; set; }

        public IComponent GetComponent()
        {
            // Create the instance
            Type type = Type.GetType(this.type) ;
            if(type == null)
            {
                Console.WriteLine($"Could not find a component by type '{this.type}' based on the string.");
            }

            IComponent instance = (IComponent) Activator.CreateInstance(type);

            // Set the values
            foreach(ComponentProperty prop in properties)
            {
                try
                {
                    
                    if(prop.key == null)
                    {
                        Console.WriteLine("Key is null!");
                        continue;
                    }
                    // Try to get the field first
                    FieldInfo? fieldInfo = type.GetField(prop.key, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                    // If no field found, try to get the property
                    PropertyInfo? propertyInfo = null;
                    if (fieldInfo == null)
                    {
                        propertyInfo = type.GetProperty(prop.key, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    }

                    if (fieldInfo == null && propertyInfo == null)
                    {
                        Console.WriteLine($"Tried to write to '{prop.key}' but it does not exist on {type}!");
                        continue;
                    }

                    if (!instance.OverrideParse(prop.key, prop.value))
                    {
                        Console.WriteLine($"Writing for key {prop.key}");
                        var parsedValue = Parse(prop.type, prop.value);

                        if (fieldInfo != null)
                            fieldInfo.SetValue(instance, parsedValue);
                        else if (propertyInfo?.CanWrite == true)
                            propertyInfo.SetValue(instance, parsedValue);
                        else
                            Console.WriteLine($"Property {prop.key} is read-only.");
                    }

                    else
                    {
                        Console.WriteLine($"Overwrite on {prop.key}");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error while loading component field '{prop.key}'! {e}");
                }
            }

            return instance;
        }

        public object Parse(string type, object value)
        {
            if(type == "default")
                return value;

            ParseablePropTypes pType = (ParseablePropTypes)Enum.Parse(typeof(ParseablePropTypes), type);

            if (pType == ParseablePropTypes.Vector3)
            {
                string[] args = ((string) value).Split(' ');
                float x = float.Parse(args[0], CultureInfo.InvariantCulture);
                float y = float.Parse(args[1], CultureInfo.InvariantCulture);
                float z = float.Parse(args[2], CultureInfo.InvariantCulture);

                return new Vector3(x, y, z);
            }

            if(pType == ParseablePropTypes.Mesh)
            {
                Mesh mesh = Mesh.FromFileObj((string) value);
                Console.WriteLine($"Loaded mesh from {(string) value}");
                return mesh;
            }

            if(pType == ParseablePropTypes.Material)
            {
                MaterialSettings materialSettings = MaterialSettings.LoadFromFile((string)value);
                return materialSettings.GetMaterial();
            }


            return value;
        }
    } 

    enum ParseablePropTypes
    {
        Vector3,
        Mesh,
        Material
    }

    public class ComponentProperty
    {
        public string key { get; set; }
        public string type { get; set; } = "default";
        public string value { get; set; }
    }
}
