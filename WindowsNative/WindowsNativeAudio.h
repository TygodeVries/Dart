#pragma once


struct WindowsNativeAudioInternal;

namespace Runtime
{
	namespace WindowsNative
	{
		public ref class WindowsNative
		{
			WindowsNative()
			{
			}
		public:
			static void Load();
		};

		namespace Audio
		{
			public ref class WindowsNativeAudioController
			{
				WindowsNativeAudioInternal* i;
			public:
				WindowsNativeAudioController();
				~WindowsNativeAudioController();
				bool Initialize();
			};
		}
	}
}
