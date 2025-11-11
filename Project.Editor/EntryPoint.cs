using Project.Editor.UI;
using Project.Editor.UI.FileSystem;
using Project.Editor.UI.Inspectors;
using Runtime.Component.Core;
using Runtime.DearImGUI.Gui;
using Runtime.Graphics.Renderers;
using Runtime.Logging;
using Runtime.Objects;
using Runtime.Scenes;

namespace Editor
{
    [Runtime.Plugins.DartEntryPoint("Main")]
    public class EntryPoint
    {
        static EntryPoint()
        {

        }

        public static void Main()
        {
            Debug.Log("Loading Editor...");
            GuiWindow.Enable(new NavBarUI());
            GuiWindow.Enable(new ProjectWindow());
            GuiWindow.Enable(new InspectorWindow());

            float backgroundGrayness = 30;
            Scene.main.Instantiate(new GameObjectFactory().
                AddComponent(new Camera()
                {
                    backgroundColor = new OpenTK.Mathematics.Vector3(backgroundGrayness / 255f, backgroundGrayness / 255f, backgroundGrayness / 255f)
                })
                .AddComponent(new Transform()
                {
                    position = new OpenTK.Mathematics.Vector3(0, 0, 1),
                    rotation = new OpenTK.Mathematics.Vector3(180, 0, 0)
                })
                .Build());

            Scene.main.Instantiate(new GameObjectFactory().
                AddComponent(new TextRenderer("Hallo from\nthe Editor!", TextSpace.World))
                .Build());
        }
    }
}
