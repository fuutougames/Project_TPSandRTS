using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ContractLogics
{
    public class DefendContract : ContractBase
    {
        public DefendContract(int confID) : base (confID)
        {
            _Type = CONTRACT_TYPE.DEFEND;
        }
    }
}
