using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ContractLogics
{
    public enum CONTRACT_TYPE
    {
        ASSASINATE,
        DELIVER,
        DESTROY,
        ESCORT,
        WIPE_OUT,
        DEFEND,
    }

    public enum CONTRACT_UPDATE_ACTION
    {

    }

    public class ContractBase
    {
        protected int _Uid;
        protected CONTRACT_TYPE _Type;
        protected int _ConfID;

        public ContractBase(int confID)
        {
            _ConfID = confID;
        }

        public virtual void AddListeners()
        {

        }

        public virtual void RemoveListeners()
        {

        }

        public virtual bool IsContractComplete()
        {
            throw new System.NotImplementedException();
        }

    }
}
