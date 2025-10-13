using OpenTK.Mathematics;
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
        }

        public Vector2 mouseDelta;
    }
}
