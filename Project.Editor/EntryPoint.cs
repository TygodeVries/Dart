using Runtime.Logging;

namespace Editor
{
    [Runtime.Plugins.DartEntryPoint("Main")]
    public class EntryPoint
    {
        static EntryPoint()
        {

        }

        public static void Main()
        {
            Debug.Log("Loading Editor...");
        }
    }
}
