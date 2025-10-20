
using Runtime;
using System.Runtime.CompilerServices;

namespace FeatureTestProject
{
	[Runtime.Plugins.DartEntryPoint("Main")]
	public class EntryPoint
	{
		public EntryPoint()
		{

		}
		public static void Main()
		{
			Runtime.Logging.Debug.Log("Entrypoint");
			Runtime.WindowsNative.Audio.Sample sample = Runtime.WindowsNative.Audio.Sample.ReadSample("assets\\sounds\\gurgle.wav");
			Runtime.WindowsNative.WindowsNative.GetAudio().Play(sample);
		}
	}
}

