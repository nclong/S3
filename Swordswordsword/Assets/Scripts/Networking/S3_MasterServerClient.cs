using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using UnityEngine;
using SimpleJSON;

// State object for receiving data from remote device.
public class StateObject
{
    // Client socket.
    public Socket workSocket = null;
    // Size of receive buffer.
    public const int BufferSize = 256;
    // Receive buffer.
    public byte[] buffer = new byte[BufferSize];
    // Received data string.
    public StringBuilder sb = new StringBuilder();

}

public class S3_MasterServerClient
{
    // The port number for the remote device.
    private const int port = 11000;

    // ManualResetEvent instances signal completion.
    private static ManualResetEvent connectDone =
        new ManualResetEvent( false );

    private static ManualResetEvent receiveDone =
        new ManualResetEvent(false);

    // Create a TCP/IP socket.
    private Socket client;

    private static S3DataResponse DataResponse;

    
    public void StartClient(string hostIp)
    {
        // Connect to a remote device.
        try
        {
            client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            // Establish the remote endpoint for the socket.
            // The name of the 
            // remote device is "host.contoso.com".
            IPHostEntry ipHostInfo = Dns.GetHostEntry( hostIp );
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPEndPoint remoteEP = new IPEndPoint( ipAddress, port );



            // Connect to the remote endpoint.
            client.BeginConnect( remoteEP,
                new AsyncCallback( ConnectCallback ), client );
            connectDone.WaitOne( 15000 );
            connectDone.Reset();
            //response = "Connection created";

            // Send test data to the remote device.
            //Send( client, "This is a test<EOF>" );
            //sendDone.WaitOne();

            //System.Threading.Thread.Sleep( 1000 );
            //// Receive the response from the remote device.
            //Receive( client );
            //receiveDone.WaitOne();

            // Write the response to the console.
            //Debug.Log( "Response received : " + response );

            // Release the socket.


        }
        catch( Exception e )
        {
            Console.WriteLine( e.ToString() );
        }
    }

    private static int IpStringToInt( string ip )
    {
        IPAddress ipAddr = IPAddress.Parse( ip );
        byte[] ipBytes = ipAddr.GetAddressBytes();
        return BitConverter.ToInt32( ipBytes, 0 );
    }

    private static string IntToIpString( int ipInt )
    {
        byte[] ipBytes = BitConverter.GetBytes( ipInt );
        IPAddress ip = new IPAddress( ipBytes );
        return ip.ToString();
    }

    public void SendServerUpdate(string name, int players, string ip)
    {
        JSONClass InfoJson = new JSONClass();
        InfoJson["UserName"] = name;
        InfoJson["passwordHash"].AsInt = players;
        InfoJson["type"] = ip;
        Send( client, InfoJson.ToString() );
    }

    public List<S3_LobbyServerInfo> RequestServerList()
    {
        List<S3_LobbyServerInfo> result = new List<S3_LobbyServerInfo>();
        JSONClass InfoJson = new JSONClass();
        InfoJson["UserName"] = string.Empty;
        InfoJson["passwordHash"].AsInt = 0;
        InfoJson["type"] = "listRequest";
        Send( client, InfoJson.ToString() );
        Receive( client );
        while( receiveDone.WaitOne( 1000 ) )
        {
            Debug.Log("Receiving Server...");
            if( DataResponse.message != null )
            {
                S3_LobbyServerInfo serverInfo = new S3_LobbyServerInfo
                {
                    PlayerCount = Int32.Parse( DataResponse.message.Substring( 0, 1 ) ),
                    ServerName = DataResponse.message.Substring( 1 ),
                    Ip = IntToIpString( DataResponse.responseCode ),
                };
                result.Add( serverInfo );
                Receive( client ); 
            }
        }

        return result;
    }

    public void SendChatString(string toSend)
    {
        //Debug.Log( "Sending chat string: " + toSend );
        JSONClass ChatJson = new JSONClass();
        ChatJson["UserName"] = toSend;
        ChatJson["passwordHash"].AsInt = toSend.Length;
        ChatJson["type"] = "chatMessage";
        Send( client, ChatJson.ToString() );
    }

    public string SendChatHeartBeat()
    {
        JSONClass HeartbeatJson = new JSONClass();
        HeartbeatJson["UserName"] = string.Empty;
        HeartbeatJson["passwordHash"].AsInt = 0;
        HeartbeatJson["type"] = "chatHeartbeat";
        Send( client, HeartbeatJson.ToString() );
        Receive( client );
        receiveDone.WaitOne( 1000 );
        return DataResponse.message;
    }

    public void SendServerClose(string name, int players)
    {
        JSONClass CloseJson = new JSONClass();
        CloseJson["UserName"] = name;
        CloseJson["passwordHash"].AsInt = players;
        CloseJson["type"] = "closeServer";
        Send( client, CloseJson.ToString() );


    }

    public S3DataResponse AttemptLoginOrRegister(S3DataRequet request)
    {
        StateObject sb;
        JSONClass jRequest = new JSONClass();
        jRequest["UserName"] = request.UserName;
        jRequest["passwordHash"].AsInt = request.passwordHash;
        jRequest["type"] = request.type;
        Send( client, jRequest.ToString() );

        Receive( client);
        Debug.Log("Waiting for reply");
        receiveDone.WaitOne(15000);
        Debug.Log("Reply received");

        Debug.Log(String.Format("SBDataResponse = {0}", DataResponse.message));
        return DataResponse;
    }

    public void StopClient()
    {
        try
        {
            client.Shutdown( SocketShutdown.Both );
            client.Close();
        }
        catch( SocketException e )
        {
            Debug.Log( "Socket closed remotely" );
        }
        client.Close();
    }


    private static void ConnectCallback( IAsyncResult ar )
    {
        try
        {
            // Retrieve the socket from the state object.
            Socket client = (Socket)ar.AsyncState;

            // Complete the connection.
            client.EndConnect( ar );

            Debug.Log( "Socket connected to " +
                client.RemoteEndPoint.ToString() );

            // Signal that the connection has been made.
            connectDone.Set();
        }
        catch( Exception e )
        {
            Console.WriteLine( e.ToString() );
        }
    }

    private void Receive( Socket client)
    {
        try
        {
            // Create the state object.
            StateObject state = new StateObject();
            state.workSocket = client;
            
            
            // Begin receiving the data from the remote device.
            client.BeginReceive( state.buffer, 0, StateObject.BufferSize, 0,
                new AsyncCallback( ReceiveCallback ), state );
    
        }
        catch( Exception e )
        {
            Console.WriteLine( e.ToString() );
        }
    }

    private static void ReceiveCallback( IAsyncResult ar )
    {
        try
        {
            // Retrieve the state object and the client socket 
            // from the asynchronous state object.
            StateObject state = (StateObject)ar.AsyncState;
            Socket client = state.workSocket;

            // Read data from the remote device.
            int bytesRead = client.EndReceive( ar );
            Debug.Log( "Read " + bytesRead.ToString() + " bytes" );

            if( bytesRead > 0 )
            {
                
                // There might be more data, so store the data received so far.
                string readString = Encoding.ASCII.GetString( state.buffer, 0, bytesRead );

                state.sb.Append( readString );
                Debug.Log("Receiving data: " + state.sb.ToString());
                // Get the rest of the data.
                //this piece of code was causing problems with account management inconsistencies. this is our exercise from Arthur!
                /*client.BeginReceive( state.buffer, 0, StateObject.BufferSize, 0,
                    new AsyncCallback( ReceiveCallback ), state );*/

                //this was moved from else to here
                
                string content = state.sb.ToString();
                var s3Response = JSON.Parse(content);
                if (s3Response["responseCode"] != null )
                {
                    DataResponse = new S3DataResponse
                    {
                        responseCode = s3Response["responseCode"].AsInt,
                        message = s3Response["message"]
                    };

                    Debug.Log(String.Format("s3Response = {0}", s3Response["message"]));
                    receiveDone.Set();
                }
                else
                {
                    Debug.Log("Failed to parse response!");
                    receiveDone.Set();
                }
            }
            else
            {
                // All the data has arrived; put it in response.
                Debug.Log("Finished receiving data");
                
            }
        }
        catch( Exception e )
        {
            Console.WriteLine( e.ToString() );
        }
    }

    private static void Send( Socket client, String data )
    {
        // Convert the string data to byte data using ASCII encoding.
        byte[] byteData = Encoding.ASCII.GetBytes( data );
        // Begin sending the data to the remote device.
        client.BeginSend( byteData, 0, byteData.Length, 0,
            new AsyncCallback( SendCallback ), client );
    }

    private static void SendCallback( IAsyncResult ar )
    {
        try
        {
            // Retrieve the socket from the state object.
            Socket client = (Socket)ar.AsyncState;

            // Complete sending the data to the remote device.
            int bytesSent = client.EndSend( ar );
            Debug.Log( "Sent " + bytesSent + " bytes to server." );

            // Signal that all bytes have been sent.
        }
        catch( Exception e )
        {
            Console.WriteLine( e.ToString() );
        }
    }
}