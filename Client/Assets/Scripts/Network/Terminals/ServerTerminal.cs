using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Network
{
    public class ServerTerminal : Singleton<STerminal>
    {

    }

    /// <summary>
    /// Server implementation, Use its singleton version instead of using it immediately
    /// </summary>
    public class STerminal : Terminal
    {
        protected int m_ConnID = 0;
        protected int m_HostID = 0;
        protected int m_WebHostID = 0;
        protected int m_ReliableChannelID = 0;
        protected int m_UnreliableChannelID = 0;

        public STerminal()
        {
            int port = NetworkConst.HOST_PORT;

            m_TerminalType = TERMINAL_TYPE.SERVER;
            m_ConnectedTerminalInfoMap = new Dictionary<int, RemoteTerminalInfo>();
            m_ConnectedIpSet = new HashSet<string>();
            m_Buffer = new ByteArray(NetworkConst.MAX_PACKET_LEN);

            NetworkTransport.Init();
            ConnectionConfig config = new ConnectionConfig();
            m_ReliableChannelID = config.AddChannel(QosType.ReliableSequenced);
            m_UnreliableChannelID = config.AddChannel(QosType.Unreliable);

            #region EXTRA CHANNEL
            #endregion

            HostTopology topo = new HostTopology(config, NetworkConst.MAX_CONNECTIONS);
            m_HostID = NetworkTransport.AddHost(topo, port, null);
            m_WebHostID = NetworkTransport.AddWebsocketHost(topo, port, null);

            Debug.Log("<color=cyan>Host Established, HostId: " + m_HostID + "</color>");
        }
        
        public void Init()
        {
            
        }

        public void SendPacketToAllClient(IPacket packet, int channel)
        {
            Dictionary<int, RemoteTerminalInfo>.Enumerator iter = m_ConnectedTerminalInfoMap.GetEnumerator();
            while (iter.MoveNext())
            {
                SendPacket(packet, iter.Current.Value.HostID, iter.Current.Value.ConnID, channel);
            }
        }

        public void SendPacketToAllClientReliably(IPacket packet)
        {
            SendPacketToAllClient(packet, m_ReliableChannelID);
        }

        public void SendPacketToAllClientUnreliably(IPacket packet)
        {
            SendPacketToAllClient(packet, m_UnreliableChannelID);
        }

        public void SendPacketTo(IPacket packet, int connId, int channelId)
        {
            RemoteTerminalInfo info;
            if (!m_ConnectedTerminalInfoMap.TryGetValue(connId, out info))
            {
                return;
            }

            SendPacket(packet, info.HostID, info.ConnID, channelId);
        }

        public void SendPacketToClientReliably(IPacket packet, int connId)
        {
            SendPacketTo(packet, connId, m_ReliableChannelID);
        }

        public void SendPacketToClientUnreliably(IPacket packet, int connId)
        {
            SendPacketTo(packet, connId, m_UnreliableChannelID);
        }
    }
}
