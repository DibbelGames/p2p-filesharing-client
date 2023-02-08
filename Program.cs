using System.Numerics;
using System.Net;
using Raylib_cs;

namespace Gnutella
{
    static class Program
    {
        //4 main threads
        public static FileSystem? fileSystem;
        public static PeerList? peerList;
        public static Sender? sender;
        public static Listener? listener;

        //ui elements
        static TextBox? customConnection_TextBox;
        static TextBox? customQuery_TextBox;
        static InformationBox? alerts;

        public static void Main()
        {
            Raylib.InitWindow(640, 480, "Gnutella");
            Raylib.SetTargetFPS(60);

            //ui elements
            customConnection_TextBox = new TextBox(new Vector2(407, 45), new Vector2(200, 35), "IP-Address", 18);
            Button connect_Button = new Button(new Vector2(407, 95), new Vector2(200, 35), "Connect", 18, ConnectToPeer);
            customQuery_TextBox = new TextBox(new Vector2(407, 350), new Vector2(200, 35), "Filename", 18);
            Button query_Button = new Button(new Vector2(407, 400), new Vector2(200, 35), "Search", 18, QueryFile);
            alerts = new InformationBox(new Vector2(407, 230), "No files over \n65,5kB!", 20);

            //main threads
            fileSystem = new FileSystem(alerts);
            peerList = new PeerList(alerts);
            sender = new Sender(peerList, alerts);
            listener = new Listener(sender, peerList, fileSystem, alerts);

            //own thread for listener for it and others to run simultaneously
            Thread thread_listener = new Thread(listener.Listen);
            thread_listener.Start();

            while (!Raylib.WindowShouldClose())
            {
                Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.WHITE);

                Raylib.DrawText("Connected Peers:", 8, 8, 24, Color.BLACK);

                //UI
                //drawing all the connected peers
                for (int i = 0; i < peerList.listedPeers.Count; i++)
                {
                    Peer peer = peerList.listedPeers[i];
                    if (peer.endPoint != null)
                        Raylib.DrawText((i + 1) + ": " + peer.endPoint.Address + " on " + peer.endPoint.Port + ", " + peer.missedPings, 8, 50 * (i + 1), 20, Color.BLACK);
                }
                //drawing information box
                //drawing text boxes and buttons
                customConnection_TextBox.Main();
                connect_Button.Main();
                customQuery_TextBox.Main();
                query_Button.Main();
                alerts.Main();

                Raylib.EndDrawing();
            }

            //thread_listener.Abort(); //not on linux
            Raylib.CloseWindow();
            Environment.Exit(1);
        }

        static void ConnectToPeer()
        {
            if (peerList != null && customConnection_TextBox != null)
                peerList.AddPeer(customConnection_TextBox.input_string);
        }
        static void QueryFile()
        {
            String strHostName = string.Empty;
            IPHostEntry ipEntry = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress[] addr = ipEntry.AddressList;

            if (sender != null && customQuery_TextBox != null)
                sender.SendQuery(customQuery_TextBox.input_string, addr[1]);
        }
    }

    public class Peer
    {
        public System.Net.IPEndPoint? endPoint;
        public int missedPings;
        public int waitingForPong;

        public Peer(System.Net.IPEndPoint endPoint)
        {
            this.endPoint = endPoint;
        }
    }
}