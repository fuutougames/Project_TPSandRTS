using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Battle;


namespace Config
{
    [CreateAssetMenu(fileName = "GunConf", menuName = "Config/GunConf", order =1)]
    public class GunConf : ScriptableObject
    {
        /// <summary>
        /// Gun Type
        /// </summary>
        public GUN_TYPE GunType;
        /// <summary>
        /// Accuracy
        /// </summary>
        public float BaseAccuracy;
        /// <summary>
        /// Rounds per second;
        /// </summary>
        public float BaseFireRate;
        /// <summary>
        /// 
        /// </summary>
        public int BaseMagCapacity;

        /// <summary>
        /// Basic Damage Modifier
        /// </summary>
        public float BaseDmgWeight = 1;

        /// <summary>
        /// Basic Range Modifier
        /// </summary>
        public float BaseRangeWeight = 1;

        /// <summary>
        /// Basic Velocity Modifier
        /// </summary>
        public float BaseVelocityWeight = 1;

        /// <summary>
        /// Basic Penetration Modifier
        /// </summary>
        public float BasePenetrationWeight = 1;
    }
}
