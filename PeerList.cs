using System.Net;

namespace Gnutella
{
    class PeerList
    {
        public List<Peer> listedPeers = new List<Peer>();
        string peerList_path = "/home/leon/Documents/Gnutella_Files/list.txt";

        InformationBox alerts;

        public PeerList(InformationBox alerts)
        {
            this.alerts = alerts;

            //adding peers from local list
            string[] lines = File.ReadAllLines(peerList_path);
            for (int i = 0; i < lines.Length; i++)
            {
                AddPeer(lines[i]);
            }
        }

        public void AddPeer(string ip)
        {
            int i = 0;

            foreach (Peer p in listedPeers)
            {
                if (p.endPoint != null)
                {
                    if (p.endPoint.Address.ToString() == ip)
                    {
                        i++;
                    }
                }
            }

            try
            {
                if (i == 0)
                {
                    listedPeers.Add(new Peer(new IPEndPoint(IPAddress.Parse(ip), 11000)));

                    //if not already there => add ip to local file of stored peer
                    string text = File.ReadAllText(peerList_path);
                    if (!text.Contains(ip))
                        File.AppendAllLines(peerList_path, new string[1] { ip });

                    alerts.ShowInfo("New Peer added! \n(" + ip + ")");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("{0} Exeption caught.", e);
            }
        }

        public void RemovePeer(string ip)
        {
            Peer? removing = null;
            foreach (Peer p in listedPeers)
            {
                if (p.endPoint != null)
                {
                    if (p.endPoint.Address.ToString() == ip)
                    {
                        removing = p;
                    }
                }
            }
            if (removing != null)
            {
                listedPeers.Remove(removing);

                //remove ip from listed ips in the local file
                string[] lines = File.ReadAllLines(peerList_path);
                var linesToKeep = File.ReadAllLines(peerList_path).Where(l => l != ip);
                File.WriteAllLines(peerList_path, linesToKeep);

                alerts.ShowInfo("Peer removed! \n(" + ip + ")");
            }
        }
    }
}