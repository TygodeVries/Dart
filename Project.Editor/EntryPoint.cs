using Project.Editor.UI;
using Project.Editor.UI.FileSystem;
using Project.Editor.UI.Inspectors;
using Runtime.DearImGUI.Gui;
using Runtime.Logging;

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
            Style.Apply();
            Debug.Log("Loading Editor...");
            GuiWindow.Enable(new NavBarUI());
            GuiWindow.Enable(new ProjectWindow());
            GuiWindow.Enable(new InspectorWindow());
        }
    }
}
