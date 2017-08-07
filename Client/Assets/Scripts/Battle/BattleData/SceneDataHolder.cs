using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneDataHolder : MonoBase
{
    [SerializeField] public List<StaticObstacleData> _ObstacleDataList;
    [SerializeField] public List<DynamicObstacleData> _DynamicObstacleList;
}
