using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using Raylib_cs;

namespace Gnutella
{
    class Listener
    {
        UdpClient client;

        public Listener()
        {
            Console.WriteLine("hello - listener");
            this.client = new UdpClient(11000);
        }

        public void Listen()
        {
            Console.WriteLine("Listening...");

            IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);

            // Blocks until a message returns on this socket from a remote host.
            Byte[] receiveBytes = client.Receive(ref RemoteIpEndPoint);
            string returnData = Encoding.ASCII.GetString(receiveBytes);

            // Uses the IPEndPoint object to determine which of these two hosts responded.
            Console.WriteLine("This is the message you received " +
                                         returnData.ToString());
            Console.WriteLine("This message was sent from " +
                                        RemoteIpEndPoint.Address.ToString() +
                                        " on their port number " +
                                        RemoteIpEndPoint.Port.ToString());
            Listen();
        }
    }
}