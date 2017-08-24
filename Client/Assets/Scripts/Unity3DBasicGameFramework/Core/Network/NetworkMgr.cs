using System.Collections.Generic;
using UnityEngine;
using Common;
using UnityEngine.Networking;


public class NetworkMgr : Singleton<NetworkMgr>
{
    public CommEnum.NETWORK_TYPE NetworkType { get; private set; }

    private int _ChannelId;

    public NetworkMgr()
    {

    }

    public void SetupNetworkMgrAsHost()
    {
        NetworkType = CommEnum.NETWORK_TYPE.HOST;
        NetworkTransport.Init();
        ConnectionConfig config = new ConnectionConfig();
        _ChannelId = config.AddChannel(QosType.Reliable);
        HostTopology topology = new HostTopology(config, 10);
        int hostId = NetworkTransport.AddHost(topology, 8888);
    }

    public void SetupNetworkMgrAsClient()
    {
        NetworkType = CommEnum.NETWORK_TYPE.CLIENT;
    }
}

