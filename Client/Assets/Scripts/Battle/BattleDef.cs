
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

    public enum GUN_TYPE
    {
        PISTOL,
        ASSULT_RIFLE,
        SNIPER_RIFLE,
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
