using OpenTK.Mathematics;
using Runtime.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runtime.Input
{
    internal class Mouse
    {
        public static Mouse current;
        public Mouse()
        {
            current = this;
            Debug.Log("Activated Mouse!");
        }

        public Vector2 mouseDelta;
    }
}
