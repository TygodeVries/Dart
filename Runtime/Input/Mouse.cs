using OpenTK.Windowing.Common;
using Runtime.Calc;
using Runtime.Graphics;
using Runtime.Logging;

namespace Runtime.Input
{
    public class Mouse
    {
        public static Mouse current = new Mouse();
        public Vector2 scroll;
        private Mouse()
        {
            Debug.Log("Activated Mouse!");
        }

        public void SetCursorState(CursorState state)
        {
            RenderCanvas.main.CursorState = state;
        }

        /// <summary>
        /// Cleanup at the end of a frame
        /// </summary>
        public void EndOfFrame()
        {
            mouseDelta = Vector2.Zero;
            scroll = Vector2.Zero;
        }
        Vector2 lastFrameScroll = Vector2.Zero;
        public Vector2 mouseDelta;
        public Vector2 position;

        public bool leftPressed;
        public bool rightPressed;
        public bool middlePressed;
    }
}
