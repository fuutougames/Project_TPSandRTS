using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Network
{
    public class NetworkMgr : Singleton<NetworkMgr>
    {
        private Terminal.TERMINAL_TYPE _terminalType = Terminal.TERMINAL_TYPE.NONE;
        public Terminal.TERMINAL_TYPE TerminalType { get { return _terminalType; } }

        //public ServerTerminal _server;
        //public Terminal _client;
        public Terminal NTerminal
        {
            get
            {
                switch(_terminalType)
                {
                    case Terminal.TERMINAL_TYPE.SERVER:
                        return ServerTerminal.Instance;
                    case Terminal.TERMINAL_TYPE.CLIENT:
                        return ClientTerminal.Instance;
                    default:
                        return null;
                }
            }
        }

        public NetworkMgr()
        {

        }

        public void Init(Terminal.TERMINAL_TYPE ttype)
        {
            _terminalType = ttype;
            switch(ttype)
            {
                case Terminal.TERMINAL_TYPE.SERVER:
                    ServerTerminal.Instance.Init();
                    break;
                case Terminal.TERMINAL_TYPE.CLIENT:
                    ClientTerminal.Instance.Init();
                    break;
                default:break;
            }
        }

        public void ShutDown()
        {
            switch(_terminalType)
            {
                case Terminal.TERMINAL_TYPE.SERVER:
                    break;
                case Terminal.TERMINAL_TYPE.CLIENT:
                    break;
            }
        }
    }
}
