using OpenTK.Mathematics;
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
        /// <summary>
        /// Cleanup at the end of a frame
        /// </summary>
        public void EndOfFrame()
        {
            mouseDelta = Vector2.Zero;
        }
        public Vector2 mouseDelta;
        public Vector2 position;

        public bool leftPressed;
        public bool rightPressed;
        public bool middlePressed;
    }
}
