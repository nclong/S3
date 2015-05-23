using UnityEngine;
using System.Collections;
using System.Net;

public class S3_HostMessageProcessor : MonoBehaviour {

    private S3server server;

    void Start()
    {
        server = GetComponent<S3server>();
    }
	public void ProcessMessage(S3_GameMessage message)
    {
        switch(message.MessageType)
        {
            case S3_GameMessageType.ClientConnectRequest:
                HandleConnectRequest(message); break;
            case S3_GameMessageType.ClientResponseAck:
                HandleResponseAck( message ); break;
            case S3_GameMessageType.ClientTimeReceived:
                HandleTimeReceived( message ); break;
            case S3_GameMessageType.ClientPosDR:
                HandlePosDR( message ); break;
            case S3_GameMessageType.ClientRotDR:
                HandleRotDR( message ); break;
            case S3_GameMessageType.ClientSwing:
                HandleSwing( message ); break;
            case S3_GameMessageType.ClientSwitch:
                HandleSwitch( message ); break;
            case S3_GameMessageType.ServerConnectResponse:
            case S3_GameMessageType.ServerTime:
            case S3_GameMessageType.ServerTimeOffset:
            case S3_GameMessageType.ServerPlayerPosDR:
            case S3_GameMessageType.ServerPlayerRotDR:
            case S3_GameMessageType.ServerPlayerDied:
            case S3_GameMessageType.ServerPlayerHit:
            case S3_GameMessageType.ServerPlayerSwing:
            case S3_GameMessageType.ServerPlayerSwitch:
            case S3_GameMessageType.ServerStartGame:
            case S3_GameMessageType.ServerNewPlayer:
            case S3_GameMessageType.ServerSwordzookaPos:
            case S3_GameMessageType.ServerSwordzookaRot:
            case S3_GameMessageType.ServerSwordzookaHit:
            case S3_GameMessageType.ServerGunswordPos:
            case S3_GameMessageType.ServerGunswordRot:
            case S3_GameMessageType.ServerGunswordHit:
            default:
                break;
        }
    }

    private void HandleConnectRequest( S3_GameMessage message )
    {
        S3_ClientConnectRequestData requestData = (S3_ClientConnectRequestData)(message.MessageData);
        int playerNum = server.playerManager.CurrentPlayers;
        S3_ServerConnectResponseData responseData = new S3_ServerConnectResponseData();
        if( server.playerManager.HasRoom)
        {
            responseData.acceptance = true;
            server.PlayerEndPoints[playerNum] = new IPEndPoint( new IPAddress( requestData.ip ), 3501 );
            server.PlayerNames[playerNum] = requestData.playerName;
            server.playerManager.CreatePlayer();
            server.playerManager.Players[playerNum].transform.position =
                server.playerManager.SpawnPoints[playerNum].transform.position;

            responseData.PosX = server.playerManager.Players[playerNum].transform.position.x;
            responseData.PosY = server.playerManager.Players[playerNum].transform.position.y;
        }
        else
        {
            responseData.acceptance = false;
        }
        
        S3_GameMessage toSend = new S3_GameMessage
        {
            PlayerNum = (byte)playerNum,
            SendTime = Time.time,
            MessageType = S3_GameMessageType.ServerConnectResponse,
            MessageData = responseData
        };

        server.SendGameMessage( toSend );
    }

    private void HandleResponseAck(S3_GameMessage message)
    {
        S3_ServerTimeData data = new S3_ServerTimeData
        {
            time = Time.time
        };

        S3_GameMessage toSend = new S3_GameMessage
        {
            PlayerNum = message.PlayerNum,
            SendTime = Time.time,
            MessageType = S3_GameMessageType.ServerTime,
            MessageData = data
        };

        
        server.playerManager.TimeOffsets[(int)(message.PlayerNum)] = toSend.SendTime;
        server.SendGameMessage( toSend );
        for(int i = 0; i < server.playerManager.CurrentPlayers; ++i)
        {
            if( i != (int)(message.PlayerNum))
            {
                S3_ServerNewPlayerData newData = new S3_ServerNewPlayerData
                {
                    PlayerName = "Jack",
                    PlayerNum = message.PlayerNum,
                    PosX = server.playerManager.SpawnPoints[(int)( message.PlayerNum )].transform.position.x,
                    PosY = server.playerManager.SpawnPoints[(int)( message.PlayerNum )].transform.position.y
                };
                S3_GameMessage newSend = new S3_GameMessage
                {
                    SendTime = Time.time,
                    MessageData = newData,
                    MessageType = S3_GameMessageType.ServerNewPlayer,
                    PlayerNum = (byte)i
                };
                server.SendGameMessage( newSend );
            }
        }
    }

    private void HandleTimeReceived(S3_GameMessage message)
    {
        //needs to be divided by 2
        server.playerManager.TimeOffsets[(int)( message.PlayerNum )] = Time.time - server.playerManager.TimeOffsets[(int)( message.PlayerNum )];

        S3_ServerTimeOffsetData data = new S3_ServerTimeOffsetData
        {
            offset = server.playerManager.TimeOffsets[(int)( message.PlayerNum )]
        };
        S3_GameMessage toSend = new S3_GameMessage
        {
            PlayerNum = message.PlayerNum,
            SendTime = Time.time,
            MessageType = S3_GameMessageType.ServerTimeOffset,
            MessageData = data
        };

        server.SendGameMessage( toSend );
    }

    private void HandlePosDR(S3_GameMessage message)
    {
        S3_ClientPosDRData posDRData = (S3_ClientPosDRData)( message.MessageData );
        int playerNum = (int)( message.PlayerNum );
        Vector3 oldPos = new Vector3( posDRData.DRPosX, posDRData.DRPosY );
        server.playerManager.Players[playerNum].GetComponent<Rigidbody2D>().velocity = new Vector2( posDRData.DRVelX, posDRData.DRVelY );
        Vector3 vel = new Vector3( server.playerManager.Players[playerNum].GetComponent<Rigidbody2D>().velocity.x,
            server.playerManager.Players[playerNum].GetComponent<Rigidbody2D>().velocity.y );
        Vector3 newPos = oldPos + vel * ( Time.time - message.SendTime );
        server.playerManager.Players[playerNum].transform.position = newPos;

        for(int i = 0; i < server.playerManager.CurrentPlayers; ++i)
        {
            if( i != message.PlayerNum )
            {
                S3_ServerPlayerPosDRData data = new S3_ServerPlayerPosDRData
                {
                    InitialTime = message.SendTime,
                    PlayerNum = message.PlayerNum,
                    DRPosX = posDRData.DRPosX,
                    DRPosY = posDRData.DRPosY,
                    DRVelX = posDRData.DRVelX,
                    DRVelY = posDRData.DRVelY,
                    DRAngle = 0f
                };
                S3_GameMessage toSend = new S3_GameMessage
                {
                    PlayerNum = (byte)i,
                    MessageType = S3_GameMessageType.ServerPlayerPosDR,
                    SendTime = Time.time,
                    MessageData = data
                };
                server.SendGameMessage( toSend );
            }
        }
        
    }

    private void HandleRotDR(S3_GameMessage message)
    {
        
        S3_ClientRotDRData data = (S3_ClientRotDRData)( message.MessageData );
        Debug.Log(string.Format("Received angle {0} from player {1}", data.Angle, message.PlayerNum));
        int playerNum = (int)( message.PlayerNum );
        Vector3 currentRot = server.playerManager.Players[playerNum].transform.eulerAngles;
        server.playerManager.Players[playerNum].transform.eulerAngles = new Vector3(
            currentRot.x, currentRot.y, data.Angle );

        for(int i = 0; i < server.playerManager.CurrentPlayers; ++i)
        {
            if( i != message.PlayerNum )
            {
                S3_ServerPlayerRotDRData newData = new S3_ServerPlayerRotDRData
                {
                    DRAngle = data.Angle,
                    PlayerNum = message.PlayerNum
                };
                S3_GameMessage toSend = new S3_GameMessage
                {
                    PlayerNum = (byte)i,
                    MessageType = S3_GameMessageType.ServerPlayerRotDR,
                    SendTime = Time.time,
                    MessageData = newData
                };

                server.SendGameMessage( toSend );
            }
        }
    }

    private void HandleSwing(S3_GameMessage message)
    {

    }

    private void HandleSwitch(S3_GameMessage message)
    {
        return;
    }
}
