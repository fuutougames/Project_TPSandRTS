using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Network;

public class Client : MonoBehaviour {
    [SerializeField] InputField _Ip;
    [SerializeField] InputField _Port;
    [SerializeField] Text _ServerResponse;


	// Use this for initialization
	void Start () {
        _ServerResponse.horizontalOverflow = HorizontalWrapMode.Overflow;
        ClientTerminal.Instance.Init();
        Dispatcher.RegisterHandler(NetworkEvt.EVT_ON_CONNECTION_ESTABLISH, OnConnEstablished);
        PacketDispatcher.Instance.RegisterHandler(Network.Packets.PacketID.PACKET_CLIENT_CONN_RESPONSE, Handle_PACKET_CLIENT_CONN_RESPONSE);
	}
    
    private void OnConnEstablished(params object[] paramArr)
    {
        SendTest();
    }

	// Update is called once per frame
	void Update () {
        ClientTerminal.Instance.BufferProcess();
	}

    public void SendTest()
    {
        Network.Packets.PACKET_TEST_PACKET packet = new Network.Packets.PACKET_TEST_PACKET();
        packet.attrib1 = 1;
        packet.attrib2 = 2;
        packet.attrib3 = 3;
        packet.attrib4 = .25f;
        packet.attrib6 = "attrib6";
        packet.attrib7 = (System.Int64)int.MaxValue + 2;
        packet.pack = new Network.Packets.PACKET_TEST_PACKET_2();
        packet.pack.attrib1 = 123;
        for (int i = 0; i < 10; ++i)
        {
            packet.packs.Add(new Network.Packets.PACKET_TEST_PACKET_2() { attrib1 = i });
            packet.intlist.Add(i);
            packet.floatList.Add(i + .1f);
            packet.shortList.Add((short)i);
            packet.strList.Add("str: " + i);
        }
        ClientTerminal.Instance.SendPacketReliably(packet);
    }


    public void Connect()
    {
        if (_Ip == null || _Port == null)
            return;

        string ip = _Ip.text;
        int port = int.Parse(_Port.text);

        ClientTerminal.Instance.Connect(ip, port);
    }

    private void Handle_PACKET_CLIENT_CONN_RESPONSE(IPacket packet)
    {
        Network.Packets.PACKET_CLIENT_CONN_RESPONSE response = (Network.Packets.PACKET_CLIENT_CONN_RESPONSE)packet;

        _ServerResponse.text = "Server Response Received!";
    }
}
