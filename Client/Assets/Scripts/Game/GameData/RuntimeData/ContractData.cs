using System.Collections;
using System.Collections.Generic;
using Network.Packets;

namespace GameData
{
    using ContractLogics;
    public class ContractData : DataModuleBase
    {
        /// <summary>
        /// Commissions available
        /// </summary>
        private List<ContractLogicBase> _Contracts;
        public List<ContractLogicBase> Contracts { get { return _Contracts; } }

        /// <summary>
        /// Commissions assigned
        /// </summary>
        private List<ContractLogicBase> _CurrentContracts;
        public List<ContractLogicBase> CurrentContracts { get { return _CurrentContracts; } }


        public ContractLogicBase ConstructCommissionFromNetwork(PACKET_INFO_CONTRACT data)
        {
            ContractLogicBase contract = new ContractLogicBase();
            return contract;
        }

        public PACKET_INFO_CONTRACT ConstructNetworkPacketFromCommission(ContractLogicBase contract)
        {
            PACKET_INFO_CONTRACT packet = new PACKET_INFO_CONTRACT();
            return packet;
        }
    }
}
