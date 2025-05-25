using ggwave.net.Native;

namespace ggwave.net;

public static class GGWaveStatic
{
    public static GGWaveParameters getDefaultParameters() => Functions.ggwave_getDefaultParameters();
    
    public static void rxToggleProtocol(GGWaveProtocolId protocolId, int state) => 
        Functions.ggwave_rxToggleProtocol(protocolId, state);
    public static void rxProtocolSetFreqStart(GGWaveProtocolId protocolId, int freqStart) => 
        Functions.ggwave_rxProtocolSetFreqStart(protocolId, freqStart);
    
    public static void txToggleProtocol(GGWaveProtocolId protocolId, int state) => 
        Functions.ggwave_txToggleProtocol(protocolId, state);
    public static void txProtocolSetFreqStart(GGWaveProtocolId protocolId, int freqStart) => 
        Functions.ggwave_txProtocolSetFreqStart(protocolId, freqStart);
}