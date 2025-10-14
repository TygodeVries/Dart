using OpenTK.Windowing.GraphicsLibraryFramework;
using Runtime.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runtime.Input
{
    internal class Keyboard
    {
        public static Keyboard current;
        public Keyboard()
        {
            current = this;
            Debug.Log("Activated keyboard!");
        }

        Dictionary<Keys, bool> keyStates = new Dictionary<Keys, bool>();
        public void SetKeyState(Keys key, bool pressed)
        {
            if (keyStates.ContainsKey(key))
                keyStates[key] = pressed;
            else
            {
                keyStates.Add(key, pressed);
            }
        }

        public bool IsPressed(Keys key)
        {
            if (!keyStates.ContainsKey(key))
                return false;
            return keyStates[key];
        }
    }
}
