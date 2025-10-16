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
        public static Mouse current = new Mouse();
        static Mouse()
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
    }
}
