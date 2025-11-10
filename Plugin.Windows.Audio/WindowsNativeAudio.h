#pragma once
#include <vcclr.h>

struct WindowsNativeAudioInternal;

namespace Runtime
{
	namespace Audio
	{
		public ref class Sample abstract
		{
		public:
			virtual cli::array<short>^ GetRawData() = 0;
			operator std::string();
		};
	}
	namespace WindowsNative
	{
		namespace Audio
		{
			public ref class NativeSample: public Runtime::Audio::Sample
			{
				cli::array<short>^ Data;
			public:
				NativeSample(const char* data, size_t length);
				virtual cli::array<short>^ GetRawData() override;
				static Sample^ ReadSample(System::String^ path);
			};

			public ref class WindowsNativeAudioController
			{
				WindowsNativeAudioInternal* i;
			public:
				WindowsNativeAudioController();
				~WindowsNativeAudioController();
				bool Initialize();
				void Open();
				void Close();
				void Play(Runtime::Audio::Sample^ sample);
			};
		}
		[Runtime::Plugins::DartEntryPointAttribute("Load")]
		public ref class WindowsNative
		{
			Audio::WindowsNativeAudioController^ audio;
			static WindowsNative^ instance;
			WindowsNative();

		public:
			static WindowsNative^ GetInstance();
			static Audio::WindowsNativeAudioController^ GetAudio();
			static void Load();
		};

	}
}
