using System;
using System.Buffers;
using System.IO;
using ggwave.net.Native;
using static ggwave.net.Native.Functions;

namespace ggwave.net;

public class GGWaveInstance : IDisposable
{
    public readonly GGWaveParameters Parameters;
    private int _instance;

    public GGWaveInstance() : this(GGWaveStatic.getDefaultParameters())
    {
    }

    public GGWaveInstance(GGWaveParameters parameters)
    {
        Parameters = parameters;
        _instance = ggwave_init(parameters);
    }

    public void Dispose()
    {
        if (_instance == 0)
            return;

        ggwave_free(_instance);
        _instance = 0;
    }

    // TODO: find out what is rxDurationFrames
    public int rxDurationFrames() => ggwave_rxDurationFrames(_instance);

    /// calculate the length of waveform output buffer
    private int CalculateEncodedSize(IntPtr pPayload, int payloadLength,
        GGWaveProtocolId protocolId, int volumePercent)
    {
        int n = ggwave_encode(_instance,
            pPayload, payloadLength,
            protocolId, volumePercent,
            IntPtr.Zero, 1);
        if (n < 0)
            throw new Exception("Could not calculate Waveform size");
        return n;
    }


    private int EncodePtr(IntPtr pPayload, int payloadLength,
        GGWaveProtocolId protocolId, int volumePercent, IntPtr pWaveform)
    {
        int n = ggwave_encode(_instance,
            pPayload, payloadLength,
            protocolId, volumePercent,
            pWaveform, 0);
        if (n < 0)
            throw new Exception("Could not encode Waveform");
        return n;
    }

    private int DecodePtr(IntPtr pWaveform, int waveformLength, IntPtr pPayload, int payloadLength)
    {
        int n = ggwave_ndecode(_instance,
            pWaveform, waveformLength,
            pPayload, payloadLength);
        if (n == -2)
            throw new Exception($"Payload buffer is too small ({payloadLength} bytes)");
        if (n < 0)
            throw new Exception("Could not decode Waveform");
        return n;
    }


    public unsafe byte[] EncodeArray(byte[] payload, GGWaveProtocolId protocolId, int volumePercent)
    {
        if (payload.Length == 0)
            return [];

        fixed (byte* pPayload = payload)
        {
            int n = CalculateEncodedSize((IntPtr)pPayload, payload.Length,
                protocolId, volumePercent);
            byte[] waveform = new byte[n];
            fixed (byte* pWaveform = waveform)
            {
                int actuallyEncodedN = EncodePtr((IntPtr)pPayload, payload.Length,
                    protocolId, volumePercent,
                    (IntPtr)pWaveform);
                // TODO: CalculateEncodedSize() may return bigger value than actual encoded pcm size
                if (actuallyEncodedN < n) 
                    Array.Resize(ref waveform, actuallyEncodedN);

                // hope this never happens
                if (actuallyEncodedN > n)
                    throw new Exception($"encoded {actuallyEncodedN} bytes, but calculated size was {n}");
            }

            return waveform;
        }
    }

    public unsafe int DecodeArray(byte[] waveform, byte[] payload)
    {
        fixed (byte* pWaveform = waveform)
        {
            fixed (byte* pPayload = payload)
            {
                int n = DecodePtr((IntPtr)pWaveform, waveform.Length,
                    (IntPtr)pPayload, payload.Length);
                return n;
            }
        }
    }


    public unsafe void EncodeStream(Stream input, Stream output,
        GGWaveProtocolId protocolId, int volumePercent, int inputBufferSize = 8192)
    {
        byte[] inputBuffer = ArrayPool<byte>.Shared.Rent(inputBufferSize);
        try
        {
            fixed (byte* pPayload = inputBuffer)
            {
                int readN;
                while ((readN = input.Read(inputBuffer, 0, inputBufferSize)) != 0)
                {
                    int outputBufferSize = CalculateEncodedSize((IntPtr)pPayload, readN,
                        protocolId, volumePercent);
                    byte[] outputBuffer = ArrayPool<byte>.Shared.Rent(outputBufferSize);
                    try
                    {
                        fixed (byte* pWaveform = outputBuffer)
                        {
                            int encN = EncodePtr((IntPtr)pPayload, readN,
                                protocolId, volumePercent,
                                (IntPtr)pWaveform);
                            output.Write(outputBuffer, 0, encN);
                        }
                    }
                    finally
                    {
                        ArrayPool<byte>.Shared.Return(outputBuffer);
                    }
                }
            }
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(inputBuffer);
        }
    }

    public unsafe void DecodeStream(Stream input, Stream output, int inputBufferSize = 8192)
    {
        byte[] inputBuffer = ArrayPool<byte>.Shared.Rent(inputBufferSize);
        byte[] outputBuffer = ArrayPool<byte>.Shared.Rent(inputBufferSize);
        try
        {
            fixed (byte* pWaveform = inputBuffer)
            {
                fixed (byte* pPayload = outputBuffer)
                {
                    int readN;
                    while ((readN = input.Read(inputBuffer, 0, inputBufferSize)) != 0)
                    {
                        int decN = DecodePtr((IntPtr)pWaveform, readN,
                            (IntPtr)pPayload, outputBuffer.Length);
                        output.Write(outputBuffer, 0, decN);
                    }
                }
            }
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(outputBuffer);
            ArrayPool<byte>.Shared.Return(inputBuffer);
        }
    }
}