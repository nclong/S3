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
    const int BUFFER_SIZE = 1024;
    const int PLAYER_CAP = 4; //including "host"

    byte[] data;
    UdpClient newsock;
    IPEndPoint sender;
    Queue receiveQ;
    Queue toSendQ;

    void Start()
    {
        data = new byte[1024];
        ep = new IPEndPoint(IPAddress.Any, 3500);
        newsock = new UdpClient(ep);
        server.serverPort = new UdpClient(3500);
        ep = null;
        sender = new IPEndPoint(IPAddress.Any, 0);
        receiveQ = new Queue();
        toSendQ = new Queue();

        Thread receiveThread = new Thread(delegate()
        {
            while (true)
            {
                data = newsock.Receive(ref sender);
                if(data.Length > 0)
                    receiveQ.Enqueue(data);
            }
        });

        Thread sendThread = new Thread(delegate()
        {
            while (true)
            {
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
        SendMessage();
    }
    
    void ReadMessage()
    {
        
    }

    void SendMessage()
    {
        
    }

    public void AcceptOrRejectRequest()
    { 
        
    }
}

