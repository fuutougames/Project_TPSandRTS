using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Network
{
    public class PacketDispatcher : Singleton<PacketDispatcher>
    {
        Dictionary<int, Action<IPacket>> m_PacketHandlerMap; 

        public PacketDispatcher()
        {
            m_PacketHandlerMap = new Dictionary<int, Action<IPacket>>();
        }

        public void RegisterHandler(short packetID, Action<IPacket> handler)
        {
            if (m_PacketHandlerMap.ContainsKey(packetID))
            {
                throw new PacketHandlerDuplicatedException(packetID);
            }

            m_PacketHandlerMap.Add(packetID, handler);
        }

        public void UnregisterHandler(short packetID)
        {
            if (!m_PacketHandlerMap.ContainsKey(packetID))
            {
                return;
            }

            m_PacketHandlerMap.Remove(packetID);
        }

        public void DispatchPacket(IPacket packet)
        {
            Action<IPacket> handler;
            if (!m_PacketHandlerMap.TryGetValue(packet.GetPacketID(), out handler))
            {
#if _DEBUG
                Debug.Log("<color=red>Packet ID: " + packet.GetPacketID() + " Doesn't have a handler</color>");
#endif
                return;
            }

            handler.Invoke(packet);
        }
    }
}
