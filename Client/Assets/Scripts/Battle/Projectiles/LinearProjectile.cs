using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Battle;


namespace Battle.Projectiles
{
    using Data;

    public class LinearProjectile : ProjectileBase
    {
        protected ProjectileBattleData _PBData;
        protected ProjectileDmgLine _DmgLine = new ProjectileDmgLine();
        public Action<ProjectileDmgLineNode> OnCollideAction;

        /// <summary>
        /// Init CLIENT & SERVER infos, like _BaseDamage, _Velocity, etc ...
        /// </summary>
        public override void Init(ProjectileBattleData data)
        {
            base.Init(data);
            _PBData = data;
        }

        /// <summary>
        /// projectile position update function
        /// both server and client will use it
        /// </summary>
        protected override void UpdatePosition()
        {
            Vector3 newPos = CachedTransform.position + _SyncDirection*_PBData.Velocity*TimeMgr.Instance.GetDeltaTime();
            if (Vector3.Distance(newPos, _SyncStartPos) > _RealRange)
            {
                // TODO:dispose or recycle projectile here;
                CachedTransform.position = _SyncStartPos + _SyncDirection*_RealRange;
                Debug.LogError("Instance: " + this.GetInstanceID() + " Exceed Range, Request Dispose");
                DisposeProjectile();
            }
            else
            {
                CachedTransform.position = newPos;
            }
        }

        protected override void OnFixedUpdate()
        {
            if (_Disposed)
                return;

            base.OnFixedUpdate();
            float now = TimeMgr.Instance.GetCurrentTime();
            int curIdx = _DmgLine.GetPassedIdxByTime(now);
            for (int i = 0; i <= curIdx; ++i)
            {
                if (!_DmgLine.Nodes[i].Triggred)
                {
                    if (_DmgLine.Nodes[i].Obstacle != null)
                    {
                        _DmgLine.Nodes[i].Obstacle.OnProjectileCollide(this, _DmgLine.Nodes[i].TriggerPoint,
                            _DmgLine.Nodes[i].HitType, _ProjectileType);
                        if (OnCollideAction != null)
                            OnCollideAction.Invoke(_DmgLine.Nodes[i]);
                    }
                    _DmgLine.Nodes[i].Triggred = true;
                }
            }
        }

        /// <summary>
        /// Calculate Projectile's Damage Line before it's triggered;
        /// Client should have DmgLine data too, for showing hit effect by itself;
        /// </summary>
        protected override void BeforeProjectileTrigger()
        {
            base.BeforeProjectileTrigger();
            _DmgLine.ClearNode();
            Ray ray = new Ray(_SyncStartPos, _SyncDirection);
            RaycastHit[] hits = Physics.RaycastAll(ray, _PBData.MaxRange, ~GameLayer.ObstacleCollider);
            float dmgRemain = _PBData.BaseDamage;
            float projectileStartTime = Time.realtimeSinceStartup;
            _DmgLine.AddNode(projectileStartTime, dmgRemain, null, _SyncStartPos, 0);
            _DmgLine.StartPos = _SyncStartPos;
            bool projectileBlocked = false;
            for (int i = 0; i < hits.Length; ++i)
            {
                RaycastHit hit = hits[i];
                int instanceId = hit.transform.GetInstanceID();
                //TODO:Setup Obstacle Data in BattleMgr
                StaticObstacleData obstacle = BattleMgr.Instance.SceneData.GetObstacleDataByInstanceID(instanceId);
                // if not a stored obstacle, just break for now;
                if (!obstacle)
                    break;

                float timeNode = projectileStartTime + hit.distance/_PBData.Velocity;
                // obstacle not penetrable, projectile stop here;
                if (!obstacle.Penetrable)
                {
                    _DmgLine.SetStartAndEndTime(projectileStartTime, projectileStartTime + hit.distance/ _PBData.Velocity);
                    _RealRange = hit.distance;
                    _DmgLine.EndPos = _SyncStartPos + _SyncDirection*hit.distance;
                    _DmgLine.RealRange = _RealRange;
                    _DmgLine.SetStartAndEndTime(projectileStartTime, timeNode);
                    _DmgLine.AddNode(timeNode, 0, obstacle, _DmgLine.EndPos, 0);
                    projectileBlocked = true;
                    break;
                }

                // obstacle penetrable
                // calculate projectile's time-damage line node and store it;
                float penLen = float.MaxValue;
                Vector3 targetPoint = Vector3.zero;
                for (int j = 0; j < obstacle.CollideSurfaces.Length; ++j)
                {
                    Plane plane = obstacle.CollideSurfaces[j];

                    //filter surfaces which will never intersect with current ray.
                    //float dotResult = Vector3.Dot(plane.normal, ray.direction);
                    if (Vector3.Dot(plane.normal, ray.direction) < .0f)
                        continue;

                    float tmp;
                    if (plane.Raycast(ray, out tmp))
                    {
                        Vector3 point = ray.GetPoint(tmp);
                        float curLen = Vector3.Distance(point, hit.point);
                        if (curLen < penLen)
                        {
                            penLen = curLen;
                            targetPoint = point;
                        }
                    }
                }
                if (Mathf.Abs(penLen - float.MaxValue) <= Mathf.Epsilon)
                {
                    penLen = .0f;
                    targetPoint = hit.point;
                }

                float dmgLost = CalcDmgLost(obstacle, penLen);
                dmgRemain -= dmgLost;
                // projectile lost all damage, stop here;
                if (dmgRemain <= 0)
                {
                    // projectile stop here
                    _RealRange = hit.distance;
                    _DmgLine.EndPos = _SyncStartPos + _SyncDirection*hit.distance;
                    _DmgLine.RealRange = _RealRange;
                    _DmgLine.SetStartAndEndTime(projectileStartTime, timeNode);
                    _DmgLine.AddNode(timeNode, 0, obstacle, hit.point, BattleDef.PROJECTILE_HITTYPE.IN);
                    projectileBlocked = true;
                    break;
                }
                _DmgLine.AddNode(timeNode, dmgRemain, obstacle, hit.point, BattleDef.PROJECTILE_HITTYPE.IN);
                _DmgLine.AddNode(timeNode + penLen/ _PBData.Velocity, dmgRemain, obstacle, targetPoint,
                    BattleDef.PROJECTILE_HITTYPE.OUT);
            }

            // if projectile is not blocked, it should stop at it's max range;
            if (!projectileBlocked)
            {
                _RealRange = _PBData.MaxRange;
                _DmgLine.EndPos = _SyncStartPos + _SyncDirection* _PBData.MaxRange;
                _DmgLine.RealRange = _PBData.MaxRange;
                float timeNode = projectileStartTime + _PBData.MaxRange / _PBData.Velocity;
                _DmgLine.SetStartAndEndTime(projectileStartTime, timeNode);
                _DmgLine.AddNode(timeNode, dmgRemain, null, _DmgLine.EndPos, BattleDef.PROJECTILE_HITTYPE.IN);
            }
        }

        /// <summary>
        /// Calculate the damage lost after bullet go through an obstacle;
        /// both server and client will need this function
        /// </summary>
        /// <param name="obstacle"></param>
        /// <param name="penLen"></param>
        /// <returns></returns>
        protected virtual float CalcDmgLost(StaticObstacleData obstacle, float penLen)
        {
            //TODO: Calculate penetration damage lost;
            return 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="start"></param>
        /// <param name="dir"></param>
        /// <param name="center"></param>
        /// <param name="radius"></param>
        /// <param name="hitPoint1"></param>
        /// <param name="hitPoint2"></param>
        /// <returns></returns>
        protected bool IntersectWithSphere(Vector3 start, Vector3 dir, Vector3 center, float radius,
            out Vector3 hitPoint1, out Vector3 hitPoint2)
        {

            hitPoint1 = Vector3.zero;
            hitPoint2 = Vector3.zero;
            Vector3 startToUpCenter = center - start;
            float tmp = Vector3.Dot(startToUpCenter, dir);
            Vector3 closestPoint2Center = start + tmp*dir;
            Vector3 center2ClosestPointVec = closestPoint2Center - center;

            if (center2ClosestPointVec.magnitude >= radius)
            {
                return false;
            }
            float halfSegment = Mathf.Sqrt(radius*radius + center2ClosestPointVec.sqrMagnitude);
            hitPoint1 = closestPoint2Center - halfSegment*dir;
            hitPoint2 = closestPoint2Center + halfSegment*dir;

            return true;
        }

        /// <summary>
        /// The Y-axis of capsule collider must be Vector3.up;
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="capsule"></param>
        /// <returns>0: collide in; 1: collide out; 2: penetrate; other: no collide</returns>
        protected BattleDef.PROJECTILE_HITTYPE IntersectWithCapsuleCollider(Vector3 start, Vector3 end,
            CapsuleCollider capsule, Transform capTransform,
            out Vector3 hitPoint, out Vector3 hitPoint2)
        {
            hitPoint = Vector3.zero;
            hitPoint2 = Vector3.zero;

            Vector3 localStart = capTransform.InverseTransformPoint(start) - capsule.center;
            Vector3 localEnd = capTransform.InverseTransformPoint(end) - capsule.center;

            // line is above or below the capsule;
            float cylinderHeight = capsule.height - capsule.radius*2;
            cylinderHeight = cylinderHeight > 0 ? cylinderHeight : 0;
            float halfHeight = cylinderHeight/2.0f;
            float maxHeight = halfHeight + capsule.radius;
            float minHeight = -maxHeight;
            if (localStart.y > maxHeight && localEnd.y > maxHeight)
            {
                return BattleDef.PROJECTILE_HITTYPE.NONE;
            }

            if (localStart.y < minHeight && localEnd.y < minHeight)
            {
                return BattleDef.PROJECTILE_HITTYPE.NONE;
            }

            Vector2 projectOnXZ_Start = new Vector2(localStart.x, localStart.z);
            Vector2 projectOnXZ_End = new Vector2(localEnd.x, localEnd.z);
            Vector3 posDiff = (localEnd - localStart);
            Vector3 lineDir = posDiff.normalized;
            Vector2 ProjectOnXZ_lineDir = new Vector2(lineDir.x, lineDir.z);
            float maxDirWeight = posDiff.magnitude;
            float tmp = Vector3.Dot(lineDir, -localStart);

            // projection of line is not intersect with the projection of capsule, no collision
            if (tmp < 0 && projectOnXZ_Start.magnitude > capsule.radius)
            {
                return BattleDef.PROJECTILE_HITTYPE.NONE;
            }
            if (Vector3.Dot(-projectOnXZ_End, lineDir) > 0 && projectOnXZ_End.magnitude > capsule.radius)
            {
                return BattleDef.PROJECTILE_HITTYPE.NONE;
            }
            Vector2 closestPointOnXZ = projectOnXZ_Start + tmp*ProjectOnXZ_lineDir;
            float magnitudeOfColsestPiont = closestPointOnXZ.magnitude;
            if (magnitudeOfColsestPiont > capsule.radius)
            {
                return BattleDef.PROJECTILE_HITTYPE.NONE;
            }

            Vector3 upCenter = new Vector3(.0f, .0f, halfHeight), downCenter = new Vector3(.0f, .0f, -halfHeight);
            Vector3 sHitPoint1, sHitPoint2;
            float halfSegment =
                Mathf.Sqrt(capsule.radius*capsule.radius - magnitudeOfColsestPiont*magnitudeOfColsestPiont);
            Vector2 projectPossibleIntersectPoint1 = closestPointOnXZ - halfSegment*ProjectOnXZ_lineDir.normalized;
            Vector2 projectPossibleIntersectPoint2 = closestPointOnXZ + halfSegment*ProjectOnXZ_lineDir.normalized;
            float dirWeight1 = (projectPossibleIntersectPoint1.x - projectOnXZ_Start.x)/ProjectOnXZ_lineDir.x;
            float dirWeight2 = (projectPossibleIntersectPoint2.x - projectOnXZ_Start.x)/ProjectOnXZ_lineDir.x;
            Vector3 intersectPoint1 = localStart + dirWeight1*lineDir;
            Vector3 intersectPoint2 = localStart + dirWeight2*lineDir;
            bool notIntersectWithCylinder = (intersectPoint1.y > upCenter.y && intersectPoint2.y > upCenter.y) ||
                                            (intersectPoint1.y < downCenter.y && intersectPoint2.y < downCenter.y);
            // if intersect with up sphere
            bool notIntersectWithUpSphere =
                !IntersectWithSphere(localStart, lineDir, upCenter, capsule.radius, out sHitPoint1, out sHitPoint2);
            if (!notIntersectWithUpSphere)
            {
                if (sHitPoint1.y > upCenter.y)
                    intersectPoint1 = sHitPoint1;
                if (sHitPoint2.y > upCenter.y)
                    intersectPoint2 = sHitPoint2;
            }

            // if intersect with down sphere
            bool notIntersectWithDownSphere =
                !IntersectWithSphere(localStart, lineDir, downCenter, capsule.radius, out sHitPoint1, out sHitPoint2);
            if (!notIntersectWithDownSphere)
            {
                if (sHitPoint1.y < downCenter.y)
                    intersectPoint1 = sHitPoint1;
                if (sHitPoint2.y < downCenter.y)
                    intersectPoint2 = sHitPoint2;
            }

            if (notIntersectWithCylinder && notIntersectWithDownSphere && notIntersectWithUpSphere)
            {
                return BattleDef.PROJECTILE_HITTYPE.NONE;
            }

            dirWeight1 = (intersectPoint1.x - localStart.x)/lineDir.x;
            dirWeight2 = (intersectPoint2.x - localStart.x)/lineDir.x;

            hitPoint = intersectPoint1;
            hitPoint2 = intersectPoint2;
            // not arrive yet;
            if (dirWeight1 < 0 && dirWeight2 < 0)
            {
                return BattleDef.PROJECTILE_HITTYPE.NONE;
            }
            // passed already
            if (dirWeight1 > maxDirWeight && dirWeight2 > maxDirWeight)
            {
                return BattleDef.PROJECTILE_HITTYPE.NONE;
            }
            // in the capsule and no collide with any surface;
            if (dirWeight1 < 0 && dirWeight2 > maxDirWeight)
            {
                return BattleDef.PROJECTILE_HITTYPE.NONE;
            }

            // collide into the capsule
            if (dirWeight1 > 0 && dirWeight2 > maxDirWeight)
                return BattleDef.PROJECTILE_HITTYPE.IN;

            // collide out of the capsule
            if (dirWeight1 < 0 && dirWeight2 < maxDirWeight)
                return BattleDef.PROJECTILE_HITTYPE.OUT;

            return BattleDef.PROJECTILE_HITTYPE.PENETRATE;
        }

        public override BattleDef.PROJECTILE_HITTYPE IsCollideWithCharacter(float time, CharacterBattleData cbData,
            out Vector3[] hitPoints, out float penLen)
        {
            //base.IsCollideWithCharacter(time, cbData, out hitPoints);
            hitPoints = null;
            penLen = .0f;
            Vector3 projectilePos = _DmgLine.GetPositionByTime(time);
            Vector3 projectilePosNext = _DmgLine.GetPositionByTime(time + TimeMgr.Instance.GetDeltaTime());

            Vector3 hitPoint1, hitPoint2;
            BattleDef.PROJECTILE_HITTYPE hitType = IntersectWithCapsuleCollider(projectilePos, projectilePosNext,
                cbData.CCollider, cbData.CachedTransform,
                out hitPoint1, out hitPoint2);
            if (hitType != BattleDef.PROJECTILE_HITTYPE.NONE)
            {
                switch (hitType)
                {
                    case BattleDef.PROJECTILE_HITTYPE.IN:
                        hitPoints = new Vector3[] {hitPoint1};
                        penLen = (hitPoint2 - hitPoint1).magnitude;
                        break;
                    case BattleDef.PROJECTILE_HITTYPE.OUT:
                        hitPoints = new Vector3[] {hitPoint2};
                        break;
                    case BattleDef.PROJECTILE_HITTYPE.PENETRATE:
                        hitPoints = new Vector3[] {hitPoint1, hitPoint2};
                        penLen = (hitPoint2 - hitPoint1).magnitude;
                        break;
                }
            }
            return hitType;
        }

        public override bool IsCollideWithDynamicObstacle(float time, DynamicObstacleData doData, out Vector3 hitPoint)
        {
            return base.IsCollideWithDynamicObstacle(time, doData, out hitPoint);
        }
    }
}
