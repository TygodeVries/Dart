
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
			Runtime.Audio.Sample sample = Runtime.WindowsNative.Audio.NativeSample.ReadSample("assets\\sounds\\portal.wav");
			Runtime.WindowsNative.WindowsNative.GetAudio().Play(sample);
		}
	}
}

