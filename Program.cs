using System.Net.Sockets;
using System.Threading;
using Raylib_cs;

namespace Gnutella
{
    static class Program
    {
        //static UdpClient client = new UdpClient(11000);

        public static Listener? listener;
        public static Sender? sender;

        public static void Main()
        {
            Raylib.InitWindow(640, 480, "Gnutella");

            listener = new Listener();
            sender = new Sender();

            Thread thread_listener = new Thread(listener.Listen);
            Thread thread_sender = new Thread(sender.SendPing);

            thread_listener.Start();

            while (!Raylib.WindowShouldClose())
            {
                Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.WHITE);

                Raylib.DrawText("Waddup", 8, 8, 18, Color.BLACK);

                if (Raylib.IsKeyPressed(KeyboardKey.KEY_SPACE))
                {
                    sender.SendPing();
                }

                Raylib.EndDrawing();
            }

            //thread_listener.Abort(); //not on linux
            Raylib.CloseWindow();
        }
    }
}