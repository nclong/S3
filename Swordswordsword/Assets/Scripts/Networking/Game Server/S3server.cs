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
}

public class S3server : MonoBehaviour
{
    //GameServer server;
    IPEndPoint ep;
    S3_GameMessage messageType;
    S3_GameMessageType t;
    const int BUFFER_SIZE = 1024;
    const int PLAYER_CAP = 4; //including "host"

    byte[] data;
    UdpClient newsock;
    IPEndPoint sender;
    S3_MessagesQueue receiveQ;
    S3_MessagesQueue toSendQ;

    void Start()
    {
        data = new byte[1024];
        ep = new IPEndPoint(IPAddress.Any, 3500);
        newsock = new UdpClient(ep);
        //server.serverPort = new UdpClient(3500);
        ep = null;
        sender = new IPEndPoint(IPAddress.Any, 0);
        receiveQ = new S3_MessagesQueue();
        toSendQ = new S3_MessagesQueue();

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
        ReadMessage();
    }

    void FixedUpdate()
    {
        S3_StateObject state = new S3_StateObject()
        {
            socket = newsock
        };
        newsock.BeginReceive( ReceiveCallback, state );
    }

    void LateUpdate()
    {
    }
    
    void ReadMessage()
    {
        
    }

    public void SendMessage(S3_GameMessage msg)
    {
        toSendQ.AddMessage( msg );
    }

    public void AcceptOrRejectRequest()
    { 
        
    }

    private void ReceiveCallback(IAsyncResult result)
    {
        Debug.Log( "ReceiveCallback called" );
        S3_StateObject state = (S3_StateObject)result.AsyncState;
        data = state.socket.EndReceive( result, ref ep );
        Debug.Log( "Message received: " + data.ToString() );
        if( data.Length > 0)
        {
            receiveQ.AddMessage( S3_MessageFormatter.BytesToGameMessage( data ) );
        }
     
    }

}

