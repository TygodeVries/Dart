using Project.Editor;
using Project.Editor.Data;
using Project.Editor.UI;
using Project.Editor.UI.FileSystem;
using Project.Editor.UI.Inspectors;
using Runtime.DearImGUI.Gui;
using Runtime.Graphics.Materials;
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
            string[] args = Environment.GetCommandLineArgs();

            for (int cx = 0; cx < args.Length - 1; cx++)
            {
                if (args[cx] == "-p")
                {
                    Project.Editor.Editor.projectPath = args[cx + 1];
                }
                if (args[cx] == "-e")
                {
                    Project.Editor.Editor.exeLocation = args[cx + 1];
                }
            }

            GuiWindow.Enable(new NavBarUI());
            GuiWindow.Enable(new ProjectWindow());
            GuiWindow.Enable(new InspectorWindow());

            AssetDatabase.Start();
            AssetDatabase.Refresh();

            TextureMaterialField.fallback = DefaultsTextures.GetFallbackTexture();
        }
    }
}
