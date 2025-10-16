using Runtime.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static OpenTK.Graphics.OpenGLES2.GL;

namespace Runtime.Asm
{
    public class AssemblyLoader
    {
        public static void LogLoaded()
        {
            Debug.Log("The following assemblies are loaded:");
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                Debug.Log("- " + asm.FullName);
            }
        }

        public static void LoadExternal(string filename)
        {
            Debug.Log($"Loading assembly from {filename}");
            Assembly ass = Assembly.LoadFrom(Path.Join(Directory.GetCurrentDirectory(), filename));
            if(ass == null)
            {
                Debug.Error($"Failed to load assembly from {filename}. Null!");
            }
        }
    }
}
