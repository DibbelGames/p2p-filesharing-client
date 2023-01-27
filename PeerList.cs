using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using Raylib_cs;

namespace Gnutella
{
    class PeerList
    {
        public List<string> peers = new List<string>();

        public PeerList()
        {

        }

        public void AddPeer(string ip)
        {
            if (!peers.Contains(ip))
            {
                peers.Add(ip);
            }
        }

        public void Main()
        {

        }
    }
}