using System.Collections;
using System.Collections.Generic;
using Battle;
using UnityEngine;

/// <summary>
/// collider center must be vector3.zero
/// </summary>
public class ObstacleData : MonoBase
{
    [SerializeField] public float Hardness;
    [SerializeField] public bool Penetrable;
    [SerializeField] public Plane[] CollideSurfaces;
    [SerializeField] public BoxCollider Collider;
    [SerializeField] public Transform CachedTrans;

#if UNITY_EDITOR
    /// <summary>
    /// Editor only
    /// </summary>
    public void Initialize()
    {
        CachedTrans = transform;
        Collider = GetComponent<BoxCollider>();

        Vector3 size = new Vector3(
            Collider.size.x*CachedTrans.lossyScale.x,
            Collider.size.y*CachedTrans.lossyScale.y,
            Collider.size.z*CachedTrans.lossyScale.z
            );

        Vector3 halfSize = size/2.0f;
        Vector3 zOffset = CachedTrans.forward*halfSize.z;
        Vector3 yOffset = CachedTrans.up*halfSize.y;
        Vector3 xOffset = CachedTrans.right*halfSize.x;

        CollideSurfaces = new Plane[]
        {
            //front
            new Plane(CachedTrans.forward, CachedTrans.position + zOffset),
            //rear
            new Plane(-CachedTrans.forward, CachedTrans.position - zOffset),
            //top
            new Plane(CachedTrans.up, CachedTrans.position + yOffset),
            //bottom
            new Plane(-CachedTrans.up, CachedTrans.position - yOffset),
            //right
            new Plane(CachedTrans.right, CachedTrans.position + xOffset),
            //left
            new Plane(-CachedTrans.right, CachedTrans.position - xOffset)
        };
    }
#endif

    void OnEnable()
    {
        BattleMgr.Instance.SceneData.RegisterObstacle(this);
    }

    void OnDisable()
    {
        BattleMgr.Instance.SceneData.UnRegisterObstacle(this);
    }
}
