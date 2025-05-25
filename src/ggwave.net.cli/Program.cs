using System;
using System.IO;
using ggwave.net.Native;
using NAudio.Wave;

namespace ggwave.net.cli;

class Program
{
    enum ProgramMode
    {
        None,
        Encode,
        Decode,
    }

    private static void Encode(GGWaveInstance ggWave, Stream input, Stream output)
    {
        Debug("encoding wav...");
        var waveFormat = WaveFormat.CreateIeeeFloatWaveFormat((int)ggWave.Parameters.sampleRate, 1);
        using (var waveStream = new WaveFileWriter(output, waveFormat))
        {
            ggWave.EncodeStream(input, waveStream,
                GGWaveProtocolId.AUDIBLE_NORMAL, 25);
            waveStream.Flush();
            Debug("done");
        }
    }
    
    
    private static void Decode(GGWaveInstance ggWave, Stream input, Stream output)
    {
        Debug("decoding wav...");
        using (var waveStream = new WaveFileReader(input))
        {
            ggWave.DecodeStream(waveStream, output);
            output.Flush();
            Debug("done");
        }
    }

    static void Debug(string msg)
    {
        Console.Error.WriteLine(msg);
    }

    static void Error(string msg)
    {
        Console.Error.WriteLine("Error: {0}", msg);
    }
    
    static int Main(string[] args)
    {
        try
        {
            if (args.Length == 0 || args[0] is "-h" or "--help")
            {
                Debug("ggwave.net.cli [MODE] [INPUT_FILE] [OUTPUT_FILE] [OPTIONS]");
                Debug("Modes:");
                Debug("\t-e --encode   - encode data as wav");
                Debug("\t-d --decode   - decode data from wav");
                Debug("Options:");
                Debug("\t--dss         - Enable direct-sequence spread spectrum");
                return 1;
            }
            
            ProgramMode mode;
            switch (args[0])
            {
                case "-e" or "--encode":
                    mode = ProgramMode.Encode;
                    break;
                case "-d" or "--decode":
                    mode = ProgramMode.Decode;
                    break;
                default:
                    Error($"Unknown mode: {args[0]}");
                    return 1;
            }
            
            using Stream inputStream = File.Open(args[1], FileMode.Open, FileAccess.Read);
            using Stream outputStream = File.Open(args[2], FileMode.OpenOrCreate, FileAccess.Write);
            
            GGWaveParameters parameters = GGWaveStatic.getDefaultParameters();
            if(args.Length > 3 && args[3] == "--dss")
            {
                parameters.operatingMode |= GGWaveOperatingMode.USE_DSS;
            }
            Debug($"ggwave parameters: {parameters.ToString().Replace(", ", ",\n\t")}");
            // GGWaveStatic.disableStdoutLog();
            using var ggWave = new GGWaveInstance(parameters);

            switch (mode)
            {
                case ProgramMode.Encode:
                    Encode(ggWave, inputStream, outputStream);
                    break;
                case ProgramMode.Decode:
                    Decode(ggWave, inputStream, outputStream);
                    break;
            }
        }
        catch (Exception ex)
        {
            Error(ex.ToString());
            return 1;
        }

        return 0;
    }
}