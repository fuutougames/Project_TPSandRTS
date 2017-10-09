/*
 * AUTO GENERATED FILE, DO NOT MODIFY
 */

using System.Collections;
using System.Collections.Generic;

namespace Network.Packets
{
    public partial class PacketFactory
    {
        private static Dictionary<int, PACKET_DISAESSEMBLER> m_PacketDisassemblerMap = new Dictionary<int, PACKET_DISAESSEMBLER>()
        {
            {
                PacketID.PACKET_PACKET_TEST_1,
                Disassembler_PACKET_PACKET_TEST_1
            },
            {
                PacketID.PACKET_CLIENT_CONN_RESPONSE,
                Disassembler_PACKET_CLIENT_CONN_RESPONSE
            },
            {
                PacketID.PACKET_TEST_PACKET,
                Disassembler_PACKET_TEST_PACKET
            },
            {
                PacketID.PACKET_TEST_PACKET_2,
                Disassembler_PACKET_TEST_PACKET_2
            },

        };

        private static void Disassembler_PACKET_PACKET_TEST_1 (IPacket packet, ref ByteArray buffer)
        {
            packet.Write(buffer);
        }

        private static void Disassembler_PACKET_CLIENT_CONN_RESPONSE (IPacket packet, ref ByteArray buffer)
        {
            packet.Write(buffer);
        }

        private static void Disassembler_PACKET_TEST_PACKET (IPacket packet, ref ByteArray buffer)
        {
            packet.Write(buffer);
        }

        private static void Disassembler_PACKET_TEST_PACKET_2 (IPacket packet, ref ByteArray buffer)
        {
            packet.Write(buffer);
        }

    }
}
