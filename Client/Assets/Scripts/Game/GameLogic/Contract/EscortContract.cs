using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ContractLogics
{
    public class EscortContract : ContractBase
    {
        public EscortContract(int confID) : base (confID)
        {
            _Type = CONTRACT_TYPE.ESCORT;
        }
    }
}
