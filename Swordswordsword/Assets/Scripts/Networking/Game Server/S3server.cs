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

    S3_HostMessageProcessor messageProcessor;

    public IPEndPoint[] PlayerEndPoints;
    public string[] PlayerNames;

    void Start()
    {
        messageProcessor = GetComponent<S3_HostMessageProcessor>();
        playerManager = ServerPlayerManagerObj.GetComponent<S3_ServerPlayerManager>();
        PlayerEndPoints = new IPEndPoint[4];
        PlayerNames = new string[4];

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

        //Receive Thread
        //Thread receiveThread = new Thread(delegate()
        //{
        //    while (true)
        //    {
        //        data = newsock.Receive(ref sender);
        //        receiveQ.AddMessage(S3_MessageFormatter.BytesToGameMessage(data));
        //    }
        //});


        ////Send Thread
        //Thread sendThread = new Thread(delegate()
        //{
        //    while (true)
        //    {
        //        //determine what kind of message is the server sending
        //    }
        //});

        //receiveThread.Start();
   
    }

    void Update()
    {
        
    }

    void FixedUpdate()
    {
        //Receive Messages

        //Move to the callback

        //SendMessages if there are any
        SendMessages();
        ReadMessages();
    }

    void LateUpdate()
    {
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
                socket = newsock
            };
            newsock.BeginSend(bytesData, bytesData.Length, PlayerEndPoints[toSend.PlayerNum], new AsyncCallback(SendCallback), state);
        }
    }

    public void SendGameMessage(S3_GameMessage msg)
    {
        toSendQ.AddMessage( msg );
    }

    private void SendCallback(IAsyncResult result)
    {
        S3_StateObject state = (S3_StateObject)result.AsyncState;
        int sendResult = state.socket.EndSend( result );
        Debug.Log( "Send state: " + sendResult );
    }
    private void ReceiveCallback(IAsyncResult result)
    {
        S3_StateObject state = (S3_StateObject)result.AsyncState;
        state.buffer = state.socket.EndReceive( result, ref state.endPoint );
        if( state.buffer.Length > 0)
        {
            state.message = S3_MessageFormatter.BytesToGameMessage( state.buffer );
            receiveQ.AddMessage( state.message );
        }
        for(int i = 0; i < state.buffer.Length; ++i )
        {
            Debug.Log(state.buffer[i]);
        }

        S3_StateObject newState = new S3_StateObject()
        {
            socket = newsock,
            endPoint = sender
        };
        newsock.BeginReceive(new AsyncCallback(ReceiveCallback), newState);
    }
}

