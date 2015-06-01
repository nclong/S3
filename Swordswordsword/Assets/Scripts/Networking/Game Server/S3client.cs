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
    public GameObject ThePlayer;
    public S3_ClientPlayerManager playerManager;
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
        playerManager = GetComponent<S3_ClientPlayerManager>();
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
            string url = "http://checkip.dyndns.org";
            WebRequest req = System.Net.WebRequest.Create( url );
            WebResponse resp = req.GetResponse();
            StreamReader sr = new System.IO.StreamReader( resp.GetResponseStream() );
            string response = sr.ReadToEnd().Trim();
            string[] a = response.Split( ':' );
            string a2 = a[1].Substring( 1 );
            string[] a3 = a2.Split( '<' );
            string a4 = a3[0];

            IPAddress ipAddress = IPAddress.Parse( a4 );
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
            S3_StateObject state = new S3_StateObject()
            {
                socket = player,
                endPoint = HostEndPoint
            };

            player.BeginReceive(new AsyncCallback(ReceiveCallback), state);
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
                buffer = bytesData,
                endPoint = HostEndPoint,
                message = toSend
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
        if( state.message.MessageType == S3_GameMessageType.ClientRotDR)
        {
            for(int i = 0; i < state.buffer.Length; ++i)
            {
                Debug.Log(state.buffer[i]);
            }
        }
        Debug.Log(String.Format("Sent message of type {0}", state.message.MessageType));
        Debug.Log( "Send state: " + sendResult );
        Debug.Log("Buffer Size: " + state.buffer.Length);
    }
    private void ReceiveCallback( IAsyncResult result )
    {
        Debug.Log( "ReceiveCallback called" );
        S3_StateObject state = (S3_StateObject)result.AsyncState;
        state.buffer = state.socket.EndReceive( result, ref state.endPoint );
        S3_StateObject newState = new S3_StateObject()
        {
            socket = player,
            endPoint = HostEndPoint
        };

        player.BeginReceive(new AsyncCallback(ReceiveCallback), newState);
        if( state.buffer.Length > 0 )
        {
            state.message = S3_MessageFormatter.BytesToGameMessage(state.buffer);
            ReceiveQueue.AddMessage( state.message );
            Debug.Log(String.Format("Received message of type {0}", state.message.MessageType ));
        }


    }
}
