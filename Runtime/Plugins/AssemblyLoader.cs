using Runtime.Data;
using Runtime.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static OpenTK.Graphics.OpenGLES2.GL;

namespace Runtime.Plugins
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
            if (ass == null)
            {
                Debug.Error($"Failed to load assembly from {filename}. Null!");
            }

            return ass;
        }

        public static Assembly? LoadPlugin(string plugin)
        {
            PluginData? pluginData = Files.Load<PluginData>($"plugins/{plugin}/info.plugin.json");
            if (pluginData == null)
            {
                Debug.Error($"Could not find 'info.plugin.json' in plugin: {plugin}!");
                return null;
            }

            string coreDll = pluginData.CoreDll;
            string mainClass = pluginData.MainClass;
            string[] dependencies = pluginData.Dependencies;

            foreach (string dependency in dependencies)
            {
                Debug.Log($"First loading dependency {dependency}");
                LoadPlugin(dependency);
            }

            Assembly? assemblyDef = null;
            string[] dllFiles = Directory.GetFiles($"plugins/{plugin}/net8.0");
            foreach (string dll in dllFiles)
            {
                if (dll.EndsWith(".dll"))
                {
                    Assembly? ass = LoadExternal(dll);
                    if (coreDll != "none" && Path.GetFileName(dll) == coreDll)
                    {
                        assemblyDef = ass;
                    }
                }
            }

            // Loading native dlls
            string platformFolder = $"plugins/{plugin}/net8.0/runtimes/{GetRuntimeIdentifier()}/native";
            Debug.Log($"Loading dll for platform from: {platformFolder}");
            dllFiles = Directory.GetFiles(platformFolder);
            foreach (string dll in dllFiles)
            {
                if (dll.EndsWith(".dll"))
                {
                    NativeLibrary.Load(dll);
                }
            }

            if (mainClass == "none")
            {
                Debug.Log("No main class provided for plugin, nothing will load by itself.");
                return assemblyDef;
            }

            object? obj = Activator.CreateInstance(assemblyDef.GetType(mainClass));

            if (obj is Plugin pluginInstance)
            {
                pluginInstance.Load();
            }
            else
            {
                Debug.Error($"{obj.GetType().AssemblyQualifiedName} is not implementing 'Plugin'. Thus, it can not be used as a start point.");
            }

            return assemblyDef;
        }

        static string GetRuntimeIdentifier()
        {
            string os = GetOS();
            string arch = RuntimeInformation.OSArchitecture.ToString().ToLowerInvariant();
            return $"{os}-{arch}";
        }

        static string GetOS()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return "win";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                return "linux";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                return "osx";
            return "unknown";
        }
    }
}

