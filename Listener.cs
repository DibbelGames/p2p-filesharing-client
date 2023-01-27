using System;
using System.Net;
using System.Net.Sockets;
using Raylib_cs;

namespace Gnutella
{
    class Listener
    {
        public Listener()
        {
            UDPSocket s = new UDPSocket();
            s.Server("127.0.0.1", 21000);
        }

        public void Main()
        {

        }
    }
}