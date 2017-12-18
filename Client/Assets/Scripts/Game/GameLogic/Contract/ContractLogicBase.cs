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

    public class ContractLogicBase
    {
        protected CONTRACT_TYPE _Type;
        protected int _ConfID;

        public virtual bool IsCommissionComplete()
        {
            throw new System.NotImplementedException();
        }

        public virtual void OnCommissionComplete()
        {
            throw new System.NotImplementedException();
        }
    }
}
