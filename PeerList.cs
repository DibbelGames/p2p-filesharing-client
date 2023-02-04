using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Raylib_cs;

namespace Gnutella
{
    class PeerList
    {
        public List<Peer> listedPeers = new List<Peer>();

        public PeerList()
        {
            //hardcoded entrypoint (127.0.0.1)
            //listedPeers.Add(new Peer(new IPEndPoint(IPAddress.Parse("192.168.178.75"), 11000)));
        }
    }
}