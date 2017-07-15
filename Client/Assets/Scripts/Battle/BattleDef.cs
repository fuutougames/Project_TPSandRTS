
using System.Collections.Generic;

namespace Battle
{
    public class BattleDef
    {
        public enum BATTLE_TYPE
        {
            TYPE_1 = 1,
            TYPE_2 = 2,
            TYPE_3 = 3
        }

        public enum DAMAGE_TYPE
        {
            BULLET_PENETRATE = 1,
            EXPLOSIVE = 2
        }

        public enum PROJECTILE_TYPE
        {
            LINEAR = 1,
            MISSILE = 2,
            SECTOR = 3,
        }

        public enum PROJECTILE_HITTYPE
        {
            NONE = -1,
            IN = 0,
            OUT = 1,
            PENETRATE = 2
        }

        // not really need it right now
        //public static readonly HashSet<PROJECTILE_TYPE> HitOnceOnlyProjectile = new HashSet<PROJECTILE_TYPE>()
        //{

        //};
    }
}
