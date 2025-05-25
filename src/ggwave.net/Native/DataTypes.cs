using System;
using System.Runtime.InteropServices;

namespace ggwave.net.Native;

//TODO: find out should it be changed
//#define GGWAVE_MAX_INSTANCES 4

// Data format of the audio samples
public enum GGWaveSampleFormat
{
    FORMAT_UNDEFINED,
    FORMAT_U8,
    FORMAT_I8,
    FORMAT_U16,
    FORMAT_I16,
    FORMAT_F32,
}

// Protocol ids
public enum GGWaveProtocolId
{
    AUDIBLE_NORMAL,
    AUDIBLE_FAST,
    AUDIBLE_FASTEST,
    ULTRASOUND_NORMAL,
    ULTRASOUND_FAST,
    ULTRASOUND_FASTEST,

    DUALTONE_NORMAL,
    DUALTONE_FAST,
    DUALTONE_FASTEST,
    MONOTONE_NORMAL,
    MONOTONE_FAST,
    MONOTONE_FASTEST,

    CUSTOM_0,
    CUSTOM_1,
    CUSTOM_2,
    CUSTOM_3,
    CUSTOM_4,
    CUSTOM_5,
    CUSTOM_6,
    CUSTOM_7,
    CUSTOM_8,
    CUSTOM_9,

    COUNT,
}

public enum GGWaveFilter
{
    HANN,
    HAMMING,
    FIRST_ORDER_HIGH_PASS,
}

// Operating modes of ggwave
//
//   RX:
//     The instance will be able to receive audio data
//
//   TX:
//     The instance will be able generate audio waveforms for transmission
//
//   TX_ONLY_TONES:
//     The encoding process generates only a list of tones instead of full audio
//     waveform. This is useful for low-memory devices and embedded systems.
//
//   USE_DSS:
//     Enable the built-in Direct Sequence Spread (DSS) algorithm
//
[Flags]
public enum GGWaveOperatingMode
{
    RX = 1 << 1,
    TX = 1 << 2,

    RX_AND_TX = (RX |
                                       TX),
    TX_ONLY_TONES = 1 << 3,
    USE_DSS = 1 << 4,
}

// GGWave instance parameters
//
//   If payloadLength <= 0, then GGWave will transmit with variable payload length
//   depending on the provided payload. Sound markers are used to identify the
//   start and end of the transmission.
//
//   If payloadLength > 0, then the transmitted payload will be of the specified
//   fixed length. In this case, no sound markers are emitted and a slightly
//   different decoding scheme is applied. This is useful in cases where the
//   length of the payload is known in advance.
//
//   The sample rates are values typically between 1000 and 96000.
//   Default value: GGWave::kDefaultSampleRate
//
//   The captured audio is resampled to the specified sampleRate if sampleRatInp
//   is different from sampleRate. Same applies to the transmitted audio.
//
//   The samplesPerFrame is the number of samples on which ggwave performs FFT.
//   This affects the number of bins in the Fourier spectrum.
//   Default value: GGWave::kDefaultSamplesPerFrame
//
//   The operatingMode controls which functions of the ggwave instance are enabled.
//   Use this parameter to reduce the memory footprint of the ggwave instance. For
//   example, if only Rx is enabled, then the memory buffers needed for the Tx will
//   not be allocated.
//
[StructLayout(LayoutKind.Sequential)]
public record struct GGWaveParameters
{
    public int payloadLength; // payload length
    public float sampleRateInp; // capture sample rate
    public float sampleRateOut; // playback sample rate
    public float sampleRate; // the operating sample rate
    public int samplesPerFrame; // number of samples per audio frame
    public float soundMarkerThreshold; // sound marker detection threshold
    public GGWaveSampleFormat sampleFormatInp; // format of the captured audio samples
    public GGWaveSampleFormat sampleFormatOut; // format of the playback audio samples
    public GGWaveOperatingMode operatingMode; // operating mode
}