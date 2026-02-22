using Runtime.Calc;
using Runtime.Data;

namespace Project.Editor.Data
{
    public class EditorPrefs
    {
        private static List<ValueRecord> records = new List<ValueRecord>();

        public static ValueRecord? GetRecord(string name)
        {
            foreach (ValueRecord r in records)
            {
                if (r.Name == name)
                    return r;
            }

            return null;
        }

        public static Vector3 GetVector3(string key, Vector3 def)
        {
            ValueRecord? record = GetRecord(key);
            if (record == null || record.GetValue() == null)
                return def;

            return (Vector3)record!.GetValue()!;
        }

        public static float GetFloat(string key, float def)
        {
            ValueRecord? record = GetRecord(key);
            if (record == null || record.GetValue() == null)
                return def;

            return (float)record!.GetValue()!;
        }

        public static string GetString(string key, string def)
        {
            ValueRecord? record = GetRecord(key);
            if (record == null || record.GetValue() == null)
                return def;

            return (string)record!.GetValue()!;
        }
    }
}
