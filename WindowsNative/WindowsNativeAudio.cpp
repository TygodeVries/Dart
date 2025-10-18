#include "pch.h"

#include <Windows.h>
#include <stdio.h>

#include "WindowsNativeAudio.h"

#pragma comment(lib, "winmm")

struct WindowsNativeAudioInternal
{
	static void CallBack(HWAVEOUT handle, UINT uMsg, DWORD_PTR i, DWORD_PTR wParam, DWORD_PTR lParam)
	{
		printf("Audio callback\n");
	}
	HWAVEOUT waveOut = 0;
};

::Runtime::Audio::WindowsNativeAudioController::WindowsNativeAudioController()
{
	i = new WindowsNativeAudioInternal;
}

::Runtime::Audio::WindowsNativeAudioController::~WindowsNativeAudioController()
{
	waveOutClose(i->waveOut);
	delete i;
}

bool ::Runtime::Audio::WindowsNativeAudioController::Initialize()
{
	WAVEFORMATEX format{};
	format.wFormatTag = WAVE_FORMAT_PCM;
	format.nChannels = 1;
	format.nSamplesPerSec = 22050;
	format.wBitsPerSample = 16;

	format.nAvgBytesPerSec = format.nChannels * format.nSamplesPerSec * format.wBitsPerSample / 8;
	format.nBlockAlign = format.nChannels * format.wBitsPerSample / 8;
	MMRESULT result =
		waveOutOpen(&i->waveOut, WAVE_MAPPER, &format, (DWORD_PTR)&WindowsNativeAudioInternal::CallBack, (DWORD_PTR)i, CALLBACK_FUNCTION);

	MMSYSERR_BASE;
	printf("Result: %i\n", result);
	return !result;
}
