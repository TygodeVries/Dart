using Project.Editor.Data;
using Project.Editor.UI;
using Project.Editor.UI.Assets;
using Project.Editor.UI.Build_Mode;
using Project.Editor.UI.FileSystem;
using Project.Editor.UI.Inspectors;
using Project.Editor.UI.Scenes;
using Runtime;
using Runtime.Data;
using Runtime.DearImGUI.Gui;
using Runtime.Logging;
using Runtime.Objects;
using Runtime.Scenes;

namespace Project.Editor.EditorModes
{
    public class WindowSwitcher
    {
        public static void Init()
        {
            EditorMode.OnModeSwitch += OnSwitch;
        }

        private static void OnSwitch(Mode mode)
        {
            CloseAll();

            GuiWindow.Enable(new Headerbar());
            GuiWindow.Enable(new DockerWindow());

            if (mode == Mode.Edit)
                EnableEditMode();
            else
                EnableBuildMode();
        }

        private static void CloseAll()
        {
            GuiWindow.DisableAll();
        }

        private static void EnableEditMode()
        {
            GuiWindow.Enable(new ProjectWindow());
            GuiWindow.Enable(new InspectorWindow());
        }

        private static void EnableBuildMode()
        {
            GuiWindow.Enable(new AssetBrowser());
            GuiWindow.Enable(new BuildBar());
            LoadLastScene();
        }

        private static void LoadLastScene()
        {
            Asset asset = Game.GetAssetDatabase().GetAsset(EditorPrefs.GetString("last_open_scene", GameSettings.GetGameSettings().StartScene));

            Scene? scene = Scene.LoadFromAsset(asset);

            if (scene == null)
            {
                Debug.Error("Could not load scene!!!");
                return;
            }

            Scene.Load(scene);
            SceneEditor.EnableInCurrentScene();

            foreach (GameObject gm in scene.GetGameObjects())
            {
                gm.enableUpdates = false;
            }

            SceneEditor.CreateSceneCamera();
        }
    }
}
