using System;
using System.Text;
using System.Net.Sockets;
using Raylib_cs;

namespace Gnutella
{
    class Sender
    {
        UdpClient client;

        public Sender()
        {
            Console.WriteLine("hello - sender");
        }

        public async void SendPing()
        {
            client = new UdpClient(11001);
            Console.WriteLine("connecting");
            client.Connect("127.0.0.1", 11000);
            Console.WriteLine("connected");

            Byte[] sendBytes = Encoding.ASCII.GetBytes("Ping");
            client.Send(sendBytes, sendBytes.Length);

            client.Dispose();

            Console.WriteLine("sent ping...");
            await Task.Delay(5000);
            SendPing();
        }
    }
}