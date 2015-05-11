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

    public static void StartClient()
    {
        //remember that UdpClient *is* a socket
        UdpClient player = new UdpClient(3000);

        byte[] gamePlayData = new byte[1024];
        string name = "Teddy Tester";

        try
        {

            //prepare player info to be sent to server
            gamePlayData = Encoding.ASCII.GetBytes(name);

            //send info out
            player.Send(gamePlayData, gamePlayData.Length);

            //make an endpoint that is able to read anything the server sends
            IPEndPoint ipRemoteEP = new IPEndPoint(IPAddress.Any, 0);

            byte[] gpdBytes = player.Receive(ref ipRemoteEP);
            string gameState = Encoding.ASCII.GetString(gpdBytes);

            player.Close();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }

    void SendRequest()
    {
    
    }

    void RecieveResponse()
    { 
    
    }

    public static int Main()
    {
        //Using UDP methodology
        StartClient();
        return 0;
    }
}
