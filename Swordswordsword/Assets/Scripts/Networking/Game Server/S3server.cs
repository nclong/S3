using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Configuration;
using UnityEngine;

public class GameServer
{
    public UdpClient serverPort;
    public bool hasPassword = false;
    public string password = "";
    public int passHash = 0;

    public byte[] gameData;
    public byte[] gameDataGram;
}

public class S3server
{
    const int BUFFER_SIZE = 1024;

    public void UDPServerStart()
    {
        //a default made server is assumed to have no password
        GameServer server = new GameServer();
        server.serverPort = new UdpClient(3500);
        IPEndPoint ep = null;
        
        //before starting, load level first

        while (true)
        {
            byte[] gameData = server.serverPort.Receive(ref ep);

            //do stuff with it here
            //put all the compiled data in a string
            string decisions = "";
            
            byte[] gameDataGram = Encoding.ASCII.GetBytes(decisions);
            server.serverPort.Send(gameDataGram, gameDataGram.Length);
        }
    }
   public int portNum = 2545;
    const int PLAYER_LIMIT = 3;

    public int Main()
    {
        UDPServerStart();
        return 0;
    }


}

