using System;
using System.Text;
using ggwave.net.Native;
using NAudio.Wave;

namespace ggwave.net.cli;

class Program
{
    
    static void Main(string[] args)
    {
        string dataStr = "Prived";
        Console.WriteLine($"dataStr: {dataStr}");
        var utf8WithoutBom = new UTF8Encoding(false, true);
        byte[] dataBytes = utf8WithoutBom.GetBytes(dataStr);
        Console.WriteLine($"dataBytes: {Convert.ToHexString(dataBytes)}");

        GGWaveParameters parameters = GGWaveStatic.getDefaultParameters();
        // parameters.operatingMode |= GGWaveOperatingMode.USE_DSS;
        var ggwave = new GGWaveInstance();
        Console.WriteLine($"ggwave params: {ggwave.Parameters}");
        
        byte[] encodedBytes = ggwave.encode(dataBytes, GGWaveProtocolId.AUDIBLE_NORMAL, 25);
        Console.WriteLine($"encoded wav {encodedBytes.Length} bytes");
        
        string wavFilePath = "out.wav";
        Console.WriteLine($"wavFilePath: {wavFilePath}");
        var waveFormat = WaveFormat.CreateIeeeFloatWaveFormat((int)ggwave.Parameters.sampleRate, 1);
        using (var stream = new WaveFileWriter(wavFilePath, waveFormat))
        {
            stream.Write(encodedBytes, 0, encodedBytes.Length);
        }
    }
}