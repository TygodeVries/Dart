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

        public void HideCursor()
        {
            SetCursorState(CursorState.Grabbed);
        }

        public void ShowCursor()
        {
            SetCursorState(CursorState.Normal);
        }

        private void SetCursorState(CursorState state)
        {
            RenderCanvas.main!.CursorState = state;
        }

        /// <summary>
        /// Cleanup at the end of a frame
        /// </summary>
        public void EndOfFrame()
        {
            mouseDelta = Vector2.Zero;
            scroll = Vector2.Zero;

            leftWasPressed = leftPressed;
            rightWasPressed = rightPressed;
        }
        Vector2 lastFrameScroll = Vector2.Zero;
        public Vector2 mouseDelta;
        public Vector2 position;

        public bool leftPressed;
        public bool rightPressed;
        public bool middlePressed;

        private bool leftWasPressed;
        private bool rightWasPressed;
        public bool LeftPressedThisFrame()
        {
            return leftPressed && !leftWasPressed;
        }

        public bool RightPressedThisFrame()
        {
            return rightPressed && !rightWasPressed;
        }

        public bool RightReleasedThisFrame()
        {
            return !rightPressed && rightWasPressed;
        }

        public bool LeftReleasedThisFrame()
        {
            return !leftPressed && leftWasPressed;
        }
    }
}
