using OpenTK.Windowing.GraphicsLibraryFramework;
using Runtime.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runtime.Input
{
    public class Keyboard
    {
        public static Keyboard current = new Keyboard();

        private Keyboard()
        {
            Debug.Log("Activated keyboard!");
        }

        private Dictionary<Keys, bool> keyStates = new Dictionary<Keys, bool>();

        // Keys pressed during this frame
        private List<Keys> keysPressedThisFrame = new List<Keys>();

        // Keys released during this frame
        private List<Keys> keysReleasedThisFrame = new List<Keys>();

        /// <summary>
        /// Update key state and track presses/releases per frame
        /// </summary>
        public void SetKeyState(Keys key, bool pressed)
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
        public bool IsPressed(Keys key)
        {
            return keyStates.ContainsKey(key) && keyStates[key];
        }

        /// <summary>
        /// Was key pressed this frame?
        /// </summary>
        public bool IsPressedThisFrame(Keys key)
        {
            return keysPressedThisFrame.Contains(key);
        }

        /// <summary>
        /// Was key released this frame?
        /// </summary>
        public bool IsReleasedThisFrame(Keys key)
        {
            return keysReleasedThisFrame.Contains(key);
        }
    }

}
