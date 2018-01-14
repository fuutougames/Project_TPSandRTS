using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ContractLogics
{
    public class DestroyContract : ContractBase
    {
        public DestroyContract(int confID) : base (confID)
        {
            _Type = CONTRACT_TYPE.DESTROY;
        }
    }
}
