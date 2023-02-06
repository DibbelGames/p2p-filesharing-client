using System.Net.Sockets;
using System.Threading;
using Raylib_cs;

using System.Text;
using System.Net;

namespace Gnutella
{
    static class Program
    {
        //static UdpClient client = new UdpClient(11000);

        public static FileSystem? fileSystem;
        public static PeerList? peerList;
        public static Sender? sender;
        public static Listener? listener;

        public static void Main()
        {
            Raylib.InitWindow(640, 480, "Gnutella");

            fileSystem = new FileSystem();
            peerList = new PeerList();
            sender = new Sender(peerList);
            listener = new Listener(sender, peerList, fileSystem);

            Thread thread_sender = new Thread(sender.SendPing);
            Thread thread_listener = new Thread(listener.Listen);

            thread_listener.Start();

            while (!Raylib.WindowShouldClose())
            {
                Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.WHITE);

                Raylib.DrawText("Waddup", 8, 8, 18, Color.BLACK);

                for (int i = 0; i < peerList.listedPeers.Count; i++)
                {
                    Peer peer = peerList.listedPeers[i];

                    Raylib.DrawText("Peer: " + peer.endPoint.Address + ", " + peer.endPoint.Port + ", " + peer.missedPings + ", " + peer.waitingForPong, 8, 50, 24, Color.BLACK);
                }

                if (Raylib.IsKeyPressed(KeyboardKey.KEY_SPACE))
                {
                    sender.SendQuery("cat.txt");
                }
                if (Raylib.IsKeyPressed(KeyboardKey.KEY_ENTER))
                {
                    String strHostName = string.Empty;
                    IPHostEntry ipEntry = Dns.GetHostEntry(Dns.GetHostName());
                    IPAddress[] addr = ipEntry.AddressList;

                    for (int i = 0; i < addr.Length; i++)
                    {
                        Console.WriteLine("IP Address {0}: {1} ", i, addr[i].ToString());
                    }
                }

                Raylib.EndDrawing();
            }

            //thread_listener.Abort(); //not on linux
            Raylib.CloseWindow();
            Environment.Exit(1);
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