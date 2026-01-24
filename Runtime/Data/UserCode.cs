using Runtime;
using Runtime.Calc;
using Runtime.Data;
using Runtime.Logging;
using Runtime.Scenes;
using System.Reflection;
using System.Runtime.Loader;

public class UserCode
{
    private static AssemblyLoadContext? loadContext;
    private static WeakReference? loadContextRef;
    private static bool unloadRequested;


    public static void Unload(Action onUnloadFinish)
    {
        // Try to unload everything
        if (!unloadRequested)
        {
            // Clear the scene
            Scene.Load(new Scene());

            // Ask to clear caches, etc.
            OnAttemptUnload?.Invoke();

            // Unload the context
            loadContext?.Unload();
            loadContext = null;
            unloadRequested = true;
        }

        // Check if we have unloaded
        if (CheckUnload())
        {
            unloadRequested = false;
            onUnloadFinish?.Invoke(); // Keep going!
            return;
        }

        ObjectTracker.ReportAlive();

        // Try again
        new Thread(() =>
        {
            Debug.Log("Waiting for data to be unloaded...");
            Thread.Sleep(500);
            MainThread.Run(() => Unload(onUnloadFinish));
        }).Start();
    }

    public static bool Unloading()
    {
        return unloadRequested;
    }

    private static bool CheckUnload()
    {
        if (loadContextRef == null)
            return true;

        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        return !loadContextRef.IsAlive;
    }

    public static void Load()
    {
        if (loadContext != null)
        {
            Debug.Error("Before loading a new user.dll, the old one has to be unloaded first.");
            return;
        }

        Asset dllAsset = Game.GetAssetDatabase().GetAsset("Game.dll");

        loadContext = new AssemblyLoadContext("Game", isCollectible: true);
        loadContextRef = new WeakReference(loadContext);
        unloadRequested = false;

        loadContext.LoadFromAssemblyPath(dllAsset.GetSystemPath());
        OnAttemptRestore?.Invoke();

        Debug.Log("Loaded Game.dll");
    }

    public static Action? OnAttemptUnload;
    public static Action? OnAttemptRestore;

    public static Assembly[] GetAllAssemblies()
    {
        var main = AppDomain.CurrentDomain.GetAssemblies();
        var user = loadContext != null ? loadContext.Assemblies : new Assembly[0];
        return main.Concat(user).ToArray();
    }

    public static Type? GetTypeOf(string name)
    {
        var type = Type.GetType(name, throwOnError: false);
        if (type != null)
            return type;

        foreach (var assembly in GetAllAssemblies())
        {
            if (assembly.GetName().Name == "Game")
                foreach (var t in assembly.GetTypes())
                {
                    if (t.AssemblyQualifiedName == name)
                        return t;
                }
        }

        return null;
    }


}

public static class ObjectTracker
{
    private static List<WeakReference> tracked = new List<WeakReference>();

    public static void Track(object obj)
    {
        tracked.Add(new WeakReference(obj));
    }

    public static void ReportAlive()
    {
        Console.WriteLine("=== Tracking Report ===");
        foreach (var wr in tracked)
        {
            if (wr.IsAlive)
            {
                Console.WriteLine($"Alive: {wr.Target.GetType().FullName}");
            }
        }
    }
}

