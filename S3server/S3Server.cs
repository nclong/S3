﻿using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections.Generic;
using Newtonsoft.Json;

// State object for reading client data asynchronously
public class StateObject
{
    // Client  socket.
    public Socket workSocket = null;
    // Size of receive buffer.
    public const int BufferSize = 1024;
    // Receive buffer.
    public byte[] buffer = new byte[BufferSize];
    // Received data string.
    public StringBuilder sb = new StringBuilder();
}

public class ServerObject
{
    int currentPlayers;
    string ipAddress;
}

public class AsynchronousSocketListener
{
    // Thread signal.
    public static ManualResetEvent allDone = new ManualResetEvent(false);
    public static Dictionary<string, Socket> clients = new Dictionary<string, Socket>();

    // Data buffer for incoming data.
    byte[] bytes = new Byte[1024];

    // Create a TCP/IP socket.
    public static Socket listener = new Socket(AddressFamily.InterNetwork,
         SocketType.Stream, ProtocolType.Tcp);

    //Listen to external IP address
    IPHostEntry ipHostInfo;
    IPAddress ipAddress;
    IPEndPoint localEndPoint;
    public static IPEndPoint any;

    static Dictionary<string, ServerObject> serverList;
    
    //test function
    public static void listenLoop(object o)
    {
        StateObject myState = (StateObject)o;
        int bytesRead;

        serverList = new Dictionary<string, ServerObject>();

        while ((bytesRead = myState.workSocket.Receive(myState.buffer)) > 0)
        {
            //testcopypaste
            String content = String.Empty;

            // Retrieve the state object and the handler socket
            // from the asynchronous state object.

            // Read data from the client socket. 
            //endtestpaste

            // There  might be more data, so store the data received so far.
            myState.sb.Append(Encoding.ASCII.GetString(
                myState.buffer, 0, bytesRead));


            //Assumes entire message will come in on one packet
            //Porbably a bad idea

            content = myState.sb.ToString();
            //this is where we parse the string to see if it's login request vs a server update request vs server join request

            if (false) //case for login/register request
            {
                //do things as normal
            }
            else if (myState.sb.GetType() = 'join') //case for join server
            {
                //send ip address of the requested server to the client
                if (serverList[name.toString()])
                {
                    sendToClient(serverList[name.toString()]);
                }
                else printError();
            }
            else //case for server update
            {
                //if the server to be created exists or if the server to be updated doesn't exist throw error
            }

            S3DataRequest requestContent = JsonConvert.DeserializeObject<S3DataRequest>(content);
            // All the data has been read from the 
            // client. Display it on the console.
            Console.WriteLine("Read {0} bytes from socket. \n Data : {1}",
                content.Length, content);
            Console.WriteLine("\n\n");

            Send(myState.workSocket, JsonConvert.SerializeObject(HandleDataRequest(requestContent)));
            myState.buffer = new byte[1024];
            myState.sb = new StringBuilder();
        }
    }

    public AsynchronousSocketListener()
    {
        //Listen to external IP address
        ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
        ipAddress = ipHostInfo.AddressList[0];
        localEndPoint = new IPEndPoint(ipAddress, 11000);

        // Listen to any IP Address
        any = new IPEndPoint(IPAddress.Any, 11000);

        //bind listener
        try
        {
            listener.Bind(any);
            listener.Listen(100);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }

    public static void StartListening()
    {
        //// Data buffer for incoming data.
        //byte[] bytes = new Byte[1024];

        //// Create a TCP/IP socket.
        //Socket listener = new Socket(AddressFamily.InterNetwork,
        //    SocketType.Stream, ProtocolType.Tcp);

        ////Listen to external IP address
        //IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
        //IPAddress ipAddress = ipHostInfo.AddressList[0];
        //IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 11000);

        //// Listen to any IP Address
        //IPEndPoint any = new IPEndPoint(IPAddress.Any, 11000);

        // Bind the socket to the local endpoint and listen for incoming connections.
        try
        {
            //listener.Bind(any);
            //listener.Listen(100);

            //while (true)
            //{
            // Set the event to nonsignaled state.
            allDone.Reset();

            // Start an asynchronous socket to listen for connections.
            listener.BeginAccept(
                new AsyncCallback(AcceptCallback),
                listener);
            // Wait until a connection is made before continuing.
            allDone.WaitOne();
            //}

        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }

        //Console.WriteLine("\nPress ENTER to continue...");
        //Console.Read();

    }

    public static void AcceptCallback(IAsyncResult ar)
    {
        // Signal the main thread to continue.
        

        // Get the socket that handles the client request.
        Socket listener = (Socket)ar.AsyncState;
        Socket handler = listener.EndAccept(ar);
        allDone.Set();

        // Create the state object.
        StateObject state = new StateObject();
        state.workSocket = handler;

        // Games have bidirectional communication (as opposed to request/response)
        // So I need to store all clients sockets so I can send them messages later
        // TODO: store in meaningful way,such as Dictionary<string,Socket>
        Thread listeningThread = new Thread(new ParameterizedThreadStart(listenLoop));
        listeningThread.Start(state);

        listener.BeginAccept(new AsyncCallback(AcceptCallback), listener);
    }

    public static void ReadCallback(IAsyncResult ar)
    {
        String content = String.Empty;

        // Retrieve the state object and the handler socket
        // from the asynchronous state object.
        StateObject state = (StateObject)ar.AsyncState;
        Socket handler = state.workSocket;

        // Read data from the client socket. 
        int bytesRead = handler.EndReceive(ar);

        if (bytesRead > 0)
        {
            //// There  might be more data, so store the data received so far.
            //state.sb.Append(Encoding.ASCII.GetString(
            //    state.buffer, 0, bytesRead));


            ////Assumes entire message will come in on one packet
            ////Porbably a bad idea

            //content = state.sb.ToString();
            //S3DataRequest requestContent = JsonConvert.DeserializeObject<S3DataRequest>(content);
            //// All the data has been read from the 
            //// client. Display it on the console.
            //Console.WriteLine("Read {0} bytes from socket. \n Data : {1}",
            //    content.Length, content);
            //Console.WriteLine("\n\n");

            //Send(handler, JsonConvert.SerializeObject(HandleDataRequest(requestContent)));


            //Not sure this part is neccessary

            // Setup a new state object
            //StateObject newstate = new StateObject();
            //newstate.workSocket = handler;

            // Call BeginReceive with a new state object
            //handler.BeginReceive(newstate.buffer, 0, StateObject.BufferSize, 0,
            //new AsyncCallback(ReadCallback), newstate);
        }

        // Setup a new state object
        StateObject newstate = new StateObject();
        newstate.workSocket = handler;

        // Call BeginReceive with a new state object
        handler.BeginReceive(newstate.buffer, 0, StateObject.BufferSize, 0,
        new AsyncCallback(ReadCallback), newstate);
    }

    private static S3DataResponse HandleDataRequest(S3DataRequest request)
    {
        if (request.type == "Login")
        {
            return AccountManager.RequestLogin(request);
        }
        else if (request.type == "Register")
        {
            return AccountManager.RequestRegister(request);
        }
        return new S3DataResponse()
        {
            responseCode = -1,
            message = "Invalid Data"
        };
    }

    private static void Send(Socket handler, String data)
    {
        // Convert the string data to byte data using ASCII encoding.
        byte[] byteData = Encoding.ASCII.GetBytes(data);
        Console.WriteLine("Attempting to send {0} bytes", byteData.Length);

        // Begin sending the data to the remote device.
        handler.BeginSend(byteData, 0, byteData.Length, 0,
            new AsyncCallback(SendCallback), handler);
    }

    private static void SendCallback(IAsyncResult ar)
    {
        try
        {
            // Retrieve the socket from the state object.
            Socket handler = (Socket)ar.AsyncState;

            // Complete sending the data to the remote device.
            int bytesSent = handler.EndSend(ar);
            Console.WriteLine("Sent {0} bytes to client.", bytesSent);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }


    public static void drawMenu(string status)
    {
        //clear the console
        Console.Clear();

        Console.WriteLine("----------------------------------------------------");
        Console.WriteLine("           Sword Sword Sword Server!");
        Console.WriteLine("----------------------------------------------------");
        Console.WriteLine("SERVER STATUS: [" + status + "]");
        Console.WriteLine("[F1] Start Server");
        Console.WriteLine("[INSERT] Exit");
        Console.WriteLine("----------------------------------------------------");
        Console.WriteLine("");
        Console.WriteLine("----------------------------------------------------");
    }

    
    public static int Main(String[] args)
    {
        //variable to store information about key presses
        ConsoleKeyInfo keyinfo;

        //Give initial message
        Console.WriteLine("Welcome to the S3 server! Please press any key to continue...");

        //grab initial state
        keyinfo = Console.ReadKey();

        //string to hold the status of the server
        string serverStatus = "OFF";

        //variables
        AsynchronousSocketListener server = new AsynchronousSocketListener();

        bool updateServerDisplay = true; //starts true for initial draw
        bool started = false;

        //Create listening thread
        Thread listeningThread = new Thread(() =>
        {
            Thread.CurrentThread.IsBackground = true;
            StartListening();
        });



        //server loops until we press insert
        do
        {
            if (updateServerDisplay)
            {

                drawMenu(serverStatus);

                //toggle update flag off
                updateServerDisplay = false;
            }

            //If there is a key in the key stream buffer
            if (Console.KeyAvailable)
            {
                //Grab the key pressed
                keyinfo = Console.ReadKey();

                if (keyinfo.Key == ConsoleKey.F1 && !started)
                {
                    serverStatus = "Awaiting New Connections";
                    started = true;

                    //redraw
                    drawMenu(serverStatus);
                   // StartListening();

                    //start the listening thread if it hasn't been started
                    if (!listeningThread.IsAlive) listeningThread.Start();

                    //otherwise resume it
                    else listeningThread.Resume();

                    //tell the server UI to update, eventually we need this to run only if the pressed key is F1, F2, or Insert
                    updateServerDisplay = true;
                }
            }
        }
        while (keyinfo.Key != ConsoleKey.Insert);

        return 0;
    }
}
