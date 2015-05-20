using UnityEngine;
using System.Collections;

public class S3_ClientMessageProcessor : MonoBehaviour {
    private S3client client;
    public GameObject ServerPlayer;
	// Use this for initialization
	void Start () {
        client = GetComponent<S3client>();
	}

    public void ProcessMessage( S3_GameMessage message )
    {
        switch( message.MessageType )
        {
            case S3_GameMessageType.ServerConnectResponse:
                HandleServerConnectResponse( message ); break;
            case S3_GameMessageType.ServerTime:
                HandleServerTime( message ); break;
            case S3_GameMessageType.ServerTimeOffset:
                HandleServerTimeOffset( message ); break;
            case S3_GameMessageType.ServerPlayerPosDR:
                HandleServerPlayerPosDR( message ); break;
            case S3_GameMessageType.ServerPlayerRotDR:
                HandleServerPlayerRotDR( message ); break;
            case S3_GameMessageType.ServerPlayerDied:
                HandleServerPlayerDied( message ); break;
            case S3_GameMessageType.ServerPlayerHit:
            case S3_GameMessageType.ServerPlayerSwing:
                HandleServerPlayerSwing( message ); break;
            case S3_GameMessageType.ServerPlayerSwitch:
            case S3_GameMessageType.ServerStartGame:
            case S3_GameMessageType.ServerNewPlayer:
                HandleServerNewPlayer( message ); break;
            case S3_GameMessageType.ClientConnectRequest:
            case S3_GameMessageType.ClientResponseAck:
            case S3_GameMessageType.ClientTimeReceived:
            case S3_GameMessageType.ClientPosDR:
            case S3_GameMessageType.ClientRotDR:
            case S3_GameMessageType.ClientSwing:
            case S3_GameMessageType.ClientSwitch:
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

    private void HandleServerConnectResponse(S3_GameMessage message)
    {
        S3_ServerConnectResponseData responseData = (S3_ServerConnectResponseData)( message.MessageData );
        if( responseData.acceptance )
        {
            client.PlayerNum = (int)( message.PlayerNum );
            client.Players[client.PlayerNum] = client.ThePlayer;
            client.ThePlayer.transform.position = new Vector3( responseData.PosX, responseData.PosY );

            S3_ClientResponseAckData data = new S3_ClientResponseAckData
            {
                ack = true
            };
            S3_GameMessage toSend = new S3_GameMessage
            {
                PlayerNum = (byte)( client.PlayerNum ),
                SendTime = 0f,
                MessageData = data,
                MessageType = S3_GameMessageType.ClientResponseAck
            };

            client.SendGameMessage( toSend );
        }
    }

    private void HandleServerTime(S3_GameMessage message)
    {
        S3_ServerTime.SetTime( message.SendTime );
        S3_ClientTimeReceivedData data = new S3_ClientTimeReceivedData
        {
            acceptance = true
        };
        S3_GameMessage toSend = new S3_GameMessage
        {
            PlayerNum = (byte)( client.PlayerNum ),
            SendTime = 0f,
            MessageData = data,
            MessageType = S3_GameMessageType.ClientTimeReceived
        };

        client.SendGameMessage( toSend );
    }

    private void HandleServerTimeOffset(S3_GameMessage message)
    {
        S3_ServerTimeOffsetData offsetData = (S3_ServerTimeOffsetData)(message.MessageData);
        S3_ServerTime.SetOffset( offsetData.offset );
    }

    private void HandleServerPlayerPosDR(S3_GameMessage message)
    {
        S3_ServerPlayerPosDRData data = (S3_ServerPlayerPosDRData)(message.MessageData);
        int playerNum = (int)( data.PlayerNum );
        Vector3 oldPos = new Vector3( data.DRPosX, data.DRPosY );
        Vector3 vel = new Vector3( data.DRVelX, data.DRVelY );
        Vector3 newPos = oldPos + new Vector3( vel.x, vel.y ) * ( S3_ServerTime.ServerTime - data.InitialTime );
        client.Players[playerNum].transform.position = newPos;
        client.Players[playerNum].GetComponent<Rigidbody2D>().velocity = new Vector2( vel.x, vel.y );
    }
    private void HandleServerPlayerRotDR(S3_GameMessage message)
    {
        S3_ServerPlayerRotDRData data = (S3_ServerPlayerRotDRData)( message.MessageData );
        int playerNum = (int)( data.PlayerNum );
        client.Players[playerNum].transform.eulerAngles = new Vector3( 0f, 0f, data.DRAngle );
    }
    private void HandleServerPlayerDied(S3_GameMessage message)
    {
        S3_ServerPlayerDiedData data = (S3_ServerPlayerDiedData)( message.MessageData );
        int playerNum = (int)(data.PlayerNum);
    }
    private void HandleServerPlayerHit(S3_GameMessage message){}
    private void HandleServerPlayerSwing(S3_GameMessage message)
    {
        S3_ServerPlayerSwingData data = (S3_ServerPlayerSwingData)( message.MessageData );
        int playerNum = (int)data.PlayerNum;
        client.Players[playerNum].GetComponent<S3_CombatStateController>().SwingSword();
    }
    private void HandleServerPlayerSwitch(S3_GameMessage message){}
    private void HandleServerStartGame(S3_GameMessage message){}
    private void HandleServerNewPlayer(S3_GameMessage message) 
    {
        S3_ServerNewPlayerData newPData = (S3_ServerNewPlayerData)(message.MessageData);
        int playerNum = (int)( newPData.PlayerNum );
        client.Players[playerNum] = Instantiate<GameObject>( ServerPlayer );
        client.Players[playerNum].transform.position = new Vector3( newPData.PosX, newPData.PosY );
    
    }









}
