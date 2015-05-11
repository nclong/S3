public enum S3_GameMessageType
{
    //Eric
    ClientConnectRequest,
    ServerConnectResponse,
    ClientResponseAck,
    ServerTime,
    ClientTimeReceived,
    ServerTimeOffset,
    
    //Nick
    ServerPlayerPosDR,
    ServerPlayerRotDR,
    ClientPosDR,
    ClientRotDR,
    ServerPlayerDied,
    ServerPlayerHit,
    
    //Kevin
    ServerPlayerSwing,
    ServerPlayerSwitch,
    ServerStartGame,
    ServerNewPlayer,
    ClientSwing,
    ClientSwitch
}