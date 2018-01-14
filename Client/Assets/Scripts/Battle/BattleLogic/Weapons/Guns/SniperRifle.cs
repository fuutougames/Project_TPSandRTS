using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle.Guns
{
    public class SniperRifle : GunBase
    {

        public override void Init()
        {
            base.Init();

            _FireMode = FIRE_MODE.SINGLE;
        }

        public override void SetFireMode(FIRE_MODE fireMode)
        {
            // do nothing
            //base.SetFireMode(fireMode);
        }
    }
}
