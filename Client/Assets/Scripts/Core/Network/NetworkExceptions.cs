using System;
using System.Text;

namespace Network
{
    public class ByteArrayOutofRangeException : Exception
    {
        public ByteArrayOutofRangeException() : base("The ByteArray is out of range!") { }
        public ByteArrayOutofRangeException(short packetId) : base(string.Format("The ByteArray is out of range in packetId {0}!", packetId)) { }
    }

    public class PacketHandlerDuplicatedException : Exception
    {
        public PacketHandlerDuplicatedException() : base("Packet handler registration duplicated!") { }
        public PacketHandlerDuplicatedException(short packetId) : base(string.Format("Handler of packet {0} registration duplicated!", packetId)) { }
    }
}
