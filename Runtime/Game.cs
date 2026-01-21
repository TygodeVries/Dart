using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
using Runtime.Data;
using Runtime.Graphics;
using Runtime.Graphics.Pipeline;
using Runtime.Plugins;
using Runtime.Scenes;
using Runtime.Tests;
using System.Globalization;
using static Runtime.Logging.Debug;

namespace Runtime
{
    public delegate void DartEventHandler();
    public class Game
    {
        private static AssetDatabase assetDatabase;

        public static AssetDatabase GetAssetDatabase()
        {
            return assetDatabase;
        }

        public static void SetAssetDatabase(AssetDatabase assetDatabase)
        {
            Game.assetDatabase = assetDatabase;
        }

        public static int width = 640 * 2;
        public static int height = 480 * 2;
        public static void Start(string path)
        {
            Log("Starting Tests...");
            Test[] tests = { new ValueRecordTest(), new PrefabRecordTest() };

            foreach (var test in tests)
            {
                (TestResult result, string reason) result = test.Start();
                if (result.result == TestResult.Success)
                {
                    Log($"Passed test {test.GetType()}! \"{result.reason}\"");
                }

                if (result.result == TestResult.Failure)
                {
                    Error($"Failed test {test.GetType()}! \"{result.reason}\"");
                }
            }

            Log("Tests completed!");

            Log("Starting Dart v0.1...");
            Log($"Working from {path}");
            Directory.SetCurrentDirectory(path);

            Log($"Loading asset database...");
            assetDatabase = new AssetDatabase(Directory.GetCurrentDirectory());
            assetDatabase.Start();

            Log($"Loading {Path.Join(path, "GameSettings.json")}...");
            GameSettings? gameSettings = Files.Load<GameSettings>("GameSettings.json");

            if (null == gameSettings)
            {
                Error("GameSettings.json not loaded");
                return;
            }

            Log("Attempting to switch to dedicated graphics card (If present)");
            DedicatedSwitch.Switch();

            Log($"Creating window of size {width}, {height}");
            Log($"Setting window title to {gameSettings!.WindowTitle}");
            var nativeWindowSettings = new NativeWindowSettings()
            {
                ClientSize = new Vector2i(width, height),
                Title = gameSettings?.WindowTitle,
                StartVisible = false
            };

            Log("Creating window...");
            RenderCanvas window = new RenderCanvas(nativeWindowSettings);

            Log("Loading Plugins...");
            foreach (string plugin in gameSettings!.Plugins)
            {
                Log($"Loading {plugin}...");
                AssemblyLoader.LoadPlugin(plugin);
                Log($"Plugin loaded!");
            }

            IGraphicsPipeline graphicsPipeline = new DefaultGraphicsPipeline();
            Log($"Using graphicsPipeline: {graphicsPipeline}.");
            window.SetGraphicsPipeline(graphicsPipeline);

            if (gameSettings!.CodePath == null)
            {
                Error("No user code was defined in game settings, we will not load any of your code!");
            }
            else
            {
                if (File.Exists(gameSettings!.CodePath))
                {
                    Log($"Loading user code from {gameSettings.CodePath}");
                    AssemblyLoader.LoadAndRun(gameSettings.CodePath);
                }
                else
                {
                    Error($"Could not load user code from path {gameSettings!.CodePath}. File not found!");
                }
            }

            Log("Loading start scene...");
            if (gameSettings!.StartScene == null)
            {
                Warning("No start scene was set, we will be loading an empty scene for you!");
                Scene.Load(new Scene());
            }
            else
                Scene.LoadDefault();

            onReady?.Invoke();
            window.IsVisible = true;
            Log($"Opening window...");
            window.Run(); // Keeps the thread blocked until closed.
            Log($"Cleaning up...");
        }

        public static event DartEventHandler? onReady;
    }

    class Program
    {
        public static void Main(string[] args)
        {
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.CreateSpecificCulture("en-US");
            if (0 != args.Length)
            {
                Game.Start(args[0]);
            }
            else
            {
                Logging.Debug.Error("(FATALITY) No project given");
            }
        }
    }
}