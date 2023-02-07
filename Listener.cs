using System;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace Gnutella
{
    class Listener
    {
        Sender sender;
        PeerList peerList;
        FileSystem fileSystem;
        UdpClient client;

        public Listener(Sender sender, PeerList peerList, FileSystem fileSystem)
        {
            Console.WriteLine("hello - listener");
            this.sender = sender;
            this.peerList = peerList;
            this.fileSystem = fileSystem;
            this.client = new UdpClient(11000);
        }

        public void Listen()
        {
            IPEndPoint endpoint = new IPEndPoint(IPAddress.Any, 0);

            //receiving incoming data
            Byte[] dataBytes = client.Receive(ref endpoint);

            if (dataBytes[0] == (byte)1) //check if incoming data is a ping
            {
                //check if connection to the peer who sent the ping is already established
                //if not or if no connections exist => add the peer
                if (peerList.listedPeers.Count == 0)
                {
                    Peer newPeer = new Peer(new IPEndPoint(endpoint.Address, 11000));
                    peerList.listedPeers.Add(newPeer);
                    Console.WriteLine("added new peer");
                }
                else
                {
                    int k = 0;
                    for (int i = 0; i < peerList.listedPeers.Count; i++)
                    {

                        Peer peer = peerList.listedPeers[i];

                        if (peer.endPoint.Address.ToString() == endpoint.Address.ToString())
                        {
                            k++;
                        }
                    }

                    if (k == 0)
                    {
                        peerList.AddPeer(endpoint.Address.ToString());
                    }
                }

                //send pong to hold connection
                sender.SendPong(endpoint);
            }
            else if (dataBytes[0] == (byte)2) //check if its a pong
            {
                //change "waiting for pong" if pong is from a peer that still needs to send a pong
                for (int i = 0; i < peerList.listedPeers.Count; i++)
                {
                    Peer peer = peerList.listedPeers[i];

                    if (peer.endPoint.Address.ToString() == endpoint.Address.ToString())
                    {
                        peer.waitingForPong = 0;
                    }
                }
            }
            else if (dataBytes[0] == (byte)3) //check if incoming data is a query
            {
                int ttl = (int)dataBytes[1];

                byte[] ipAdress = new byte[4] { dataBytes[2], dataBytes[3], dataBytes[4], dataBytes[5] };

                //removing first 2 bytes from message as only the last part contains the filename
                byte[] filenameBytes = new byte[dataBytes.Length - 6];
                Array.Copy(dataBytes, 6, filenameBytes, 0, filenameBytes.Length);
                //read filename and ask the filesystem
                string filename = Encoding.ASCII.GetString(filenameBytes);
                Console.WriteLine(filename + " " + ttl);

                if (fileSystem.CheckForFile(filename))
                {
                    //if file is in storage, get the file in bytes and send them to the receiver
                    byte[] file = fileSystem.GetFile(filename);
                    sender.SendFile(new IPEndPoint(new IPAddress(ipAdress), 11000), file);
                }
                else
                {
                    //if file isnt found and ttl isnt yet 0 => send the same query to all connected peers with the ip of the receiever
                    if (ttl < 7)
                    {
                        ttl--;
                        sender.SendQuery(filename, new IPAddress(ipAdress));
                    }
                }
            }
            else if (dataBytes[0] == (byte)4) //check if incoming data is a file
            {
                //if incoming data is a file (case 4) => create a file from the bytes with the name of the last query made
                byte[] fileBytes = new byte[dataBytes.Length - 1];
                Array.Copy(dataBytes, 1, fileBytes, 0, fileBytes.Length);
                fileSystem.CreateFile(sender.openQuery, fileBytes);
            }

            Listen();
        }
    }
}