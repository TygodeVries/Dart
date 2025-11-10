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
			Assembly? ass = null;
			try
			{
				ass = Assembly.LoadFrom(Path.Join(Directory.GetCurrentDirectory(), filename));
				if (ass == null)
				{
					Debug.Error($"Failed to load assembly from {filename}. Null!");
				}
			}
			catch (Exception ex)
			{
				Debug.Error($"Failed to load assembly: {ex.Message}");
			}
			return ass;
		}
		public static Assembly? LoadAndRun(string filename)
		{
			Assembly? ass = LoadExternal(filename);
			if (null != ass)
			{
				Type[] types = ass.GetTypes();
				foreach (Type t in types)
				{
					Attribute? attr = t.GetCustomAttribute<DartEntryPointAttribute>();
					if (attr is DartEntryPointAttribute da)
					{
						MethodInfo? mi = t.GetMethod(da.EntryPoint);
						mi?.Invoke(null, null);
					}
				}
			}
			else
				Logging.Debug.Error($"Could not load {filename}");

			return ass;
		}
		public static void LoadPlugin(string plugin)
		{
			PluginData? pluginData = Files.Load<PluginData>($"plugins/{plugin}/info.plugin.json");
			if (pluginData == null)
			{
				Debug.Error($"Could not find 'info.plugin.json' in plugin: {plugin}!");
				return;
			}

			List<Assembly> assemblies = new List<Assembly>();

			string[] dllFiles = Directory.GetFiles($"plugins/{plugin}/");
			foreach (string dll in dllFiles)
			{
				if (dll.EndsWith(".dll"))
				{
					Assembly? ass = LoadExternal(dll);
					if (ass == null)
					{
						// Idk do something about it?
						Debug.Error($"DLL Could not be loaded: {dll}!");
						continue;
					}
					
					assemblies.Add(ass);
                }
			}

			// Loading native dlls
			string platformFolder = $"plugins/{plugin}/runtimes/{GetRuntimeIdentifier()}/native";
			bool hasNative = Directory.Exists(platformFolder);
			// Check if there are any native DLLs to load...
			if (hasNative)
			{
				Debug.Log($"Loading native dll for platform from: {platformFolder}");
				dllFiles = Directory.GetFiles(platformFolder);
				foreach (string dll in dllFiles)
				{
					if (dll.EndsWith(".dll"))
					{
						NativeLibrary.Load(dll);
					}
				}
			}


			if (assemblies.Count == 0)
			{
				Debug.Error($"Could not load the plugin {plugin} (assemblies count is 0!)");
				return;
			}

			/* 
			 * Kick start any classes with the DartEntryPointAttribute
			 */

			foreach (Assembly assembly in assemblies)
			{
				Type[] types = assembly.GetTypes();
				foreach (Type t in types)
				{
					Attribute? attr = t.GetCustomAttribute<DartEntryPointAttribute>();
					if (attr is DartEntryPointAttribute da)
					{
						MethodInfo? mi = t.GetMethod(da.EntryPoint);
						mi?.Invoke(null, null);
					}
				}
			}
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
	[AttributeUsage(AttributeTargets.Class, AllowMultiple =false, Inherited = false)]
	public class DartEntryPointAttribute : Attribute
	{
		public string EntryPoint { get; } = "Main";
		public DartEntryPointAttribute(string EP)
		{
			EntryPoint = EP;
		}
	}
}

