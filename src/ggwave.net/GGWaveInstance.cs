using System;
using System.Runtime.InteropServices;
using ggwave.net.Native;
using static ggwave.net.Native.Functions;

namespace ggwave.net;

public class GGWaveInstance : IDisposable
{
    int _instance;

    public readonly GGWaveParameters Parameters;

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

    public int rxDurationFrames() => ggwave_rxDurationFrames(_instance);

    public unsafe byte[] encode(byte[] payload, GGWaveProtocolId protocolId, int volumePercent)
    {
        fixed (byte* pPayload = payload)
        {
            //query the number of bytes in the waveform
            int n = ggwave_encode(_instance,
                (IntPtr)pPayload, payload.Length,
                protocolId, volumePercent,
                IntPtr.Zero, 1);
            // allocate the output buffer
            byte[] waveform = new byte[n];
            fixed (byte* pWaveform = waveform)
            {
                // generate the waveform
                ggwave_encode(_instance,
                    (IntPtr)pPayload, payload.Length,
                    protocolId, volumePercent,
                    (IntPtr)pWaveform, 0);
            }

            return waveform;
        }
    }

    public unsafe byte[] decode(byte[] waveform)
    {
        fixed (byte* pWaveform = waveform)
        {
            byte[] payload = new byte[256];
            fixed (byte* pPayload = payload)
            {
                int n = ggwave_ndecode(_instance,
                    (IntPtr)pWaveform, waveform.Length,
                    (IntPtr)pPayload, payload.Length);
                if (n == -1)
                    throw new Exception("Could not decode Waveform");
                if (n == -2)
                    throw new Exception($"Payload buffer is too small ({payload.Length} bytes)");
            }

            return payload;
        }
    }
}