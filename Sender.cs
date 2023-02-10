using System;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace Gnutella
{
    class Sender
    {
        PeerList peerList;
        InformationBox alerts;

        public string openQuery = string.Empty;

        public Sender(PeerList peerList, InformationBox alerts)
        {
            this.alerts = alerts;
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
                        if (peer.endPoint != null)
                        {
                            Console.WriteLine("Ping Missed by " + peer.endPoint.Address);
                        }
                    }
                    else
                    {
                        peer.missedPings = 0;
                    }
                    if (peer.missedPings < 5)
                    {
                        try
                        {
                            UdpClient client = new UdpClient(11001);
                            if (peer.endPoint != null)
                            {
                                client.Connect(peer.endPoint.Address, 11000);
                            }

                            //send ping (one byte which is 1) if the receiving peer hasnt missed the last 5 pings
                            Byte[] sendBytes = new Byte[] { 1 };
                            client.Send(sendBytes, sendBytes.Length);
                            peer.waitingForPong = 1;

                            client.Dispose();
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("{0} Exeption caught.", e);
                        }
                    }
                    else
                    {
                        if (peer.endPoint != null)
                            peerList.RemovePeer(peer.endPoint.Address.ToString());
                    }
                }
            }

            //wait 10 secs for next ping flood
            await Task.Delay(2500);

            SendPing();
        }

        public void SendPong(IPEndPoint endPoint)
        {
            try
            {
                UdpClient client = new UdpClient(11002);
                client.Connect(endPoint.Address, 11000);

                //send pong (one byte which is 2)
                Byte[] sendBytes = new Byte[] { 2 };
                client.Send(sendBytes, sendBytes.Length);

                client.Dispose();
            }
            catch (Exception e)
            {
                Console.WriteLine("{0} Exeption caught.", e);
            }
        }

        public void SendQuery(string filename, IPAddress receiver, int ttl = 7)
        {
            try
            {
                //send query for a file to every connected peer containing the filename
                openQuery = filename;

                for (int i = 0; i < peerList.listedPeers.Count(); i++)
                {
                    Peer peer = peerList.listedPeers[i];

                    UdpClient client = new UdpClient(11003);
                    Console.WriteLine("Sending Query for -" + filename + "-");
                    if (peer.endPoint != null)
                    {
                        client.Connect(peer.endPoint.Address, 11000);
                    }
                    //**sending query in byte format**
                    //first byte (3) shows that its a query
                    //second byte (7) identifies the TTL
                    //other ones are the ip adress and filename in byte format
                    Byte[] filenameBytes = Encoding.ASCII.GetBytes(filename);

                    Byte[] ipAddress = receiver.GetAddressBytes();

                    Byte[] sendBytes = new Byte[] { 3, 7 };
                    sendBytes = sendBytes.Concat(ipAddress).ToArray();
                    sendBytes = sendBytes.Concat(filenameBytes).ToArray();

                    client.Send(sendBytes, sendBytes.Length);

                    alerts.ShowInfo("Asking for file! \n(" + filename + ")");

                    client.Dispose();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("{0} Exeption caught.", e);
                openQuery = string.Empty;
            }
        }

        public void SendFile(IPEndPoint endPoint, byte[] file)
        {
            try
            {
                UdpClient client = new UdpClient(11004);
                client.Connect(endPoint.Address, 11000);

                //sending file in bytes
                Byte[] sendBytes = new Byte[] { 4 };
                sendBytes = sendBytes.Concat(file).ToArray();
                client.Send(sendBytes, sendBytes.Length);

                client.Dispose();
            }
            catch (Exception e)
            {
                Console.WriteLine("{0} Exeption caught.", e);
            }
        }
    }
}