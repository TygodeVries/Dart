using System.Reflection;

namespace Runtime.Data
{
    public class UserCode
    {

        public static IEnumerable<string> GetAllComponentNames()
        {

            return GetAllAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => typeof(Component).IsAssignableFrom(t) && !t.IsAbstract)
                .Select(t => t.FullName!);
        }

        public static IEnumerable<string> GetAllTypeNamesThatInherit(Type inspectorType)
        {
            return GetAllAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => inspectorType.IsAssignableFrom(t) && !t.IsAbstract)
                .Select(t => t.FullName!);
        }
        public static Assembly[] GetAllAssemblies()
        {
            var main = AppDomain.CurrentDomain.GetAssemblies();
            return main.ToArray();
        }

        public static Type? GetTypeFromName(string name)
        {
            return GetAllAssemblies()
                .SelectMany(a => a.GetTypes())
                .FirstOrDefault(t => t.FullName == name);
        }

        private static bool unloading;
        public static bool IsUnloading() => unloading;

    }
}
