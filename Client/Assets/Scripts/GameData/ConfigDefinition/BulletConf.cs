using UnityEngine;
using Data.Const;

namespace Data.Config
{
    [CreateAssetMenu(fileName = "BulletConf", menuName = "Config/BulletConf", order = 1)]
    public class BulletCfg : ScriptableObject
    {
        /// <summary>
        /// Bullet type
        /// </summary>
        public CommEnum.BULLET_TYPE BulletType;
        /// <summary>
        /// Penetarte value;
        /// </summary>
        public float Penetration;
        /// <summary>
        /// Base Damage when enemy's armor is penetrate
        /// </summary>
        public float BaseDamage;
        /// <summary>
        /// Base Damage when enemy's armor is not penetrate
        /// </summary>
        public float UnPenetratedDamage;
    }
}
