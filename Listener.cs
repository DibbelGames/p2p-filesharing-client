using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using Raylib_cs;

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
            //Console.WriteLine("Listening...");

            IPEndPoint endpoint = new IPEndPoint(IPAddress.Any, 0);

            Byte[] dataBytes = client.Receive(ref endpoint);
            string data = Encoding.ASCII.GetString(dataBytes);

            /*foreach (byte b in dataBytes)
            {
                Console.WriteLine("Received: " + (int)b);
            }*/
            //Console.WriteLine("Received: -" + dataBytes.ToString() + "- from -" + endpoint.Address.ToString() + "- on -" + endpoint.Port.ToString() + "-");

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
                    for (int i = 0; i < peerList.listedPeers.Count; i++)
                    {
                        Peer peer = peerList.listedPeers[i];

                        if (peer.endPoint.Address.ToString() != endpoint.Address.ToString())
                        {
                            Peer newPeer = new Peer(new IPEndPoint(endpoint.Address, 11000));
                            peerList.listedPeers.Add(newPeer);
                        }
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
                    Console.WriteLine("File found!");
                    byte[] file = fileSystem.GetFile(filename);
                    sender.SendFile(new IPEndPoint(new IPAddress(ipAdress), 11000), file);
                    //fileSystem.CreateFile(filename, file);
                }
                else
                {
                    Console.WriteLine("File not found!");
                    if (ttl < 7)
                    {
                        ttl--;
                        sender.SendQuery("cat.txt", new IPAddress(ipAdress));
                    }
                }
            }
            else if (dataBytes[0] == (byte)4) //check if incoming data is a file
            {
                byte[] fileBytes = new byte[dataBytes.Length - 1];
                Array.Copy(dataBytes, 1, fileBytes, 0, fileBytes.Length);
                fileSystem.CreateFile(sender.openQuery, fileBytes);
            }

            Listen();
        }
    }
}