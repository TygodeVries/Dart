using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runtime.Logging
{
    public class Debug
    {
        public static void Log(string log)
        {
            var stackTrace = new StackTrace(1, true);
            var frame = stackTrace.GetFrame(0);

            var method = frame.GetMethod();

            Console.WriteLine($"[{method.DeclaringType.FullName}] {log}");
        }

        public static void Error(string log)
        {
            var stackTrace = new StackTrace(1, true);
            var frame = stackTrace.GetFrame(0);

            var method = frame.GetMethod();

            Console.BackgroundColor = ConsoleColor.Red;
            Console.WriteLine($"[{method.DeclaringType.FullName}] {log}");
            Console.BackgroundColor = ConsoleColor.Black;
        }
    }

    public enum LogLevel
    {

    }
}
