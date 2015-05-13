using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using UnityEngine;

public class S3client : MonoBehaviour
{
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    //[STAThread]

    public string HostIP;

    void Start()
    {
        StartClient();
    }

    public void StartClient()
    {
        //remember that UdpClient *is* a socket
        UdpClient player = new UdpClient(3500);

        byte[] gamePlayData = new byte[1024];
        string name = "Teddy Tester";
        IPEndPoint HostEndPoint = new IPEndPoint( IPAddress.Parse( HostIP ), 3500 );

        try
        {
            S3_ClientConnectRequestData request = new S3_ClientConnectRequestData
            {
                playerName = name
            };
            S3_GameMessage message = new S3_GameMessage
            {
                SendTime = Time.time,
                MessageType = S3_GameMessageType.ClientConnectRequest,
                MessageData = request
            };
            //prepare player info to be sent to server
            gamePlayData = S3_MessageFormatter.GameMessageToBytes( message );

            //send info out
            player.Send(gamePlayData, gamePlayData.Length);
            Debug.Log( "Data Sent" );

            //make an endpoint that is able to read anything the server sends
            IPEndPoint ipRemoteEP = new IPEndPoint(IPAddress.Any, 0);

            player.Close();
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
    }

    void SendRequest()
    {
    
    }

    void RecieveResponse()
    { 
    
    }

    //public static int Main()
    //{
    //    //Using UDP methodology
    //    StartClient();
    //    return 0;
    //}
}
