using Runtime.Graphics;
using Runtime.Input;
using Runtime.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeatureTestProject.Components
{
    internal class CursorCapture : IComponent
    {
        bool grabbed = false;
        public override void Update()
        {
            if(Keyboard.current.IsPressedThisFrame(OpenTK.Windowing.GraphicsLibraryFramework.Keys.Space))
            {
                grabbed = !grabbed;
                if (grabbed)
                {
                    RenderCanvas.main.CursorState = OpenTK.Windowing.Common.CursorState.Grabbed;
                }
                else
                {
                    RenderCanvas.main.CursorState = OpenTK.Windowing.Common.CursorState.Normal;
                }
            }
        }
    }
}
