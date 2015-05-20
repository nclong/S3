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
    public string PlayerName;
    public GameObject[] Players;
    public GameObject ThePlayer;

    private UdpClient player;
    public int PlayerNum = -1;

    private S3_MessagesQueue ReceiveQueue;
    private S3_MessagesQueue SendQueue;
    private S3_ClientMessageProcessor MessageProcessor;
    IPEndPoint HostEndPoint;
    IPEndPoint ipRemoteEP;
    void Start()
    {
        ReceiveQueue = new S3_MessagesQueue();
        SendQueue = new S3_MessagesQueue();
        MessageProcessor = GetComponent<S3_ClientMessageProcessor>();
        Players = new GameObject[4];
        StartClient();
    }

    public void StartClient()
    {
        

        //remember that UdpClient *is* a socket
        player = new UdpClient(3501);

        byte[] gamePlayData = new byte[1024];
        HostEndPoint = new IPEndPoint( IPAddress.Parse( HostIP ), 3500 );

        try
        {
            IPHostEntry ipHostInfo = Dns.GetHostEntry( Dns.GetHostName() );
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            player.Connect( HostEndPoint );
            S3_ClientConnectRequestData request = new S3_ClientConnectRequestData
            {
                playerName = PlayerName,
                ip = ipAddress.GetAddressBytes()
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
            player.Send( gamePlayData, gamePlayData.Length );
            Debug.Log( "Data Sent" );

            //make an endpoint that is able to read anything the server sends
             ipRemoteEP = new IPEndPoint(IPAddress.Any, 0);
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
    }

    public void StopClient()
    {
        player.Close();
    }

    void FixedUpdate()
    {
        //Receive Messages
        S3_StateObject state = new S3_StateObject()
        {
            socket = player,
            endPoint = ipRemoteEP
        };

        player.BeginReceive( new AsyncCallback(ReceiveCallback), state );

        //SendMessages if there are
        SendMessages();
        ReadMessages();
    }


    void ReadMessages()
    {
        while( !ReceiveQueue.IsEmpty )
        {
            MessageProcessor.ProcessMessage( ReceiveQueue.GetMessage() );
        }
    }


    private void SendMessages()
    {
        while( !SendQueue.IsEmpty )
        {
            S3_GameMessage toSend = SendQueue.GetMessage();
            byte[] bytesData = S3_MessageFormatter.GameMessageToBytes( toSend );
            S3_StateObject state = new S3_StateObject()
            {
                socket = player,
                buffer = bytesData
            };
            player.BeginSend( state.buffer, state.buffer.Length, new AsyncCallback(SendCallback), state );
        }
    }

    public void SendGameMessage( S3_GameMessage msg )
    {
        SendQueue.AddMessage( msg );
    }

    private void SendCallback( IAsyncResult result )
    {
        S3_StateObject state = (S3_StateObject)result.AsyncState;
        int sendResult = state.socket.EndSend( result );
        Debug.Log( "Send state: " + sendResult );
    }
    private void ReceiveCallback( IAsyncResult result )
    {
        Debug.Log( "ReceiveCallback called" );
        S3_StateObject state = (S3_StateObject)result.AsyncState;
        byte[] data = state.socket.EndReceive( result, ref HostEndPoint );
        if( data.Length > 0 )
        {
            ReceiveQueue.AddMessage( S3_MessageFormatter.BytesToGameMessage( data ) );
        }
    }
}
