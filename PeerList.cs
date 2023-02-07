using System.Net;

namespace Gnutella
{
    class PeerList
    {
        public List<Peer> listedPeers = new List<Peer>();

        public PeerList()
        {
            //hardcoded entrypoint (127.0.0.1)
            //listedPeers.Add(new Peer(new IPEndPoint(IPAddress.Parse("192.168.178.37"), 11000)));
            //AddPeer("127.0.0.1");
        }

        public void AddPeer(string ip)
        {
            int i = 0;

            foreach (Peer p in listedPeers)
            {
                if (p.endPoint.Address == IPAddress.Parse(ip))
                {
                    i++;
                }
            }

            try
            {
                if (i == 0)
                    listedPeers.Add(new Peer(new IPEndPoint(IPAddress.Parse(ip), 11000)));
            }
            catch (Exception e)
            {
                Console.WriteLine("{0} Exeption caught.", e);
            }
        }
    }
}