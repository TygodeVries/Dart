using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runtime.Calc
{

    /// <summary>
    /// Time related tools.
    /// </summary>
    public class Time
    {
        /// <summary>
        /// The time since the last frame, in seconds.
        /// </summary>
        public static double deltaTime = 0;

        public static double frameRate = 0;
    }
}
