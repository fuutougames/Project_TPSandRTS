using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle.Data
{
    public class BattleSceneData
    {
        private Dictionary<int, ObstacleData> _ObstacleDict;
        private Dictionary<int, DynamicObstacleData> _DynamicObstacleDict; 

        public void Reset()
        {
            if (_ObstacleDict == null)
                _ObstacleDict = new Dictionary<int, ObstacleData>();
            _ObstacleDict.Clear();
        }

        /// <summary>
        /// Get Obstacle data By Obstacle's transform instance id;
        /// </summary>
        /// <param name="id">obstacle's transform instance id</param>
        /// <returns></returns>
        public ObstacleData GetObstacleDataByInstanceID(int id)
        {
            ObstacleData data;
            _ObstacleDict.TryGetValue(id, out data);
            return data;
        }

        public void RegisterObstacle(ObstacleData data)
        {
            _ObstacleDict.Add(data.transform.GetInstanceID(), data);
        }

        public void UnRegisterObstacle(ObstacleData data)
        {
            _ObstacleDict.Remove(data.transform.GetInstanceID());
        }

        public void ClearSceneData()
        {
            _ObstacleDict.Clear();
        }
    }
}