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

    /// <summary>
    /// Load external user code into the project
    /// </summary>
    public class AssemblyLoader
    {
        /// <summary>
        /// Display all loaded assemblies
        /// </summary>
        public static void LogLoaded()
        {
            Debug.Log("The following assemblies are loaded:");
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                Debug.Log("- " + asm.FullName);
            }
        }

        /// <summary>
        /// Load an external DLL into the project based on the file name.
        /// </summary>
        /// <param name="filename"></param>
        public static Assembly? LoadExternal(string filename)
        {
            Debug.Log($"Loading assembly from {filename}");
            Assembly ass = Assembly.LoadFrom(Path.Join(Directory.GetCurrentDirectory(), filename));
            if(ass == null)
            {
                Debug.Error($"Failed to load assembly from {filename}. Null!");
            }

            return ass;
        }

        public static Assembly? LoadPlugin(string plugin, string mainDll)
        {
            Assembly? returnValue = null;
            string[] files = Directory.GetFiles($"plugins/{plugin}/net8.0");
            foreach(string file in files)
            {
                if (file.EndsWith(".dll"))
                {
                    Assembly? ass = LoadExternal(file);
                    if(Path.GetFileNameWithoutExtension(file) == mainDll)
                    {
                        returnValue = ass;
                    }
                }
            }

            return returnValue;
        }
    }
}
