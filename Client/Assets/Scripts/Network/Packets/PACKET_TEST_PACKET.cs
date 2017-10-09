/*
 * AUTO GENERATED FILE, DO NOT MODIFY
 */

using System.Collections.Generic;

namespace Network.Packets
{
    public class PACKET_TEST_PACKET : IPacket
    {
        public byte attrib1;
        public short attrib2;
        public int attrib3;
        public float attrib4;
        public string attrib6;
        public System.Int64 attrib7;
        public bool attrib8;
        public PACKET_TEST_PACKET_2 pack;
        public List<PACKET_TEST_PACKET_2> packs = new List<PACKET_TEST_PACKET_2>();
        public List<int> intlist = new List<int>();
        public List<float> floatList = new List<float>();
        public List<short> shortList = new List<short>();
        public List<string> strList = new List<string>();


        public short GetPacketID()
        {
            return PacketID.PACKET_TEST_PACKET;

        }

        public void Read(ByteArray buffer)
        {
            attrib1 = buffer.ReadByte();
            attrib2 = buffer.ReadShort();
            attrib3 = buffer.ReadInt();
            attrib4 = buffer.ReadFloat();
            attrib6 = buffer.ReadString();
            attrib7 = buffer.ReadInt64();
            attrib8 = buffer.ReadBool();
            pack = new PACKET_TEST_PACKET_2 ();
            pack.Read(buffer);
            packs.Clear();
            short packsLen = buffer.ReadShort();
            for (int i = 0; i < packsLen; ++i)
            {
                PACKET_TEST_PACKET_2 packsItem = new PACKET_TEST_PACKET_2();
                packsItem.Read(buffer);
                packs.Add(packsItem);
            }
            intlist.Clear();
            short intlistLen = buffer.ReadShort();
            for (int i = 0; i < intlistLen; ++i)
            {
                intlist.Add(buffer.ReadInt());
            }
            floatList.Clear();
            short floatListLen = buffer.ReadShort();
            for (int i = 0; i < floatListLen; ++i)
            {
                floatList.Add(buffer.ReadFloat());
            }
            shortList.Clear();
            short shortListLen = buffer.ReadShort();
            for (int i = 0; i < shortListLen; ++i)
            {
                shortList.Add(buffer.ReadShort());
            }
            strList.Clear();
            short strListLen = buffer.ReadShort();
            for (int i = 0; i < strListLen; ++i)
            {
                strList.Add(buffer.ReadString());
            }

        }

        public void Write(ByteArray buffer)
        {
            buffer.WriteByte(attrib1);
            buffer.WriteShort(attrib2);
            buffer.WriteInt(attrib3);
            buffer.WriteFloat(attrib4);
            buffer.WriteString(attrib6);
            buffer.WriteInt64(attrib7);
            buffer.WriteBool(attrib8);
            pack.Write(buffer);
            buffer.WriteShort((short)packs.Count);
            for (int i = 0; i < packs.Count; ++i)
            {
                packs[i].Write(buffer);
            }
            buffer.WriteShort((short)intlist.Count);
            for (int i = 0; i < intlist.Count; ++i)
            {
                buffer.WriteInt(intlist[i]);
            }
            buffer.WriteShort((short)floatList.Count);
            for (int i = 0; i < floatList.Count; ++i)
            {
                buffer.WriteFloat(floatList[i]);
            }
            buffer.WriteShort((short)shortList.Count);
            for (int i = 0; i < shortList.Count; ++i)
            {
                buffer.WriteShort(shortList[i]);
            }
            buffer.WriteShort((short)strList.Count);
            for (int i = 0; i < strList.Count; ++i)
            {
                buffer.WriteString(strList[i]);
            }

        }
    }
}
