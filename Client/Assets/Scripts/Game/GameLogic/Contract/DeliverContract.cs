using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ContractLogics
{
    public class DeliverContract : ContractBase
    {
        public DeliverContract(int confID) : base (confID)
        {
            _Type = CONTRACT_TYPE.DELIVER;
        }
    }
}
