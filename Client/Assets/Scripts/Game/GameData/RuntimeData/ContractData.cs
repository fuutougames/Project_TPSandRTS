using System.Collections;
using System.Collections.Generic;
using Network.Packets;

namespace GameData
{
    using ContractLogics;
    public class ContractData : DataModuleBase
    {
        /// <summary>
        /// Contracts available on contract board
        /// </summary>
        private List<ContractBase> _Contracts;
        public List<ContractBase> Contracts { get { return _Contracts; } }

        /// <summary>
        /// Contracts assigned
        /// </summary>
        private List<ContractBase> _CurrentContracts;
        public List<ContractBase> CurrentContracts { get { return _CurrentContracts; } }


        #region network related
        public ContractBase ConstructContractFromNetwork(PACKET_INFO_CONTRACT data)
        {
            ContractBase contract = new ContractBase(0);
            return contract;
        }

        public PACKET_INFO_CONTRACT ConstructNetworkPacketFromContract(ContractBase contract)
        {
            PACKET_INFO_CONTRACT packet = new PACKET_INFO_CONTRACT();
            return packet;
        }
        #endregion

        /// <summary>
        /// Refresh available contracts
        /// </summary>
        public void RefreshContracts()
        {

        }

        /// <summary>
        /// sign a contract
        /// </summary>
        /// <param name="uid"></param>
        public void SignContract(int uid)
        {

        }

        /// <summary>
        /// complete a contract manually
        /// </summary>
        /// <param name="uid"></param>
        public void CompleteContract(int uid)
        {

        }
    }
}
