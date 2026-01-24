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

        // Try again
        new Thread(() =>
        {
            Thread.Sleep(100);
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

        loadContext = new AssemblyLoadContext("UserCode", isCollectible: true);
        loadContextRef = new WeakReference(loadContext);
        unloadRequested = false;

        loadContext.LoadFromAssemblyPath(dllAsset.GetSystemPath());
        OnAttemptRestore?.Invoke();
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
            type = assembly.GetType(name, throwOnError: false, ignoreCase: false);
            if (type != null)
                return type;
        }

        return null;
    }


}
