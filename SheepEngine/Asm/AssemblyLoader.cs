using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Runtime.Asm
{
    public class AssemblyLoader
    {
        public static void LogLoaded()
        {
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                Console.WriteLine(asm.FullName);
            }
        }

        public static void LoadExternal(string filename)
        {
            Assembly.LoadFrom(filename);
        }
    }
}
