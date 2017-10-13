using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Network.Packets.Structs
{
    public class TEST_PACKET
    {
        public byte attrib1;
        public short attrib2;
        public int attrib3;
        public float attrib4;
        public string attrib6;
        public Int64 attrib7;
        public bool attrib8;
        public TEST_PACKET_2 pack;
        public List<TEST_PACKET_2> packs;
        public List<int> intlist;
        public List<float> floatList;
        public List<short> shortList;
        public List<string> strList;
    }
}
