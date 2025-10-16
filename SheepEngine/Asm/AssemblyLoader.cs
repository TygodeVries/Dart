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
        public static void LoadExternal(string filename)
        {
            Debug.Log($"Loading assembly from {filename}");
            Assembly ass = Assembly.LoadFrom(Path.Join(Directory.GetCurrentDirectory(), filename));
            if(ass == null)
            {
                Debug.Error($"Failed to load assembly from {filename}. Null!");
            }

            Activator.CreateInstance("Editor", "Editor.TestKick");
        }
    }
}
