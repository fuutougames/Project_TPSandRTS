
using System.Collections.Generic;

namespace Battle
{
    public enum BATTLE_STATE
    {
        END = 0,
        ONGOING = 1
    }

    public enum DAMAGE_TYPE
    {
        BULLET_IMPACT = 1,
        BULLET_PENETRATE = 2,
        EXPLOSIVE = 3
    }

    public enum PROJECTILE_TYPE
    {
        //LINEAR = 1,
        JHP_PROJECTILE = 2,
        AP_PROJECTILE = 3,
        MISSILE = 4,
        SECTOR = 5,
    }

    public enum PROJECTILE_HITTYPE
    {
        NONE = -1,
        IN = 0,
        OUT = 1,
        PENETRATE = 2
    }

    public enum FIRE_MODE
    {
        AUTO,
        BURST,
        SINGLE
    }

    //public enum BATTLE_TYPE
    //{
    //    TYPE_1 = 1,
    //    TYPE_2 = 2,
    //    TYPE_3 = 3
    //}

    public class BattleDef
    {

    }
}
