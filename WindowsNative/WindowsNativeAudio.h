#pragma once
#include <vcclr.h>

struct WindowsNativeAudioInternal;

namespace Runtime
{
	namespace WindowsNative
	{
		namespace Audio
		{
			public ref class WindowsNativeAudioController
			{
				WindowsNativeAudioInternal* i;
			public:
				WindowsNativeAudioController();
				~WindowsNativeAudioController();
				bool Initialize();
				void Open();
				void Close();
			};
			public ref class Sample
			{
				cli::array<short> ^Data;
			public:
				Sample(const char* data, size_t length);
				static Sample^ ReadSample(System::String ^path);
			};
		}
		public ref class WindowsNative
		{
			Audio::WindowsNativeAudioController^ audio;
			static WindowsNative^ instance;
			WindowsNative()
			{
			}

		public:
			static void Load();
		};

	}
}
