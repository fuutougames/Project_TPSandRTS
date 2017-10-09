/*
 * AUTO GENERATED FILE, DO NOT MODIFY
 */

using System.Collections;
using System.Collections.Generic;

namespace Network.Packets
{
    public static partial class PacketFactory
    {
        private static Dictionary<int, PACKET_ASSEMBLER> m_PacketAssemblerMap = new Dictionary<int, PACKET_ASSEMBLER>()
        {
            {
                PacketID.PACKET_PACKET_TEST_1,
                Assembler_PACKET_PACKET_TEST_1
            },
            {
                PacketID.PACKET_CLIENT_CONN_RESPONSE,
                Assembler_PACKET_CLIENT_CONN_RESPONSE
            },
            {
                PacketID.PACKET_TEST_PACKET,
                Assembler_PACKET_TEST_PACKET
            },
            {
                PacketID.PACKET_TEST_PACKET_2,
                Assembler_PACKET_TEST_PACKET_2
            },

        };

        private static IPacket Assembler_PACKET_PACKET_TEST_1 (ByteArray buffer)
        {
            PACKET_PACKET_TEST_1 packet = new PACKET_PACKET_TEST_1();
            packet.Read(buffer);
            return packet;
        }

        private static IPacket Assembler_PACKET_CLIENT_CONN_RESPONSE (ByteArray buffer)
        {
            PACKET_CLIENT_CONN_RESPONSE packet = new PACKET_CLIENT_CONN_RESPONSE();
            packet.Read(buffer);
            return packet;
        }

        private static IPacket Assembler_PACKET_TEST_PACKET (ByteArray buffer)
        {
            PACKET_TEST_PACKET packet = new PACKET_TEST_PACKET();
            packet.Read(buffer);
            return packet;
        }

        private static IPacket Assembler_PACKET_TEST_PACKET_2 (ByteArray buffer)
        {
            PACKET_TEST_PACKET_2 packet = new PACKET_TEST_PACKET_2();
            packet.Read(buffer);
            return packet;
        }

    }
}

