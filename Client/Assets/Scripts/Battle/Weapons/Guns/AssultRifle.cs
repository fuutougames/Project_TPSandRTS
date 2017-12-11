using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle.Guns
{
    using Projectiles;
    public class AssultRifle : GunBase
    {
#if UNITY_EDITOR
        [SerializeField] public float Accuracy;
        [SerializeField] public float FireRate;
        [SerializeField] public FIRE_MODE FireMode;
#endif
    }
}
