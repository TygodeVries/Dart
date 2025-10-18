#pragma once

using namespace System;

struct WindowsNativeAudioInternal;

namespace Runtime
{
	public ref class WindowsNative
	{
		WindowsNative()
		{
		}

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
