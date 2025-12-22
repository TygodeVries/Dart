using Runtime.Calc;
using Runtime.Logging;
using System.Globalization;

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

        public object GetValue()
        {
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

                default:
                    Debug.Error($"Unsupported type: {Type}");
                    return null;
            }
        }


        public void SetValue(object value)
        {
            switch (value)
            {
                case string s:
                    Type = ValueRecordType.String;
                    Value = s;
                    break;

                case float f:
                    Type = ValueRecordType.Float;
                    Value = f.ToString(CultureInfo.InvariantCulture);
                    break;

                case int i:
                    Type = ValueRecordType.Int;
                    Value = i.ToString(CultureInfo.InvariantCulture);
                    break;

                case Vector2 v2:
                    Type = ValueRecordType.Vector2;
                    Value = v2.ToString();
                    break;

                case Vector3 v3:
                    Type = ValueRecordType.Vector3;
                    Value = $"{v3.ToString()}";
                    break;

                case Vector4 v4:
                    Type = ValueRecordType.Vector4;
                    Value = $"{v4.ToString()}";
                    break;

                case bool b:
                    Type = ValueRecordType.Bool;
                    Value = $"{b.ToString()}";
                    break;

                default:
                    Debug.Error($"Value of type {value.GetType()} is not supported!");
                    break;
            }
        }

        public static ValueRecordType? ValueRecordTypeFromType(Type type)
        {
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
            Vector4
        }
    }
}
