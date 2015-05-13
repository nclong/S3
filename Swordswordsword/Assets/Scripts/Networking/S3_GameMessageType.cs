﻿public enum S3_GameMessageType:int
{
    ClientConnectRequest = 0,
    ServerConnectResponse,
    ClientResponseAck,
    ServerTime,
    ClientTimeReceived,
    ServerTimeOffset,
    ServerPlayerPosDR,
    ServerPlayerRotDR,
    ClientPosDR,
    ClientRotDR,
    ServerPlayerDied,
    ServerPlayerHit,
    ServerPlayerSwing,
    ServerPlayerSwitch,
    ServerStartGame,
    ServerNewPlayer,
    ClientSwing,
    ClientSwitch,
    ServerGameOver
}