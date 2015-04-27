using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Configuration;

namespace S3server
{
    class S3server
    {
        public int portNum = 2545;
        const int PLAYER_LIMIT = 3;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            AccountManager.TestDatabase();
            //setting up a tcp/ip server
            //step 1: create a listener, assign a port to it, start it
            TcpListener playerListener = new TcpListener(2545);
            playerListener.Start();

            //step 2: wait for incoming requests, accept socket
            Socket playerSoc = playerListener.AcceptSocket();

            //step 3: after accepting at least one socket, make a socket
            //        network stream
            Stream dataG = new NetworkStream(playerSoc);

            //step 4: communicate w/ client using pre-established protocols
            //aka, stuff that happens between server and client(s)

            //step 5: close stream, then socket
            dataG.Close();
            playerSoc.Close();

            //step 6: repeat step 2 until game quit/server shutdown


        }


    }
}
