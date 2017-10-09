using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Network
{
    public class ClientTerminal : Singleton<CTerminal>
    {

    }

    /// <summary>
    /// Client implementation, Use its singleton version instead of using it immediately
    /// </summary>
    public class CTerminal : Terminal
    {
        protected int m_ConnID = 0;
        protected int m_HostID = 0;
        protected int m_WebHostID = 0;
        protected int m_ReliableChannelID = 0;
        protected int m_UnreliableChannelID = 0;

        public CTerminal()
        {
            int port = NetworkConst.CONN_PORT;

            m_TerminalType = TERMINAL_TYPE.CLIENT;
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

            Debug.Log("<color=cyan>Host Established, HostId: " + m_HostID + "</color>");
        }

        public void Init()
        {

        }

        public void Connect(string ip, int port)
        {
            m_ConnID = NetworkTransport.Connect(m_HostID, ip, port, 0, out m_Err);
            Debug.Log("Error Code: " + m_Err);
        }

        public void Disconnect()
        {
            NetworkTransport.Disconnect(m_HostID, m_ConnID, out m_Err);
        }

        public void SendPacket(IPacket packet, int channel)
        {
            m_Buffer.Reset();
            m_Buffer.WriteHead(packet.GetPacketID());
            packet.Write(m_Buffer);
            NetworkTransport.Send(m_HostID, m_ConnID, channel, m_Buffer.BufferArray, m_Buffer.DataSize, out m_Err);
        }

        public void SendPacketReliably(IPacket packet)
        {
            SendPacket(packet, m_ReliableChannelID);
        }

        public void SendPacketUnreliably(IPacket packet)
        {
            SendPacket(packet, m_UnreliableChannelID);
        }
    }
}
