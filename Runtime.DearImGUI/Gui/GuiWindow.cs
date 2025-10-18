using Runtime.DearImGUI.Backend;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runtime.DearImGUI.Gui
{
    public abstract class GuiWindow
    {
        public static void Enable(GuiWindow window)
        {
            ImGuiRenderPass.instance!.guiWindows.Add(window);
        }
        public abstract void Render();
    }
}
