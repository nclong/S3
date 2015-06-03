using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Configuration;
using System.Collections;
using UnityEngine;
using SimpleJSON;

public class S3_StateObject
{
    public UdpClient socket = null;
    public const int BufferSize = 1024;
    public byte[] buffer = new byte[BufferSize];
    public S3_GameMessage message = new S3_GameMessage();
    public IPEndPoint endPoint = null;
    

}

public class S3server : MonoBehaviour
{
    //GameServer server;
    IPEndPoint ep;
    const int BUFFER_SIZE = 1024;
    const int PLAYER_CAP = 4; //including "host"

    byte[] data;
    UdpClient newsock;
    IPEndPoint sender;
    S3_MessagesQueue receiveQ;
    S3_MessagesQueue toSendQ;

    public GameObject ServerPlayerManagerObj;
    public S3_ServerPlayerManager playerManager;
    private S3_GameManager gameManager;

    S3_HostMessageProcessor messageProcessor;

    public string MasterServerIp;
    S3_MasterServerClient masterServer = new S3_MasterServerClient();

    public float InfoUpdateTick = 1000;
    public float InfoUpdateTimer = 0f;

    void Start()
    {
        messageProcessor = GetComponent<S3_HostMessageProcessor>();
        playerManager = ServerPlayerManagerObj.GetComponent<S3_ServerPlayerManager>();
        gameManager = ServerPlayerManagerObj.GetComponent<S3_GameManager>();

        masterServer.StartClient( MasterServerIp );

        data = new byte[1024];
        ep = new IPEndPoint(IPAddress.Any, 3500);

        newsock = new UdpClient(3500);
        //server.serverPort = new UdpClient(3500);
        sender = new IPEndPoint(IPAddress.Any, 0);
        receiveQ = new S3_MessagesQueue();
        toSendQ = new S3_MessagesQueue();

        S3_StateObject state = new S3_StateObject()
        {
            socket = newsock,
            endPoint = sender
        };
        newsock.BeginReceive(new AsyncCallback(ReceiveCallback), state);
   
    }

    void Update()
    {

        InfoUpdateTimer += Time.deltaTime;
        if(InfoUpdateTimer  >= InfoUpdateTick )
        {
            //Send info to clients
            S3_ServerPlayerInfoData data = new S3_ServerPlayerInfoData
            {
                scores = gameManager.PlayerScores
            };
            data.pings = new float[4];
            for( int i = 0; i < playerManager.CurrentPlayers; ++i )
            {
                data.pings[i] = playerManager.Latencies[i].GetMeanF();
            }
            for( int i = 0; i < playerManager.CurrentPlayers; ++ i )
            {
                S3_GameMessage message = new S3_GameMessage
                {
                    MessageType = S3_GameMessageType.ServerPlayerInfo,
                    MessageData = data,
                    SendTime = Time.time,
                    PlayerNum = (byte)i
                };
            }
                
                //Send info to login server
                InfoUpdateTimer = 0f;
        }
    }

    void FixedUpdate()
    {
        SendMessages();
        ReadMessages();
		SendTimeUpdates ();
    }

    void LateUpdate()
    {
    }

	void SendTimeUpdates()
	{
		for (int i = 0; i < playerManager.CurrentPlayers; ++i) {
			if(!playerManager.TimeRequestSent[i])
			{
				playerManager.TimeOffsets[i] = Time.time;
				S3_ServerTimeData data = new S3_ServerTimeData
				{
					time = playerManager.TimeOffsets[i]
				};
				S3_GameMessage message = new S3_GameMessage
				{
					PlayerNum = (byte)i,
					SendTime = playerManager.TimeOffsets[i],
					MessageData = data,
					MessageType = S3_GameMessageType.ServerTime
				};
				
				SendGameMessage(message);
				playerManager.TimeRequestSent[i] = true;
			}
		}
	}
    
    void ReadMessages()
    {
        while(!receiveQ.IsEmpty)
        {
            messageProcessor.ProcessMessage( receiveQ.GetMessage() );
        }
    }

    private void SendMessages()
    {
        while( !toSendQ.IsEmpty )
        {
            S3_GameMessage toSend = toSendQ.GetMessage();
            byte[] bytesData = S3_MessageFormatter.GameMessageToBytes(toSend);
            S3_StateObject state = new S3_StateObject()
            {
                socket = newsock,
                buffer = bytesData,
                endPoint = playerManager.PlayerEndPoints[(int)toSend.PlayerNum],
                message = toSend
            };
            newsock.BeginSend(bytesData, bytesData.Length, state.endPoint, new AsyncCallback(SendCallback), state);
        }
    }

    public void SendGameMessage(S3_GameMessage msg)
    {
        toSendQ.AddMessage( msg );
    }

    private void SendCallback(IAsyncResult result)
    {
        S3_StateObject state = (S3_StateObject)(result.AsyncState);
        int sendResult = state.socket.EndSend( result );
        Debug.Log( "Send state: " + sendResult );
        Debug.Log(String.Format("Sending message of type {0} to player {1}", state.message.MessageType, state.message.PlayerNum));
    }

    private void ReceiveCallback(IAsyncResult result)
    {


        S3_StateObject state = (S3_StateObject)result.AsyncState;
        
        state.buffer = state.socket.EndReceive( result, ref state.endPoint );
        S3_StateObject newState = new S3_StateObject()
        {
            socket = newsock,
            endPoint = sender
        };
        newsock.BeginReceive(new AsyncCallback(ReceiveCallback), newState);

        if (state.buffer.Length > 0)
        {
            state.message = S3_MessageFormatter.BytesToGameMessage(state.buffer);
            Debug.Log(String.Format("Received message of type {0} from player {1}", state.message.MessageType, state.message.PlayerNum));
            receiveQ.AddMessage(state.message);
        }
        else
        {
            Debug.Log("Empty Buffer Received");
        }


    }
}

