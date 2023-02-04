using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using Raylib_cs;

namespace Gnutella
{
    class Sender
    {
        //Thread thread_ping;  //maybe if ping, pong and others wont work together
        //Thread thread_pong;

        PeerList peerList;

        public Sender(PeerList peerList)
        {
            Console.WriteLine("hello - sender");
            this.peerList = peerList;
            SendPing();
        }

        public async void SendPing()
        {
            foreach (Peer peer in peerList.listedPeers)
            {
                if (peer.missedPings < 6)
                {
                    UdpClient client = new UdpClient(11001);
                    Console.WriteLine("Sending Ping");
                    //client.Connect("192.168.178.75", 11000);
                    client.Connect(peer.endPoint.Address, 11000);

                    Byte[] sendBytes = Encoding.ASCII.GetBytes("ping");
                    client.Send(sendBytes, sendBytes.Length);
                    Console.WriteLine("sent ping");
                    peer.waitingForPong = 1;

                    client.Dispose();
                }
                else
                {
                    peerList.listedPeers.Remove(peer);
                }
            }

            //wait 5 secs for pong
            await Task.Delay(5000);

            //wait 10 secs for next ping flood
            await Task.Delay(10000);

            SendPing();
        }

        public void SendPong(IPEndPoint endPoint)
        {
            UdpClient client = new UdpClient(11002);
            Console.WriteLine("Sending Pong");
            client.Connect(endPoint.Address, 11000);

            Byte[] sendBytes = Encoding.ASCII.GetBytes("pong");
            client.Send(sendBytes, sendBytes.Length);

            client.Dispose();

            Console.WriteLine("sent pong");
        }
    }
}