using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ContractLogics
{
    public class WipeOutContract : ContractBase
    {
        public WipeOutContract(int confID) : base(confID)
        {
            _Type = CONTRACT_TYPE.WIPE_OUT;
        }
    }
}
