using Runtime.Calc;
using Runtime.Logging;
using System.Globalization;
using System.Reflection;

namespace Runtime.Data
{
    public class ValueRecord
    {
        public string Name { get; set; }
        public ValueRecordType Type { get; set; }
        public string Value { get; set; }

        public ValueRecord()
        {

        }
        public ValueRecord(ValueRecordType type, string name, object value)
        {
            Type = type;
            Name = name;

            SetValue(value);
        }

        public ValueRecord(string name, object? value)
        {
            if (value == null)
            {
                Debug.Error($"You can not create a record for value of null on record {name}!");
                return;
            }
            var v = ValueRecordTypeFromType(value!.GetType());

            if (v == null)
            {
                Debug.Error($"You can not create a record for type {value.GetType()} on record {name}");
                return;
            }

            Type = v.Value;
            Name = name;
            SetValue(value);
        }

        public object? GetValue()
        {
            if (Type == ValueRecordType.Null)
                return null;

            switch (Type)
            {
                case ValueRecordType.String:
                    return Value;

                case ValueRecordType.Bool:
                    return bool.Parse(Value);

                case ValueRecordType.Int:
                    return int.Parse(Value, CultureInfo.InvariantCulture);

                case ValueRecordType.Float:
                    return float.Parse(Value, CultureInfo.InvariantCulture);

                case ValueRecordType.Vector2:
                    return Vector2.Parse(Value);

                case ValueRecordType.Vector3:
                    return Vector3.Parse(Value);

                case ValueRecordType.Vector4:
                    return Vector4.Parse(Value);
            }

            var v = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(a => a.GetTypes())
                    .Where(t => typeof(AssetReference).IsAssignableFrom(t) && !t.IsAbstract);

            foreach (var type in v)
            {
                AssetReferenceAttribute? attribute = type.GetCustomAttributes(typeof(AssetReferenceAttribute), false)
                                   .FirstOrDefault() as AssetReferenceAttribute;

                if (attribute.filetype.Contains(Path.GetExtension(Value).ToLower()))
                {
                    MethodInfo? method = type.GetMethod(attribute.createMethod);
                    if (method == null)
                    {
                        Debug.Error($"AssetReference of type [{type.Name}] does not contain a method called {attribute.createMethod}");
                        return null;
                    }

                    var info = method.GetParameters();
                    if (info.Length != 1)
                    {
                        Debug.Error($"AssetReference of type [{type.Name}] create method [{attribute.createMethod}] contained more than 1, or none paramaters. It must always contain one, the file path.");
                        return null;
                    }

                    if (!method.IsStatic)
                    {
                        Debug.Error($"AssetReference of type [{type.Name}] The creation method has to be static!");
                    }

                    // Get the created thing.
                    return method.Invoke(null, new object[] { Game.GetAssetDatabase().GetAsset(Value) });
                }
            }

            Debug.Log($"Could not solve {Value} of type {Type}");
            return null;
        }


        public void SetValue(object value)
        {
            if (value == null)
            {
                Type = ValueRecordType.Null;
                return;
            }

            switch (value)
            {
                case string s:
                    Type = ValueRecordType.String;
                    Value = s;
                    return;

                case float f:
                    Type = ValueRecordType.Float;
                    Value = f.ToString(CultureInfo.InvariantCulture);
                    return;

                case int i:
                    Type = ValueRecordType.Int;
                    Value = i.ToString(CultureInfo.InvariantCulture);
                    return;

                case Vector2 v2:
                    Type = ValueRecordType.Vector2;
                    Value = v2.ToString();
                    return;

                case Vector3 v3:
                    Type = ValueRecordType.Vector3;
                    Value = $"{v3.ToString()}";
                    return;

                case Vector4 v4:
                    Type = ValueRecordType.Vector4;
                    Value = $"{v4.ToString()}";
                    return;

                case bool b:
                    Type = ValueRecordType.Bool;
                    Value = $"{b.ToString()}";
                    return;

                default:
                    break;
            }

            if (typeof(AssetReference).IsAssignableFrom(value.GetType()))
            {
                Asset? asset = ((AssetReference)value).GetAsset();
                if (asset != null)
                {
                    Type = ValueRecordType.Asset;
                    Value = asset.GetPath();
                }
                else
                {
                    Type = ValueRecordType.Null;
                }
            }
        }

        public static ValueRecordType? ValueRecordTypeFromType(Type type)
        {
            if (type == null)
                return ValueRecordType.Null;

            if (type == typeof(bool))
                return ValueRecordType.Bool;

            if (type == typeof(int))
                return ValueRecordType.Int;

            if (type == typeof(float))
                return ValueRecordType.Float;

            if (type == typeof(string))
                return ValueRecordType.String;

            if (type == typeof(Vector2))
                return ValueRecordType.Vector2;

            if (type == typeof(Vector3))
                return ValueRecordType.Vector3;

            if (type == typeof(Vector4))
                return ValueRecordType.Vector4;

            if (typeof(AssetReference).IsAssignableFrom(type))
                return ValueRecordType.Asset;

            return null;
        }
        public enum ValueRecordType
        {
            Bool,
            Int,
            Float,
            String,
            Vector2,
            Vector3,
            Vector4,
            Asset,
            Null
        }
    }
}
