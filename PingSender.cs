using System;
using System.Timers;
using System.Net;
using System.Net.Sockets;
using Raylib_cs;

namespace Gnutella
{
    class PingSender
    {
        UDPSocket s = new UDPSocket();

        public PingSender()
        {
            s.Client("127.0.0.1", 21000);
            SendPing();
        }

        public void Main()
        {

        }

        public async void SendPing()
        {
            Console.WriteLine("sending ping...");
            s.Send("Ping");
            await Task.Delay(5000);
            SendPing();
        }
    }
}