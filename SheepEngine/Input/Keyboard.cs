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
        public static Keyboard current = new Keyboard();
        private Keyboard()
        {
            Debug.Log("Activated keyboard!");
        }

        Dictionary<Keys, bool> keyStates = new Dictionary<Keys, bool>();
        List<Keys> keyPressed = new List<Keys>();
        public void SetKeyState(Keys key, bool pressed)
        {
            if (keyStates.ContainsKey(key))
                keyStates[key] = pressed;
            else
            {
                keyStates.Add(key, pressed);
            }
            if (pressed && !keyPressed.Contains(key))
               keyPressed.Add(key);
        }
         /// <summary>
         /// Cleanup at the end of a frame
         /// </summary>
         public void EndOfFrame()
         {
            keyPressed.Clear();
         }
        public bool IsPressed(Keys key)
        {
            if (!keyStates.ContainsKey(key))
                return false;
            return keyStates[key];
        }
    }
}
