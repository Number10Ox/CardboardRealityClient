using LiteNetLib;
using LiteNetLib.Utils;
using System;
using System.Collections.Generic;

namespace CardboardRealityClient
{
    class CardboardRealityNetClient : INetEventListener
    {
        private readonly string kCardboardRealityKey = "cardboardReality";

        private NetManager _netClient;
        List<string> _commandsToRequest = new List<string>();
        List<string> _commandsAwaitingReply = new List<string>();
        private NetDataWriter _dataWriter;

        public void Initialize()
        {
            _netClient = new NetManager(this, kCardboardRealityKey);
            _netClient.Start();
            _netClient.UpdateTime = 15;
            _dataWriter = new NetDataWriter();

            Console.WriteLine("Waiting for connection...");
        }

        public void Terminate()
        {
            if (_netClient != null)
            {
                _netClient.Stop();
            }
        }

        public void Update()
        {
            _netClient.PollEvents();

            var peer = _netClient.GetFirstPeer();
            if (peer != null && peer.ConnectionState == ConnectionState.Connected)
            {
                if (_commandsToRequest.Count > 0)
                {
                    string requestedCommand = _commandsToRequest[0];
                    _commandsToRequest.RemoveAt(0);
                    _commandsAwaitingReply.Add(requestedCommand);

                    Console.WriteLine("Sending command..." + requestedCommand);
                    _dataWriter.Reset();
                    _dataWriter.Put(requestedCommand);
                    peer.Send(_dataWriter, SendOptions.Sequenced);
                }
            }
            else
            {
                _netClient.SendDiscoveryRequest(new byte[] { 1 }, 5000);
            }
        }

        public void SendCommand(string command)
        {
            _commandsToRequest.Add(command);
        }

        public bool HavePendingCommands()
        {
            return _commandsToRequest.Count > 0 || _commandsAwaitingReply.Count > 0;
        }

        //
        // Summary:
        //     Network error (on send or receive)
        //
        // Parameters:
        //   endPoint:
        //     From endPoint (can be null)
        //
        //   socketErrorCode:
        //     Socket error code
        public void OnNetworkError(NetEndPoint endPoint, int socketErrorCode)
        {
        }

        //
        // Summary:
        //     Latency information updated
        //
        // Parameters:
        //   peer:
        //     Peer with updated latency
        //
        //   latency:
        //     latency value in milliseconds
        public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
        {

        }

        //
        // Summary:
        //     Received some data
        //
        // Parameters:
        //   peer:
        //     From peer
        //
        //   reader:
        //     DataReader containing all received data
        public void OnNetworkReceive(NetPeer peer, NetDataReader reader)
        {
            string commandProcessed = reader.GetString();

            if (!String.IsNullOrEmpty(commandProcessed))
            {

                if (_commandsAwaitingReply.Contains(commandProcessed))
                {
                    Console.WriteLine("Received response for command " + commandProcessed);
                    _commandsAwaitingReply.Remove(commandProcessed);
                }
                else
                {
                    Console.WriteLine("Received unexpected response for command " + commandProcessed);
                }
            }
            else
            {
                Console.WriteLine("Received null command response" + commandProcessed);
            }
        }

        //
        // Summary:
        //     Received unconnected message
        //
        // Parameters:
        //   remoteEndPoint:
        //     From address (IP and Port)
        //
        //   reader:
        //     Message data
        //
        //   messageType:
        //     Message type (simple, discovery request or responce)
        public void OnNetworkReceiveUnconnected(NetEndPoint remoteEndPoint, NetDataReader reader, UnconnectedMessageType messageType)
        {
            if (messageType == UnconnectedMessageType.DiscoveryResponse && _netClient.PeersCount == 0)
            {
                Console.WriteLine("[CLIENT] Received discovery response. Connecting to: " + remoteEndPoint);
                _netClient.Connect(remoteEndPoint);
            }
        }

        //
        // Summary:
        //     New remote peer connected to host, or client connected to remote host
        //
        // Parameters:
        //   peer:
        //     Connected peer object
        public void OnPeerConnected(NetPeer peer)
        {
            Console.WriteLine("Received OnPeerConnected");
        }

        //
        // Summary:
        //     Peer disconnected
        //
        // Parameters:
        //   peer:
        //     disconnected peer
        //
        //   disconnectInfo:
        //     additional info about reason, errorCode or data received with disconnect message
        public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            Console.WriteLine("Received OnPeerDisconnected");
        }
    }
}
