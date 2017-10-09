/*
 * AUTO GENERATED FILE, DO NOT MODIFY
 */

using System.Collections.Generic;

namespace Network.Packets
{
    public class PACKET_TEST_PACKET_2 : IPacket
    {
        public int attrib1;


        public short GetPacketID()
        {
            return PacketID.PACKET_TEST_PACKET_2;

        }

        public void Read(ByteArray buffer)
        {
            attrib1 = buffer.ReadInt();

        }

        public void Write(ByteArray buffer)
        {
            buffer.WriteInt(attrib1);

        }
    }
}
