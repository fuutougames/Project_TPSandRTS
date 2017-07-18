using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Battle.Projectiles
{
    public class ProjectileDmgLineNode
    {
        public float TimeNode;
        public float RemainDamage;
        public ObstacleData Obstacle;
        public Vector3 TriggerPoint;
        public bool Triggred;
        public BattleDef.PROJECTILE_HITTYPE HitType;
    }


    public class ProjectileDmgLine
    {
        private Vector3 _StartPos;

        public Vector3 StartPos
        {
            get { return _StartPos; }
            set { _StartPos = value; }
        }

        private Vector3 _EndPos;

        public Vector3 EndPos
        {
            get { return _EndPos; }
            set { _EndPos = value; }
        }

        private float _RealRange;

        public float RealRange
        {
            get { return _RealRange; }
            set { _RealRange = value; }
        }

        private float _StartTime;
        private float _EndTime;
        private float _TimeClipLen;
        private List<ProjectileDmgLineNode> _Nodes;

        public List<ProjectileDmgLineNode> Nodes
        {
            get { return _Nodes; }
        }

        private int _NodeIdx;

        public int NodeCount
        {
            get { return _NodeIdx; }
        }

        public ProjectileDmgLine()
        {
            _Nodes = new List<ProjectileDmgLineNode>();
            _NodeIdx = 0;
        }

        public void SetStartAndEndTime(float start, float end)
        {
            _StartTime = start;
            _EndTime = end;
            _TimeClipLen = end - start;
        }

        public void AddNode(float timeNode, float remainDamage,
            ObstacleData obstacle, Vector3 triggerPoint, BattleDef.PROJECTILE_HITTYPE hitType)
        {
            if (_NodeIdx >= _Nodes.Count)
            {
                ProjectileDmgLineNode node = new ProjectileDmgLineNode()
                {
                    TimeNode = timeNode,
                    RemainDamage = remainDamage,
                    Obstacle = obstacle,
                    TriggerPoint = triggerPoint,
                    Triggred = false,
                    HitType = hitType
                };
                _Nodes.Add(node);
            }
            else
            {
                ProjectileDmgLineNode node = _Nodes[_NodeIdx];
                node.TimeNode = timeNode;
                node.RemainDamage = remainDamage;
                node.Obstacle = obstacle;
                node.TriggerPoint = triggerPoint;
                node.Triggred = false;
                node.HitType = hitType;
            }
            ++_NodeIdx;
        }

        public void ClearNode()
        {
            _NodeIdx = 0;
        }

        public int GetPassedIdxByTime(float time)
        {
            for (int i = 0; i < _NodeIdx - 1; ++i)
            {
                if (time > _Nodes[i].TimeNode && time < _Nodes[i + 1].TimeNode)
                    return i;
            }
            return _NodeIdx - 1;
        }

        /// <summary>
        /// Get Projectile Current Position By Time
        /// </summary>
        /// <param name="time">input TimeMgr.GetCurrentTime() here</param>
        /// <returns></returns>
        public Vector3 GetPositionByTime(float time)
        {
            float val = (time - _StartTime)/_TimeClipLen;
            return Vector3.Lerp(_StartPos, _EndPos, val);
        }

        /// <summary>
        /// Get Projectile Remain Damage By Time
        /// </summary>
        /// <param name="time">input TimeMgr.GetCurrentTime() here</param>
        /// <returns></returns>
        public float GetRemainDmgByTime(float time)
        {
            return _Nodes[GetPassedIdxByTime(time)].RemainDamage;
        }

        /// <summary>
        /// update damage line
        /// </summary>
        /// <param name="time">input TimeMgr.Instance.GetCurrentTime()</param>
        /// <param name="dmgLost"></param>
        /// <returns>last hit point after update</returns>
        public Vector3 UpdateDmgLine(float time, float dmgLost)
        {
            int idx = GetPassedIdxByTime(time);
            Vector3 retPos = _Nodes[_NodeIdx - 1].TriggerPoint;
            if (idx >= NodeCount)
                return retPos;
            for (int i = idx; i < NodeCount; ++i)
            {
                _Nodes[i].RemainDamage -= dmgLost;
                if (_Nodes[i].RemainDamage <= 0)
                {
                    retPos = _Nodes[i].TriggerPoint;
                }
            }
            return retPos;
        }

        /// <summary>
        /// call to force terminate a DmgLine
        /// </summary>
        public void Terminate()
        {
            //TODO: Finish Terminate function
        }
    }
}