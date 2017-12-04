using System.Collections;
using System.Collections.Generic;
using Battle;
using Battle.Projectiles;
using UnityEngine;

/// <summary>
/// static obstacle data
/// collider center must be vector3.zero
/// </summary>
public class StaticObstacleData : MonoBase
{
    [SerializeField] public float Hardness;
    [SerializeField] public bool Penetrable;
    [SerializeField] public Plane[] CollideSurfaces;
    [SerializeField] public BoxCollider Collider;

    /// <summary>
    /// Init Plane data
    /// </summary>
    public void Initialize()
    {
        Collider = GetComponent<BoxCollider>();

        Vector3 size = new Vector3(
            Collider.size.x* CachedTransform.lossyScale.x,
            Collider.size.y* CachedTransform.lossyScale.y,
            Collider.size.z* CachedTransform.lossyScale.z
            );

        Vector3 halfSize = size/2.0f;
        Vector3 zOffset = CachedTransform.forward*halfSize.z;
        Vector3 yOffset = CachedTransform.up*halfSize.y;
        Vector3 xOffset = CachedTransform.right*halfSize.x;

        CollideSurfaces = new Plane[]
        {
            //front
            new Plane(CachedTransform.forward, CachedTransform.position + zOffset),
            //rear
            new Plane(-CachedTransform.forward, CachedTransform.position - zOffset),
            //top
            new Plane(CachedTransform.up, CachedTransform.position + yOffset),
            //bottom
            new Plane(-CachedTransform.up, CachedTransform.position - yOffset),
            //right
            new Plane(CachedTransform.right, CachedTransform.position + xOffset),
            //left
            new Plane(-CachedTransform.right, CachedTransform.position - xOffset)
        };
    }

    void OnEnable()
    {
        //BattleMgr.Instance.SceneData.RegisterObstacle(this);
    }

    void OnDisable()
    {
        //BattleMgr.Instance.SceneData.UnRegisterObstacle(this);
    }

    /// <summary>
    /// when collide with a projectile, this function will be called;
    /// </summary>
    /// <param name="hitPoint"></param>
    /// <param name="projectile"></param>
    /// <param name="hitType"></param>
    /// <param name="pType"></param>
    public virtual void OnProjectileCollide(ProjectileBase projectile, Vector3 hitPoint,
        PROJECTILE_HITTYPE hitType, PROJECTILE_TYPE pType)
    {

        Debug.Log("Static Obstacle Hit!!!");
    }
}
