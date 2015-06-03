using System;
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
    public int currentPlayers;
    public string ipAddress;
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

    //our serverlist dictionary
    static Dictionary<string, ServerObject> serverList;

    //chat buffer so it all stays in one packet
    static string chatBuffer = "";

    //chat response object
    static S3DataResponse chatBufferObject = new S3DataResponse();

    private static int IpStringToInt(string ip)
    {
        IPAddress ipAddr = IPAddress.Parse(ip);
        byte[] ipBytes = ipAddr.GetAddressBytes();
        return BitConverter.ToInt32(ipBytes, 0);
    }

    private static string IntToIpString(int ipInt)
    {
        byte[] ipBytes = BitConverter.GetBytes(ipInt);
        IPAddress ip = new IPAddress(ipBytes);
        return ip.ToString();
    }

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

            S3DataRequest requestContent = JsonConvert.DeserializeObject<S3DataRequest>(content);

            //this is where we parse the string to see if it's login request vs a server update request vs server join request

            if (requestContent.type == "login" || requestContent.type == "register") //case for login/register request - finished
            {
                //do things as normal
                Send(myState.workSocket, JsonConvert.SerializeObject(HandleDataRequest(requestContent)));
            }
            else if (requestContent.type == "join") //case for join server - maybe finished? Nick needs to doublecheck
            {
                //send ip address of the requested server to the client
                if (serverList.ContainsKey(requestContent.UserName))
                {
                    //send IP address of requested server to the requesting client
                    Send(myState.workSocket, JsonConvert.SerializeObject(IpStringToInt(serverList[requestContent.UserName].ipAddress)));
                }
                //else printError();
            }
            else if (requestContent.type == "listRequest") //case for client requesting current server list
            {
                foreach (ServerObject serverObject in serverList.Values)
                {
                    //send server info to clients
                    //Send(myState.workSocket, JsonConvert.SerializeObject(HandleDataRequest(requestContent))); <--- THIS NEEDS TO BE CHANGED, this is a copy-pasta, not tailored to this function
                }
            }
            else if (requestContent.type == "closeServer") //case for host closing server - finished
            {
                serverList.Remove(requestContent.UserName);
            }
            else if (requestContent.type == "chatHeartbeat") //case for sending host the chat buffer - maybe finished? Needs to be double checked
            {
                //set the object which we will send to have the chat log as it's message
                chatBufferObject.message = chatBuffer;

                //set response code to -2
                chatBufferObject.responseCode = -2;

                //send chat buffer object
                Send(myState.workSocket, JsonConvert.SerializeObject(chatBufferObject));
            }
            else if (requestContent.type == "chatMessage") //case for player sending chat - maybe finished?
            {
                if (chatBuffer.Length + requestContent.passwordHash > 950) //if adding this message pushes the buffer over 950 characters then we need to subtract messages until it's under 950 with this message added
                {
                    //determine how many letters to remove
                    int overflow = (chatBuffer.Length + requestContent.passwordHash) - 950;

                    //peek next char to see if it's \n and if so remove it too
                    if (chatBuffer[overflow] == '\n') // might be irrelevant if 950 chars never reaches top of screen anyway
                    {
                        chatBuffer = chatBuffer.Substring(overflow);
                    }
                    else chatBuffer = chatBuffer.Substring(overflow - 1); //since we start at index 0 need to subtract 1
                }

                //actually append the chat message to the end of the buffer
                chatBuffer += requestContent.UserName;
            }
            else //case for new server created + heartbeat updates - maybe finished?
            {
                //check to see if dictionary has the key
                if (serverList.ContainsKey(requestContent.UserName) && serverList[requestContent.UserName].ipAddress == requestContent.type) //For server updates
                {
                    //do updates here
                    serverList[requestContent.UserName].currentPlayers = requestContent.passwordHash;
                }
                else if (!serverList.ContainsKey(requestContent.UserName)) //for server creation and server doesn't exist already
                {
                    serverList[requestContent.UserName] = new ServerObject
                    {
                        currentPlayers = requestContent.passwordHash,
                        ipAddress = requestContent.type
                    };
                }
            }

            
            // All the data has been read from the 
            // client. Display it on the console.
            Console.WriteLine("Read {0} bytes from socket. \n Data : {1}",
                content.Length, content);
            Console.WriteLine("\n\n");

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

        //write the menu
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
