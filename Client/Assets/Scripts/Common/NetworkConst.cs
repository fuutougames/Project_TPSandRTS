using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Network
{
    public class NetworkConst
    {
        public const int MAX_PACKET_LEN = short.MaxValue;
        public const int HOST_PORT = 9020;
        public const int CONN_PORT = 9030;
        public const int MAX_CONNECTIONS = 10;
        public const int MAX_PACKET_PROCESS_PER_FRAME = int.MaxValue;
    }
}
