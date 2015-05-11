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

    public Time time;
    /*public byte[] gameData;
    public string decisions;
    public byte[] gameDataGram;*/

    int numPlayers;
    
}

public class S3server
{
    const int BUFFER_SIZE = 1024;
    const int PLAYER_CAP = 4; //including "host"

    public void UDPServerStart()
    {
        //a default made server is assumed to have no password and no players
        GameServer server = new GameServer();
        server.serverPort = new UdpClient(3500);
        IPEndPoint ep = null;
        


        //before starting, load level first

        //again before starting a game, listen and do handshake with clients

        //game starts once all 4 players are in. should probably wait 5 seconds
        while (true)
        {
            //determine the type of message it receives
            //if a player is trying to connect, handle that
            //by rejecting or accepting connection

            //else it is just the player(s) that are in game
            //so server has to handle that.
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

    public void AcceptOrRejectRequest()
    { 
        
    }

    public int Main()
    {
        UDPServerStart();
        return 0;
    }


}

