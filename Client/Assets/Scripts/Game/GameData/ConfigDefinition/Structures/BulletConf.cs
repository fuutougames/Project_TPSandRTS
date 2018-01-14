using UnityEngine;
using Common;

namespace Config
{
    [CreateAssetMenu(fileName = "BulletConf", menuName = "Config/BulletConf", order = 1)]
    public class BulletConf : ScriptableObject
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
        public float Damage;
        /// <summary>
        /// Base Damage when enemy's armor is not penetrate
        /// </summary>
        public float UnPenetratedDamage;
        /// <summary>
        /// Base Range of Bullet
        /// </summary>
        public float Range;
        /// <summary>
        /// Base Velocity of Bullet
        /// </summary>
        public float Velocity;
        /// <summary>
        /// Projectil Type
        /// </summary>
        public CommEnum.PROJECTILE_TYPE ProjectileType;
    }
}
