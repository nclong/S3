using System;
using System.IO;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Configuration;
using System.Collections.Generic;
using Newtonsoft.Json;


public class S3UDP
{
    //to be implemented later
    class Packet
    {
        [JsonProperty]
        public PacketType type;
        //need a unique ID represented in bytes
        [JsonProperty]
        public byte ID;
        [JsonProperty]
        public string userName; 
        /*float[] position;
        float[] velocity;
        int health;*/
    }

    class HandShakePacket : Packet
    {
        [JsonProperty]
        public int connectStatus;
    }

    //accept no more than 30 players in lobby, no more than 8 chars in userName
    const int PLAYER_LIMIT = 30;
    const int CHAR_LIMIT = 8;

    //a concurrent list of players logged in
    public static Dictionary<string,byte> players = new Dictionary<string,byte>();
    
    //[STAThread]
    public static void RejectCallBack(/*IAsyncResult ar*/)
    {
        //sorry, can't get anymore players in
    }

    //Client Handler?
    public static void StartClient()
    {
        //remember that UdpClient *is* a socket
        UdpClient player = new UdpClient(3000);

        /*byte[] gamePlayData = new byte[1024];
        string name = "Teddy Tester";*/

        try
        {
            //prepare player info to be sent to server
            Packet p = new Packet();
            //gamePlayData = Encoding.ASCII.GetBytes(name);
            string playerData = JsonConvert.SerializeObject(p);
            byte[] pdBytes = Encoding.ASCII.GetBytes(playerData);

            //send info out
            player.Send(pdBytes, pdBytes.Length);

            //make an endpoint that is able to read anything the server sends
            IPEndPoint ipRemoteEP = new IPEndPoint(IPAddress.Any, 0);

            byte[] pdrBytes = player.Receive(ref ipRemoteEP);
            string lobbyState = Encoding.ASCII.GetString(pdrBytes);

            player.Close();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }

    //Master Server
    public static void StartListening()
    {
        //UdpClient is a socket type
        UdpClient incPlayer = new UdpClient(2545);
        IPEndPoint endPt = new IPEndPoint(IPAddress.Any, 2545);
        byte[] gameData = new byte[1024];

        IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);

        //begin thread
        Thread mainGame = new Thread(delegate()
        {
            while (true)
            {
                //recieve game data from all players.
                gameData = incPlayer.Receive(ref sender);
                //check if player is trying to connect or not


                //toss out excess data before translating bytes to string
                //incPlayer.BeginReceive(new AsyncCallback(RejectCallBack), gameData);
                

                //give server time to break before sending another update
                System.Threading.Thread.Sleep(1000);
                Packet p = JsonConvert.DeserializeObject<Packet>(Encoding.ASCII.GetString(gameData, 0, gameData.Length));

                //the master server will handle what type of packets it is recieving
                if( p.type == PacketType.handShake)
                {
                    //create handshaker
                    HandShakePacket hs = (HandShakePacket)p;
                    //look ahead to see if adding another player to the game will exceed player limit
                    if (players.Count + 1 > PLAYER_LIMIT)
                    {
                        //if so, reject the connection request
                        RejectCallBack();
                    }
                    else
                    { 
                        //if not, add him/her in the lobby!
                        players.Add(hs.userName, hs.ID);
                    }
                }
                else if (p.type == PacketType.accountRegister)
                { 
                    //if userName does not exist, add in to database
                }
                else if (p.type == PacketType.accountLogin)
                { 
                    //if userName does exit, allow player to login to lobby
                }

                /*//resolve game state using game data (possiblly w/ database interaction)
                string gdResolution; //does nothing at the moment, need database access*/

                //assemble resolved game state into datagram
                //byte[] gdBytes = Encoding.ASCII.GetBytes(gdString);

                //send datagram to all players
                incPlayer.Send(gameData, gameData.Length, sender);
            }
          });
        mainGame.Start();

    }

    public static int Main()
    {
        //using UDP methodology
        StartListening();
        StartClient();
        return 0;
    }
}
