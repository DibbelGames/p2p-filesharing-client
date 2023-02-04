using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using Raylib_cs;

namespace Gnutella
{
    class Listener
    {
        UdpClient client;
        Sender sender;
        PeerList peerList;

        public Listener(Sender sender, PeerList peerList)
        {
            Console.WriteLine("hello - listener");
            this.sender = sender;
            this.peerList = peerList;
            this.client = new UdpClient(11000);
        }

        public void Listen()
        {
            Console.WriteLine("Listening...");

            IPEndPoint endpoint = new IPEndPoint(IPAddress.Any, 0);

            Byte[] data_bytes = client.Receive(ref endpoint);
            string data = Encoding.ASCII.GetString(data_bytes);

            Console.WriteLine("Received: -" +
                                         data.ToString() + "- from -" +
                                         endpoint.Address.ToString() + "- on -" +
                                         endpoint.Port.ToString() + "-");

            if (data == "ping")
            {
                foreach (Peer peer in peerList.listedPeers)
                {
                    if (peer.endPoint.Address != endpoint.Address)
                    {
                        Peer newPeer = new Peer(new IPEndPoint(endpoint.Address, 11000));
                    }
                }
            }
            else if (data == "pong")
            {

            }

            Listen();
        }
    }
}