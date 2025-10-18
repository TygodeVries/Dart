#include "pch.h"
#include <string>
#include <Windows.h>
#include <vcclr.h>
#include <stdio.h>
#include <math.h>

#include "WindowsNativeAudio.h"

#pragma comment(lib, "winmm")
using namespace Runtime;
using namespace Runtime::WindowsNative;
using namespace Runtime::WindowsNative::Audio;

struct WindowsNativeAudioInternal
{
	gcroot<::Runtime::WindowsNative::Audio::WindowsNativeAudioController^> parent;
	WAVEFORMATEX format;
	static void CallBack(HWAVEOUT handle, UINT uMsg, DWORD_PTR i, DWORD_PTR wParam, DWORD_PTR lParam)
	{
		WindowsNativeAudioInternal* pHandler = (WindowsNativeAudioInternal*)i;
		switch (uMsg)
		{
		case WOM_OPEN:
			pHandler->parent->Open();
			break;
		case WOM_CLOSE:
			pHandler->parent->Close();
			break;
		case WOM_DONE:
		{
			WAVEHDR* header = (WAVEHDR*)wParam;
			waveOutWrite(handle, header, sizeof(WAVEHDR));
		}
		break;
		}
	}
	HWAVEOUT waveOut = 0;
	WAVEHDR headers[3] = { 0 };
	WindowsNativeAudioInternal()
	{
		FillFormat(format);
	}
	static void FillFormat(WAVEFORMATEX& format)
	{
		format.wFormatTag = WAVE_FORMAT_PCM;
		format.nChannels = 1;
		format.nSamplesPerSec = 22050;
		format.wBitsPerSample = 16;
		format.nAvgBytesPerSec = format.nChannels * format.nSamplesPerSec * format.wBitsPerSample / 8;
		format.nBlockAlign = format.nChannels * format.wBitsPerSample / 8;
	}
	bool Initialize()
	{
		MMRESULT result =
			waveOutOpen(&waveOut, WAVE_MAPPER, &format, (DWORD_PTR)&WindowsNativeAudioInternal::CallBack, (DWORD_PTR)this, CALLBACK_FUNCTION);
		if (result)
		{
			return false;
		}
		waveOutPause(waveOut);
		for (int cx = 0; cx < 3; cx++)
		{
			int buffer_size = format.nBlockAlign * (int)(33 * format.nAvgBytesPerSec / 1000 / format.nBlockAlign);
			char* buffer = new char[buffer_size];
			signed short* samples = (signed short*)buffer;
			for (int cy = 0; cy < buffer_size / 2; cy++)
				samples[cy] = 0x100 * sin(110. * 2. * 3.14159265 * cy / format.nSamplesPerSec);
			headers[cx].dwBufferLength = buffer_size;
			headers[cx].lpData = buffer;
		
			waveOutPrepareHeader(waveOut, &headers[cx], sizeof(WAVEHDR));
			waveOutWrite(waveOut, &headers[cx], sizeof(headers[cx]));
		}
		waveOutRestart(waveOut);
		return true;
	}
	void Cleanup()
	{
		waveOutReset(waveOut);
		waveOutClose(waveOut);
		for (int cx = 0; cx < 3; cx++)
		{
			waveOutUnprepareHeader(waveOut, &headers[cx], sizeof(headers[cx]));
			delete[] headers[cx].lpData;
		}
	}
};

::Runtime::WindowsNative::Audio::WindowsNativeAudioController::WindowsNativeAudioController()
{
	i = new WindowsNativeAudioInternal;
	i->parent = this;
}

::Runtime::WindowsNative::Audio::WindowsNativeAudioController::~WindowsNativeAudioController()
{
	waveOutClose(i->waveOut);
	delete i;
}

bool ::Runtime::WindowsNative::Audio::WindowsNativeAudioController::Initialize()
{

	return i->Initialize();
}

void Runtime::WindowsNative::Audio::WindowsNativeAudioController::Open()
{
	Logging::Debug::Log("WindowsNativeAudioController started");
}

void Runtime::WindowsNative::Audio::WindowsNativeAudioController::Close()
{
	Logging::Debug::Log("WindowsNativeAudioController stopped");
}

#include <mfapi.h>
#include <mfidl.h>
#include <mfreadwrite.h>

void ::Runtime::WindowsNative::WindowsNative::Load()
{
	MFStartup(MF_VERSION);
	Logging::Debug::Log("Loading WindowsNative plugin");
	instance = gcnew ::Runtime::WindowsNative::WindowsNative;
	instance->audio = gcnew ::Runtime::WindowsNative::Audio::WindowsNativeAudioController;
	Sample^ pinkPather = Sample::ReadSample(LR"URL(G:\Compressed Backup\mp3\Punk\Punk Covers - Pink panther (Ska Version).mp3)URL");
	if (instance->audio->Initialize())
	{
		Logging::Debug::Log("Loading WindowsNativeAudio succeeded");
	}
	else
	{
		Logging::Debug::Log("Loading WindowsNativeAudio failed");
	}
}


#pragma comment(lib, "mf")
#pragma comment(lib, "mfplat")
#pragma comment(lib, "mfuuid")
#pragma comment(lib, "mfreadwrite")

Runtime::WindowsNative::Audio::Sample::Sample(const char* data, size_t length /* in bytes */)
{
	Data = gcnew cli::array<short>(length);
	pin_ptr<short> dt = &Data[0];
	memcpy(dt, data, length);
}

Sample^ ::Runtime::WindowsNative::Audio::Sample::ReadSample(System::String^ path)
{
	pin_ptr<const wchar_t> lpath = PtrToStringChars(path);
	IMFSourceReader* reader;
	IMFAttributes* attributes;
	HRESULT res;
	res = MFCreateAttributes(&attributes, 0);
	if (!SUCCEEDED(res))
	{
		return nullptr;
	}
	res = MFCreateSourceReaderFromURL(lpath, attributes, &reader);
	if (!SUCCEEDED(res))
	{
		attributes->Release();
		return nullptr;
	}
	IMFMediaType* mt;
	MFCreateMediaType(&mt);
	WAVEFORMATEX format;
	WindowsNativeAudioInternal::FillFormat(format);
	mt->SetGUID(MF_MT_MAJOR_TYPE, MFMediaType_Audio);
	mt->SetGUID(MF_MT_SUBTYPE, MFAudioFormat_PCM);
	mt->SetUINT32(MF_MT_AUDIO_NUM_CHANNELS, format.nChannels);
	mt->SetUINT32(MF_MT_AUDIO_SAMPLES_PER_SECOND, format.nSamplesPerSec);
	mt->SetUINT32(MF_MT_AUDIO_BLOCK_ALIGNMENT, format.nBlockAlign);
	mt->SetUINT32(MF_MT_AUDIO_AVG_BYTES_PER_SECOND, format.nAvgBytesPerSec);
	mt->SetUINT32(MF_MT_AUDIO_BITS_PER_SAMPLE, format.wBitsPerSample);
	mt->SetUINT32(MF_MT_ALL_SAMPLES_INDEPENDENT, 1);

	res = reader->SetCurrentMediaType(MF_SOURCE_READER_FIRST_AUDIO_STREAM, 0, mt);
	if (!SUCCEEDED(res))
	{
		mt->Release();
		attributes->Release();
		return nullptr;
	}
	IMFSample* Sample;
	DWORD ActualStream, StreamFlags;
	LONGLONG Timestamp;
	std::string samples;
	while (SUCCEEDED(reader->ReadSample(MF_SOURCE_READER_FIRST_AUDIO_STREAM, 0, &ActualStream, &StreamFlags, &Timestamp, &Sample)))
	{
		if (!Sample)
			break;

		IMFMediaBuffer* buffer;
		Sample->ConvertToContiguousBuffer(&buffer);

		BYTE* bytes;
		ULONG curlength, maxlength;
		buffer->Lock(&bytes, &maxlength, &curlength);

		samples += std::string((const char *)bytes, curlength);

		buffer->Unlock();
		buffer->Release();
		Sample->Release();
	}
	Audio::Sample^ sample = gcnew Audio::Sample(samples.data(), samples.size());

	return sample;
}
