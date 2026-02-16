using Runtime.Logging;

namespace Runtime.Input
{
    public class Keyboard
    {
        public static Keyboard current = new Keyboard();

        private Keyboard()
        {
            Debug.Log("Activated keyboard!");
        }

        private Dictionary<Key, bool> keyStates = new Dictionary<Key, bool>();

        // Keys pressed during this frame
        private List<Key> keysPressedThisFrame = new List<Key>();

        // Keys released during this frame
        private List<Key> keysReleasedThisFrame = new List<Key>();

        /// <summary>
        /// Update key state and track presses/releases per frame
        /// </summary>
        public void SetKeyState(Key key, bool pressed)
        {
            bool wasPressed = keyStates.ContainsKey(key) && keyStates[key];

            keyStates[key] = pressed;

            if (pressed && !wasPressed)
            {
                // Key went down this frame
                if (!keysPressedThisFrame.Contains(key))
                    keysPressedThisFrame.Add(key);
            }
            else if (!pressed && wasPressed)
            {
                // Key was released this frame
                if (!keysReleasedThisFrame.Contains(key))
                    keysReleasedThisFrame.Add(key);
            }
        }

        public float GetAxis(KeyboardAxis axis)
        {
            if (axis == KeyboardAxis.Vertical)
            {
                if (IsPressed(Key.W))
                    return 1;

                if (IsPressed(Key.S))
                    return -1;
            }

            if (axis == KeyboardAxis.Horizontal)
            {
                if (IsPressed(Key.D))
                    return 1;

                if (IsPressed(Key.A))
                    return -1;
            }

            return 0;
        }

        /// <summary>
        /// Cleanup pressed/released states at end of frame
        /// </summary>
        public void EndOfFrame()
        {
            keysPressedThisFrame.Clear();
            keysReleasedThisFrame.Clear();
        }

        /// <summary>
        /// Is key currently pressed?
        /// </summary>
        public bool IsPressed(Key key)
        {
            return keyStates.ContainsKey(key) && keyStates[key];
        }

        /// <summary>
        /// Was key pressed this frame?
        /// </summary>
        public bool IsPressedThisFrame(Key key)
        {
            return keysPressedThisFrame.Contains(key);
        }

        /// <summary>
        /// Was key released this frame?
        /// </summary>
        public bool IsReleasedThisFrame(Key key)
        {
            return keysReleasedThisFrame.Contains(key);
        }
    }

    public enum KeyboardAxis
    {
        Horizontal,
        Vertical
    }
}
