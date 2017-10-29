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

        private float _RealRange;
        protected float RealRange
        {
            get { return _RealRange; }
            set { _RealRange = value; }
        }
        private float _CurrentMagnitude;
        protected float CurrentMagnitude
        {
            get { return _CurrentMagnitude; }
            set { _CurrentMagnitude = value; }
        }


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
            float newMagnitude = Vector3.Distance(newPos, _SyncStartPos);
            if (newMagnitude > RealRange)
            {
                newMagnitude = RealRange;
                // TODO:dispose or recycle projectile here;
                CachedTransform.position = _SyncStartPos + _SyncDirection*RealRange;
                //Debug.LogError("Instance: " + this.GetInstanceID() + " Exceed Range, Request Dispose");
                DisposeProjectile();
            }
            else
            {
                CachedTransform.position = newPos;
            }
            CurrentMagnitude = newMagnitude;
        }

        protected override void OnUpdate()
        {
            if (_Disposed)
                return;

            base.OnUpdate();

            float now = TimeMgr.Instance.GetCurrentTime();
            //int curIdx = _DmgLine.GetPassedIdxByTime(now);
            int curIdx = _DmgLine.GetPassedIdxByCurMagnitude(CurrentMagnitude);
            for (int i = 0; i <= curIdx; ++i)
            {
                if (!_DmgLine.Nodes[i].Triggred && _DmgLine.Nodes[i].Distance <= (RealRange + 0.001f))
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
        /// Calculate Projectile's Damage Line before it's update;
        /// IT SHOULD BE CALLED AT THE BattleData.UpdateProjectiles FUNCTION ON THE LIFE CYCLE'S FIRST FRAME OF THIS PROJECTILE.
        /// Client should have DmgLine data too, for showing hit effect by itself;
        /// </summary>
        public override void PreCalculateOnFirstFrame()
        {
            base.PreCalculateOnFirstFrame();
            _DmgLine.ClearNode();
            Ray ray = new Ray(_SyncStartPos, _SyncDirection);
            RaycastHit[] hits = Physics.RaycastAll(ray, _PBData.MaxRange, ~GameLayer.ObstacleCollider);
            float dmgRemain = _PBData.BaseDamage;
            float projectileStartTime = TimeMgr.Instance.GetCurrentTime();
            _DmgLine.StartPos = _SyncStartPos;
            _DmgLine.AddNode(dmgRemain, null, _SyncStartPos, 0);
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
                    RealRange = hit.distance;
                    _DmgLine.EndPos = _SyncStartPos + _SyncDirection*hit.distance;
                    _DmgLine.RealRange = RealRange;
                    _DmgLine.SetStartAndEndTime(projectileStartTime, timeNode);
                    _DmgLine.AddNode(0, obstacle, _DmgLine.EndPos, 0);
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
                    RealRange = hit.distance;
                    _DmgLine.EndPos = _SyncStartPos + _SyncDirection*hit.distance;
                    _DmgLine.RealRange = RealRange;
                    _DmgLine.SetStartAndEndTime(projectileStartTime, timeNode);
                    _DmgLine.AddNode(0, obstacle, hit.point, BattleDef.PROJECTILE_HITTYPE.IN);
                    projectileBlocked = true;
                    break;
                }
                _DmgLine.AddNode(dmgRemain, obstacle, hit.point, BattleDef.PROJECTILE_HITTYPE.IN);
                _DmgLine.AddNode(dmgRemain, obstacle, targetPoint,
                    BattleDef.PROJECTILE_HITTYPE.OUT);
            }

            // if projectile is not blocked, it should stop at it's max range;
            if (!projectileBlocked)
            {
                RealRange = _PBData.MaxRange;
                _DmgLine.EndPos = _SyncStartPos + _SyncDirection* _PBData.MaxRange;
                _DmgLine.RealRange = _PBData.MaxRange;
                float timeNode = projectileStartTime + _PBData.MaxRange / _PBData.Velocity;
                _DmgLine.SetStartAndEndTime(projectileStartTime, timeNode);
                _DmgLine.AddNode(dmgRemain, null, _DmgLine.EndPos, BattleDef.PROJECTILE_HITTYPE.IN);
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

        // hit ray
        private Ray ray1 = new Ray();
        // inverse hit ray
        private Ray ray2 = new Ray();
        /// <summary>
        /// Projectile collide function
        /// </summary>
        /// <param name="start">projectile position</param>
        /// <param name="end">projectile position in next frame</param>
        /// <param name="collider">target collider</param>
        /// <param name="capTrans">transform of target collider</param>
        /// <param name="hitPoint1">hit in point</param>
        /// <param name="hitPoint2">hit out point</param>
        /// <returns></returns>
        protected BattleDef.PROJECTILE_HITTYPE IntersectWithCollider(Vector3 start, Vector3 end, 
            Collider collider, Transform capTrans,
            out Vector3 hitPoint1, out Vector3 hitPoint2)
        {
            Vector3 dir = end - start;
            
            float maxdist = dir.magnitude;
            if (maxdist <= Mathf.Epsilon)
            {
                hitPoint1 = Vector3.zero;
                hitPoint2 = Vector3.zero;
                return BattleDef.PROJECTILE_HITTYPE.NONE;
            }
            dir.Normalize();
            ray1.origin = start;
            ray1.direction = dir;
            ray2.origin = end;
            ray2.direction = -dir;
            RaycastHit hit;
            bool isHit = collider.Raycast(ray1, out hit, maxdist);
            if (isHit)
            {
                hitPoint1 = hit.point;
                if ((hitPoint1 - _SyncStartPos).magnitude > RealRange)
                {
                    hitPoint1 = Vector3.zero;
                    hitPoint2 = Vector3.zero;
                    return BattleDef.PROJECTILE_HITTYPE.NONE;
                }
            }
            else
                hitPoint1 = Vector3.zero;

            bool isInverseHit = collider.Raycast(ray2, out hit, maxdist);

            if (isInverseHit)
                hitPoint2 = hit.point;
            else
                hitPoint2 = Vector3.zero;

            if (isHit && !isInverseHit)
                return BattleDef.PROJECTILE_HITTYPE.IN;
            else if (!isHit && isInverseHit)
                return BattleDef.PROJECTILE_HITTYPE.OUT;
            else if (isHit && isInverseHit)
                return BattleDef.PROJECTILE_HITTYPE.PENETRATE;

            return BattleDef.PROJECTILE_HITTYPE.NONE;
        }

        public override BattleDef.PROJECTILE_HITTYPE IsCollideWithPawn(float time, Pawn cbData,
            out Vector3[] hitPoints, out float penLen)
        {
            //base.IsCollideWithCharacter(time, cbData, out hitPoints);
            hitPoints = null;
            penLen = .0f;
            Vector3 projectilePos = _DmgLine.GetPositionByTime(time);
            Vector3 projectilePosNext = _DmgLine.GetPositionByTime(time + TimeMgr.Instance.GetDeltaTime());


            Vector3 hitPoint1, hitPoint2;
            BattleDef.PROJECTILE_HITTYPE hitType = IntersectWithCollider(projectilePos, projectilePosNext,
                cbData.PCollider, cbData.CachedTransform,
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

        //public override bool IsCollideWithDynamicObstacle(float time, DynamicObstacleData doData, out Vector3 hitPoint)
        //{
        //    return base.IsCollideWithDynamicObstacle(time, doData, out hitPoint);
        //}
    }
}
