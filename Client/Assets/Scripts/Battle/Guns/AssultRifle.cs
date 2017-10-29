using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle.Guns
{
    using Projectiles;
    public class AssultRifle : GunBase
    {
#if UNITY_EDITOR
        public float Accuracy;
        public float FireRate;
#endif
    }
}
