using UnityEngine;

namespace Battle
{
    using Data;
    public class PawnHitData
    {
        public Pawn APawn;
        public Vector3[] HitPoints;
        public float PenetrationLen;
        public BattleDef.PROJECTILE_HITTYPE HitType;
        public float HitDistance;


        public static int SortByHitDistance(PawnHitData item1, PawnHitData item2)
        {
            if (item1.HitDistance < item2.HitDistance)
                return -1;
            if (item1.HitDistance > item2.HitDistance)
                return 1;
            return 0;
        }
    }
}
