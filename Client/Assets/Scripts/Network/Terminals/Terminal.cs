using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


namespace Network
{
    using Packets;
    /// <summary>
    /// Network terminal, base class of STerminal and CTerminal
    /// USE ServerTerminal OR ClientTerminal OR YOUR CUSTOM INHERIT CLASS INSTEAD, DO NOT USE IT IMMEDIATELY
    /// </summary>
    public class Terminal
    {
        public enum TERMINAL_TYPE
        {
            NONE = 0,
            SERVER = 1,
            CLIENT = 2,
        };


        protected TERMINAL_TYPE m_TerminalType = TERMINAL_TYPE.NONE;
        public TERMINAL_TYPE TerminalType { get { return m_TerminalType; } }
        protected byte m_Err = 0;

        protected bool m_IsInit = false;

        // connection id as key
        protected Dictionary<int, RemoteTerminalInfo> m_ConnectedTerminalInfoMap;

        // if you don't need such restriction, remove it as you want.
        protected HashSet<string> m_ConnectedIpSet;

        /// <summary>
        /// buffer for both read and write
        /// </summary>
        protected ByteArray m_Buffer;

        public Terminal()
        {

        }


        public void SendPacket(IPacket packet, int hostId, int connId, int channel)
        {
            m_Buffer.Reset();
            m_Buffer.WriteHead(packet.GetPacketID());
            packet.Write(m_Buffer);
            NetworkTransport.Send(hostId, connId, channel, m_Buffer.BufferArray, m_Buffer.DataSize, out m_Err);
        }
        
        private RemoteTerminalInfo RegisterRemoteTerminal(int connID, int hostID)
        {
            if (m_ConnectedTerminalInfoMap.ContainsKey(connID))
            {
                Debug.LogError("Connection ID: " + connID + " duplicated!!!");
                return null;
            }

            int remotePort;
            ulong remoteNetwork;
            ushort remoteDstNode;
            byte error;
            string remoteIP = NetworkTransport.GetConnectionInfo(hostID, connID, 
                out remotePort, out remoteNetwork, out remoteDstNode, out error);

            if (m_ConnectedIpSet.Contains(remoteIP))
            {
                Debug.LogError("Remote Terminal: " + remoteIP + " is connected already!");
                return null;
            }

            RemoteTerminalInfo info = new RemoteTerminalInfo()
            {
                HostID = hostID,
                ConnID = connID,
                IP = remoteIP,
                Port = remotePort
            };
            m_ConnectedTerminalInfoMap.Add(connID, info);
            m_ConnectedIpSet.Add(remoteIP);

            Dispatcher.Dispatch(NetworkEvt.EVT_REMOTE_TERMINAL_REGISTERED, info);

            Debug.Log(string.Format("<color=blue>Remote Terminal {0} is connected to host {1} with Connection ID {2} via port {3}</color>", 
                remoteIP, hostID, connID, remotePort));
            return info;
        }

        private void UnRegisterRemoteTerminal(int connID)
        {
            RemoteTerminalInfo info;
            if (!m_ConnectedTerminalInfoMap.TryGetValue(connID, out info))
            {
                return;
            }

            m_ConnectedIpSet.Remove(info.IP);
            m_ConnectedTerminalInfoMap.Remove(connID);
            Debug.Log(string.Format("<color=red>Remote Terminal {0} is disconnected, host: {1}, Connection ID: {2}</color>",
                info.IP, info.HostID, info.ConnID));
        }

        // TODO: Data Receive, Packet Assembly and Dispatch
        // should drive by unity update event
        public void BufferProcess()
        {
            int processCnt = 0;
            int recvHostId;
            int recvConnId;
            int recvChannelId;
            int bufferSize = NetworkConst.MAX_PACKET_LEN;
            int dataSize;
            byte error;


            // PROCESS ONLY ONE PACKET PER FRAME IS NOT A GOOD IDEA, That's why here is a while loop
            NetworkEventType evt;
            do
            {
                m_Buffer.Reset();
                evt = NetworkTransport.Receive(out recvHostId, out recvConnId,
                    out recvChannelId, m_Buffer.BufferArray, bufferSize, out dataSize, out error);
                switch (evt)
                {
                    case NetworkEventType.ConnectEvent:
#if UNITY_EDITOR
                        Debug.Log("<color=blue>Connection Established</color>");
#endif
                        RemoteTerminalInfo info = RegisterRemoteTerminal(recvConnId, recvHostId);
                        Dispatcher.Dispatch(NetworkEvt.EVT_ON_CONNECTION_ESTABLISH, info);
                        break;
                    case NetworkEventType.DisconnectEvent:
#if UNITY_EDITOR
                        Debug.Log("<color=red>Connection Break</color>");
#endif
                        UnRegisterRemoteTerminal(recvConnId);
                        Dispatcher.Dispatch(NetworkEvt.EVT_ON_CONNECTION_BREAK, recvConnId, recvHostId);
                        break;
                    case NetworkEventType.DataEvent:
                        // assembly and dispatch packet
                        m_Buffer.ReadHead();
                        IPacket packet = PacketFactory.AssemblyPacket(m_Buffer);
                        PacketDispatcher.Instance.DispatchPacket(packet);
                        break;
                    case NetworkEventType.BroadcastEvent:
                        break;
                }
                ++processCnt;
            } while (evt != NetworkEventType.Nothing && processCnt < NetworkConst.MAX_PACKET_PROCESS_PER_FRAME);
        }
    }
}
