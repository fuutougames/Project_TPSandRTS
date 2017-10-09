using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Network
{
    public interface IPacket
    {
        void Read(ByteArray bytes);
        void Write(ByteArray bytes);
        short GetPacketID();
    }
}
