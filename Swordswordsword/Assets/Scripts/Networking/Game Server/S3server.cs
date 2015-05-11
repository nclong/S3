using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Configuration;
using System.Collections;
using UnityEngine;

public class GameServer
{
    public UdpClient serverPort;
    public bool hasPassword = false;
    public string password = "";
    public int passHash = 0;

    public Time time;
    /*public byte[] gameData;
    public string decisions;
    public byte[] gameDataGram;*/

    int numPlayers;
    
}

public class S3server : MonoBehaviour
{
    GameServer server;
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
        server.serverPort = new UdpClient(3500);
        ep = null;
        sender = new IPEndPoint(IPAddress.Any, 0);
        receiveQ = new S3_MessagesQueue();
        toSendQ = new S3_MessagesQueue();

        //Receive Thread
        Thread receiveThread = new Thread(delegate()
        {
            while (true)
            {
                data = newsock.Receive(ref sender);
                S3_ServeTimeData serverTimeData = new S3_ServeTimeData
                {
                    time = S3_ServerTime.ServerTime
                };
                messageType = new S3_GameMessage(S3_ServerTime.ServerTime, S3_GameMessageType.ServerTime, serverTimeData);
                receiveQ.AddMessage(messageType);
            }
        });


        //Send Thread
        Thread sendThread = new Thread(delegate()
        {
            while (true)
            {
                toSendQ.GetMessage();
                //determine what kind of message is the server sending
            }
        });
    }

    void Update()
    {
        ReadMessage();
    }

    void FixedUpdate()
    { 
    
    }

    void LateUpdate()
    {
        SendMessage(messageType);
    }
    
    void ReadMessage()
    {
        
    }

    void SendMessage(S3_GameMessage msg)
    {
        toSendQ.AddMessage(msg);
    }

    public void AcceptOrRejectRequest()
    { 
        
    }
}

