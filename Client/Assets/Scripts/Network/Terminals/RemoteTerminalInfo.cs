using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Network
{
    public class RemoteTerminalInfo
    {
        // which socket is hosting this remote terminal
        public int HostID;
        // connection id
        public int ConnID;
        public string IP;
        public int Port;
    }
}
