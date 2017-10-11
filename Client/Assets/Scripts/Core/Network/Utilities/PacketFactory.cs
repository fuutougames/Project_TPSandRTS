/*
 * Packet Assembly Utilities
 */

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Network.Packets
{
    /// <summary>
    /// Utilities for assembling packet from byte array or dissembling packet to byte array
    /// </summary>
    public static partial class PacketFactory
    {
        /// <summary>
        /// Packet assembly function delegate definition
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public delegate IPacket PACKET_ASSEMBLER(ByteArray buffer);

        /// <summary>
        /// Packet disassembly function delegate definition
        /// </summary>
        /// <param name="packet"></param>
        /// <param name="buffer"></param>
        public delegate void PACKET_DISAESSEMBLER(IPacket packet, ref ByteArray buffer);

        /// <summary>
        /// Assembly packet from buffer data, possible null return
        /// </summary>
        /// <param name="buffer">buffer data</param>
        /// <returns>
        ///     IPacket: 
        ///     null: assembly failed, due to miss assembly function or buffer out of range
        /// </returns>
        public static IPacket AssemblyPacket(ByteArray buffer)
        {
            PACKET_ASSEMBLER func;
            if (!m_PacketAssemblerMap.TryGetValue(buffer.PacketId, out func))
            {
#if UNITY_EDITOR
                Debug.LogError(string.Format("Packet {0} don't have any assembly function!", buffer.PacketId.ToString()));
#endif
                return null;
            }

            IPacket packet;
            packet = func.Invoke(buffer);
            return packet;
        }

        /// <summary>
        /// Disassembly packet into byte array
        /// </summary>
        /// <param name="packet">packet need to dessembly</param>
        /// <param name="buffer">target buffer</param>
        /// <returns>
        ///     0: disassembly success,
        ///    -1: disassembly failed
        /// </returns>
        public static int DisaessemblyPacket(IPacket packet, ref ByteArray buffer)
        {
            PACKET_DISAESSEMBLER func;
            if (!m_PacketDisassemblerMap.TryGetValue(packet.GetPacketID(), out func))
            {
#if UNITY_EDITOR
                Debug.LogError(string.Format("Packet {0} don't have any dessembly function!", packet.GetPacketID().ToString()));
#endif
                return -1;
            }

            func.Invoke(packet, ref buffer);
            return 0;
        }
    }
}
