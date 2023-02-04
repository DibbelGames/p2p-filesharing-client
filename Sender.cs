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
            if (peerList.listedPeers.Count != 0)
            {
                for (int i = 0; i < peerList.listedPeers.Count; i++)
                {
                    Peer peer = peerList.listedPeers[i];
                    //if no pong has yet arrived => increase "missed pings"
                    //if a pong has arrived, the "missed pings" counter resets
                    if (peer.waitingForPong != 0)
                    {
                        peer.missedPings++;
                        Console.WriteLine("Ping Missed by " + peer.endPoint.Address);
                    }
                    else
                    {
                        peer.missedPings = 0;
                    }
                    if (peer.missedPings < 5)
                    {
                        UdpClient client = new UdpClient(11001);
                        Console.WriteLine("Sending Ping");
                        //client.Connect("192.168.178.75", 11000);
                        client.Connect(peer.endPoint.Address, 11000);

                        //Byte[] sendBytes = Encoding.ASCII.GetBytes("ping");
                        Byte[] sendBytes = new Byte[] { 1 };
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
            }

            //wait 10 secs for next ping flood
            await Task.Delay(2500);

            SendPing();
        }

        public void SendPong(IPEndPoint endPoint)
        {
            UdpClient client = new UdpClient(11002);
            Console.WriteLine("Sending Pong");
            client.Connect(endPoint.Address, 11000);

            //Byte[] sendBytes = Encoding.ASCII.GetBytes("pong");
            Byte[] sendBytes = new Byte[] { 2 };
            client.Send(sendBytes, sendBytes.Length);

            client.Dispose();

            Console.WriteLine("sent pong");
        }

        public void SendQuery(IPEndPoint endPoint, string filename)
        {
            UdpClient client = new UdpClient(11003);
            Console.WriteLine("Sending Query for -" + filename + "-");
            client.Connect(endPoint.Address, 11000);

            //**sending query in byte format**
            //first byte (3) shows that its a query
            //second byte (7) identifies the TTL
            //other ones are the filename in byte format
            Byte[] filenameBytes = Encoding.ASCII.GetBytes(filename);
            Byte[] sendBytes = new Byte[] { 3, 7 };
            sendBytes = sendBytes.Concat(filenameBytes).ToArray();

            client.Send(sendBytes, sendBytes.Length);

            client.Dispose();

            Console.WriteLine("sent query");
        }
    }
}