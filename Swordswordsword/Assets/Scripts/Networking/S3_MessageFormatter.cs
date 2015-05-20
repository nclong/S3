using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;

public class S3_MessageFormatter  {

    public static byte[] GameMessageToBytes(S3_GameMessage message)
    {
        List<byte> result = new List<byte>();
        result.Add( message.PlayerNum );
        result.AddRange(BitConverter.GetBytes(message.SendTime));
        result.AddRange( BitConverter.GetBytes( (int)( message.MessageType ) ) );
        result.AddRange( GameMessageDataToBytes( message ) );
        return result.ToArray();
    }

    public static S3_GameMessage BytesToGameMessage(byte[] byteMessage)
    {
        int i = 0;
        S3_GameMessage result = new S3_GameMessage();
        result.PlayerNum = byteMessage[i++];
        result.SendTime = BitConverter.ToSingle( byteMessage, i );
        i += sizeof(float);
        result.MessageType = (S3_GameMessageType)( BitConverter.ToInt32( byteMessage, i ) );
        i += sizeof( int );
        result.MessageData = BytesToGameMessageData( SubArray<byte>( byteMessage, i, byteMessage.Length - i ), result.MessageType );

        return result;
    }

    private static IMessageData BytesToGameMessageData(byte[] rawData, S3_GameMessageType type)
    {
        int i = 0;
        switch( type )
        {
            case S3_GameMessageType.ClientConnectRequest:
                return new S3_ClientConnectRequestData
                {
                    ip = SubArray(rawData, i, 4),
                    playerName = Encoding.UTF8.GetString( SubArray(rawData, 4, rawData.Length - 4 ))
                };
            case S3_GameMessageType.ServerConnectResponse:
                return new S3_ServerConnectResponseData
                {
                    acceptance = BitConverter.ToBoolean( rawData, i ),
                    PosX = BitConverter.ToSingle(rawData, i + sizeof(bool)),
                    PosY = BitConverter.ToSingle(rawData, i + sizeof(bool) + sizeof(float))
                };
            case S3_GameMessageType.ClientResponseAck:
                return new S3_ClientResponseAckData
                {
                    ack = BitConverter.ToBoolean( rawData, i )
                };
            case S3_GameMessageType.ServerTime:
                return new S3_ServerTimeData
                {
                    time = BitConverter.ToSingle( rawData, i )
                };
            case S3_GameMessageType.ClientTimeReceived:
                return new S3_ClientTimeReceivedData
                {
                    acceptance = BitConverter.ToBoolean( rawData, i )
                };
            case S3_GameMessageType.ServerTimeOffset:
                return new S3_ServerTimeOffsetData
                {
                    offset = BitConverter.ToSingle( rawData, i )
                };
            case S3_GameMessageType.ServerPlayerPosDR:

                return new S3_ServerPlayerPosDRData
                {
                    DRAngle = BitConverter.ToSingle( rawData, i ),
                    DRPosX = BitConverter.ToSingle( rawData, i + sizeof( float ) * 1 ),
                    DRPosY = BitConverter.ToSingle( rawData, i + sizeof( float ) * 2 ),
                    DRVelX = BitConverter.ToSingle( rawData, i + sizeof( float ) * 3 ),
                    DRVelY = BitConverter.ToSingle( rawData, i + sizeof( float ) * 4 ),
                    InitialTime = BitConverter.ToSingle( rawData, i + sizeof( float ) * 5 ),
                    PlayerNum = rawData[i + sizeof( float ) * 6]
                };

            case S3_GameMessageType.ServerPlayerRotDR:
                return new S3_ServerPlayerRotDRData
                {
                    DRAngle = BitConverter.ToSingle( rawData, i ),
                    PlayerNum = rawData[i + sizeof( float )]
                };

            case S3_GameMessageType.ClientPosDR:
                return new S3_ClientPosDRData
                {
                    DRPosX = BitConverter.ToSingle( rawData, i + sizeof( float ) * 1 ),
                    DRPosY = BitConverter.ToSingle( rawData, i + sizeof( float ) * 2 ),
                    DRVelX = BitConverter.ToSingle( rawData, i + sizeof( float ) * 3 ),
                    DRVelY = BitConverter.ToSingle( rawData, i + sizeof( float ) * 4 )
                };
            case S3_GameMessageType.ClientRotDR:
                return new S3_ClientRotDRData
                {
                    Angle = BitConverter.ToSingle( rawData, i )
                };
            case S3_GameMessageType.ServerPlayerDied:
                return new S3_ServerPlayerDiedData
                {
                    PlayerNum = rawData[i]
                };
            case S3_GameMessageType.ServerPlayerHit:
                return new S3_ServerPlayerHitData
                {
                    PlayerNum = rawData[i],
                    damage = rawData[i + 1]
                };
            case S3_GameMessageType.ServerPlayerSwing:
                return new S3_ServerPlayerSwingData
                {
                    PlayerNum = rawData[i]
                };
            case S3_GameMessageType.ServerPlayerSwitch:
                return new S3_ServerPlayerSwitchData
                {
                    PlayerNum = rawData[i]
                };
            case S3_GameMessageType.ServerStartGame:
                return new S3_ServerStartGameData
                {
                    timeToStart = BitConverter.ToSingle( rawData, i )
                };
            case S3_GameMessageType.ServerNewPlayer:
                return new S3_ServerNewPlayerData
                {
                    PlayerNum = rawData[i],
                    PosX = rawData[i+1],
                    PosY = rawData[i+1+sizeof(float)],
                    PlayerName = Encoding.UTF8.GetString( SubArray<byte>( rawData, i + 1 + sizeof(float) * 2, rawData.Length - (1 + sizeof(float) * 2) ) )
                };
            case S3_GameMessageType.ClientSwing:
                return new S3_ClientSwingData { animateSpeed = 0};
            case S3_GameMessageType.ClientSwitch:
                return new S3_ClientSwitchData { animateSpeed = 0 };
            case S3_GameMessageType.ServerGameOver:
                return new S3_ServerGameOverData
                {
                    Victor = 0
                };
            case S3_GameMessageType.ServerSwordzookaPos:
            case S3_GameMessageType.ServerSwordzookaRot:
            case S3_GameMessageType.ServerSwordzookaHit:
            case S3_GameMessageType.ServerGunswordPos:
            case S3_GameMessageType.ServerGunswordRot:
            case S3_GameMessageType.ServerGunswordHit:
            default:
                throw new DataMisalignedException();
        }
    }

    private static T[] SubArray<T>(T[] data, int index, int length)
    {
        T[] result = new T[length];
        Array.Copy(data, index, result, 0, length);
        return result;
    }
    private static byte[] GameMessageDataToBytes(S3_GameMessage message)
    {
        List<byte> toReturn = new List<byte>();
        switch (message.MessageType)
        {
            case S3_GameMessageType.ClientConnectRequest:
                toReturn.AddRange(((S3_ClientConnectRequestData)(message.MessageData)).ip);
                toReturn.AddRange( Encoding.UTF8.GetBytes( ( (S3_ClientConnectRequestData)( message.MessageData ) ).playerName ));
                return toReturn.ToArray();
            case S3_GameMessageType.ServerConnectResponse:
                toReturn.AddRange( BitConverter.GetBytes( ( (S3_ServerConnectResponseData)( message.MessageData ) ).acceptance ));
                toReturn.AddRange( BitConverter.GetBytes( ( (S3_ServerConnectResponseData)( message.MessageData ) ).PosX ) );
                toReturn.AddRange( BitConverter.GetBytes( ( (S3_ServerConnectResponseData)( message.MessageData ) ).PosY ) );
                return toReturn.ToArray();
            case S3_GameMessageType.ClientResponseAck:
                return BitConverter.GetBytes( ( (S3_ClientResponseAckData)( message.MessageData ) ).ack);
            case S3_GameMessageType.ServerTime:
                return BitConverter.GetBytes( ( (S3_ServerTimeData)( message.MessageData ) ).time );
            case S3_GameMessageType.ClientTimeReceived:
                return BitConverter.GetBytes( ( (S3_ClientTimeReceivedData)( message.MessageData ) ).acceptance );
            case S3_GameMessageType.ServerTimeOffset:
                return BitConverter.GetBytes( ( (S3_ServerTimeOffsetData)( message.MessageData ) ).offset );
            case S3_GameMessageType.ServerPlayerPosDR:
                toReturn.AddRange( BitConverter.GetBytes( ( (S3_ServerPlayerPosDRData)( message.MessageData ) ).DRPosX ) );
                toReturn.AddRange( BitConverter.GetBytes( ( (S3_ServerPlayerPosDRData)( message.MessageData ) ).DRPosY ) );
                toReturn.AddRange( BitConverter.GetBytes( ( (S3_ServerPlayerPosDRData)( message.MessageData ) ).DRVelX ) );
                toReturn.AddRange( BitConverter.GetBytes( ( (S3_ServerPlayerPosDRData)( message.MessageData ) ).DRVelY ) );
                toReturn.AddRange( BitConverter.GetBytes( ( (S3_ServerPlayerPosDRData)( message.MessageData ) ).InitialTime ) );
                toReturn.Add( ( (S3_ServerPlayerPosDRData)( message.MessageData ) ).PlayerNum );
                return toReturn.ToArray();

            case S3_GameMessageType.ServerPlayerRotDR:
                toReturn.AddRange( BitConverter.GetBytes( ( (S3_ServerPlayerRotDRData)( message.MessageData ) ).DRAngle ) );
                toReturn.Add( ( (S3_ServerPlayerRotDRData)( message.MessageData ) ).PlayerNum );
                return toReturn.ToArray();
                
            case S3_GameMessageType.ClientPosDR:
                toReturn.AddRange( BitConverter.GetBytes( ( (S3_ClientPosDRData)( message.MessageData ) ).DRPosX ) );
                toReturn.AddRange( BitConverter.GetBytes( ( (S3_ClientPosDRData)( message.MessageData ) ).DRPosY ) );
                toReturn.AddRange( BitConverter.GetBytes( ( (S3_ClientPosDRData)( message.MessageData ) ).DRVelX ) );
                toReturn.AddRange( BitConverter.GetBytes( ( (S3_ClientPosDRData)( message.MessageData ) ).DRVelY ) );
                return toReturn.ToArray();

            case S3_GameMessageType.ClientRotDR:
                return BitConverter.GetBytes( ( (S3_ClientRotDRData)( message.MessageData ) ).Angle );
            case S3_GameMessageType.ServerPlayerDied:
                return new byte[1] { ( (S3_ServerPlayerDiedData)( message.MessageData ) ).PlayerNum };
            case S3_GameMessageType.ServerPlayerHit:
                return new byte[2] {
                    ( (S3_ServerPlayerHitData)( message.MessageData ) ).PlayerNum,
                    ( (S3_ServerPlayerHitData)( message.MessageData ) ).damage
                };
            case S3_GameMessageType.ServerPlayerSwing:
                return new byte[1] { ( (S3_ServerPlayerSwingData)( message.MessageData ) ).PlayerNum };
            case S3_GameMessageType.ServerPlayerSwitch:
                return new byte[1] { ( (S3_ServerPlayerSwitchData)( message.MessageData ) ).PlayerNum };
            case S3_GameMessageType.ServerStartGame:
                return BitConverter.GetBytes(( (S3_ServerStartGameData)( message.MessageData ) ).timeToStart );
            case S3_GameMessageType.ServerNewPlayer:
                toReturn.Add( ( (S3_ServerNewPlayerData)( message.MessageData ) ).PlayerNum );
                toReturn.AddRange( BitConverter.GetBytes(( (S3_ServerNewPlayerData)( message.MessageData ) ).PosX) );
                toReturn.AddRange( BitConverter.GetBytes( ( (S3_ServerNewPlayerData)( message.MessageData ) ).PosY ) );
                toReturn.AddRange( Encoding.UTF8.GetBytes( ( (S3_ServerNewPlayerData)( message.MessageData ) ).PlayerName ) );
                return toReturn.ToArray();
            case S3_GameMessageType.ClientSwing:
                return new byte[1] { 0 };
            case S3_GameMessageType.ClientSwitch:
                return new byte[1] { 0 };
            case S3_GameMessageType.ServerGameOver:
                return new byte[1] { ((S3_ServerGameOverData)(message.MessageData)).Victor };
            case S3_GameMessageType.ServerSwordzookaPos:
            case S3_GameMessageType.ServerSwordzookaRot:
            case S3_GameMessageType.ServerSwordzookaHit:
            case S3_GameMessageType.ServerGunswordPos:
            case S3_GameMessageType.ServerGunswordRot:
            case S3_GameMessageType.ServerGunswordHit:
            default:
                return new byte[1] { 0 };
        }
    }


}
