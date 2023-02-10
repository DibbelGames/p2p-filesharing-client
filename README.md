# p2p-filesharing-client (Antheia)

## A very basic and primitive Gnutella-like filesharing client, made for a school project
> ### Super clumy and bad code, but it works...

#### + _Only for use in local Networks_ <img align="right" width="170" height="333" src="https://user-images.githubusercontent.com/53196457/218199581-0721e9a7-33b0-466c-bb10-2f9ea1608d43.jpeg">
#### + _Uses Udp for everything_ 😶
#### + _Sends Ping flood every 5 minutes_
#### + _Uses a TTL of 7_



### Contains 4 main Threads:
#### Listener:
  + listens on port 11000
  + if incoming data is ping: send back pong
  + if incoming data is query: check for file or send same query to neighbours
  + if incoming data is file: write file from incoming bytes

#### Sender:
  + SendPing(to all peers)
  + SendPong(to peer that sent 'ping')
  + SendQuery(to all peers)
  + Send File(to searching peer)

#### FileSystem:
  + keeping update of what files exist in /Documents/Antheia/
  + convert file to bytes if needed

#### PeerList:
  + keeping track of active peers in localAppdata/Antheia/peerlist.txt
  + adding listed peers at the start of the client
