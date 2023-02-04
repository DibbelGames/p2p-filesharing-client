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
                //check if connection to the peer who sent the ping is already established
                //if not or if no connections exist => add the peer
                if (peerList.listedPeers.Count == 0)
                {
                    Console.WriteLine("zero");
                    Peer newPeer = new Peer(new IPEndPoint(endpoint.Address, 11000));
                    peerList.listedPeers.Add(newPeer);
                    Console.WriteLine("added");
                }
                else
                {
                    for (int i = 0; i < peerList.listedPeers.Count; i++)
                    {
                        Peer peer = peerList.listedPeers[i];

                        if (peer.endPoint.Address.ToString() != endpoint.Address.ToString())
                        {
                            Console.WriteLine("idk man");
                            Peer newPeer = new Peer(new IPEndPoint(endpoint.Address, 11000));
                            peerList.listedPeers.Add(newPeer);
                        }
                    }
                }

                //send pong to hold connection
                sender.SendPong(endpoint);
            }
            else if (data == "pong")
            {
                //change "missedPings"
                for (int i = 0; i < peerList.listedPeers.Count; i++)
                {
                    Peer peer = peerList.listedPeers[i];

                    if (peer.endPoint.Address.ToString() == endpoint.Address.ToString())
                    {
                        peer.waitingForPong = 0;
                    }
                }
            }

            Listen();
        }
    }
}