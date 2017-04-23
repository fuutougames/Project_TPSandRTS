using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Commander : MonoBase
{
    public GameObject debugGO;
    private Camera viewCamera;
    private MapGenerator map;
    private Solider soliderPrefab;
    private Solider curSolider;

    protected override void OnAwake()
    {
        base.OnAwake();
        viewCamera = Camera.main;
        groundPlane = new Plane(Vector3.up, Vector3.zero);
        map = FindObjectOfType<MapGenerator>();
        soliderPrefab = ResourceManager.Instance.LoadResource<Solider>("Prefabs/Solider");
    }

    Ray ray;
    Plane groundPlane;
    float rayDistance;
    Vector3 hitPoint;
    Vector3 curRegionPos;
    Vector2 regionSize = new Vector2(4, 4);
    protected override void OnUpdate()
    {
        base.OnUpdate();
        ray = viewCamera.ScreenPointToRay(Input.mousePosition);
        
        
        if(groundPlane.Raycast(ray, out rayDistance))
        {
            hitPoint = ray.GetPoint(rayDistance);
            Debug.DrawLine(ray.origin, hitPoint, Color.red);
            curRegionPos = map.GetRegionPosFromPosition(hitPoint, new Vector2(4, 4));
            debugGO.transform.position = curRegionPos;
        }

        if(Input.GetMouseButtonUp(0))
        {
            if(curSolider == null)
            {
                StartCoroutine(SpawnSolider(curRegionPos));
            }
            else
            {
                StartCoroutine(SetSoliderTargetPos(curRegionPos));
            }
        }
    }

    IEnumerator SpawnSolider(Vector3 sendPos)
    {
        float spawnDelay = 1;
        float tileFlashSpeed = 4;
        yield return null;
        Transform spawnTile = map.GetRandomOpenTileFromRegion(map.CalRegionIndexFromPos(sendPos, regionSize), regionSize);

        Material tileMat = spawnTile.GetComponent<Renderer>().material;
        Color initialColor = Color.white;
        Color flashColor = Color.red;
        float spawnTimer = 0;

        while (spawnTimer < spawnDelay)
        {
            tileMat.color = Color.Lerp(initialColor, flashColor, Mathf.PingPong(spawnTimer * tileFlashSpeed, 1));
            spawnTimer += Time.deltaTime;
            yield return null;
        }

        Solider spawnedSolider = Instantiate(soliderPrefab, spawnTile.position + Vector3.up, Quaternion.identity) as Solider;
        curSolider = spawnedSolider;
    }

    IEnumerator SetSoliderTargetPos(Vector3 targetPos)
    {
        float setDelay = 1;
        float tileFlashSpeed = 4;
        yield return null;
        Transform targetTile = map.GetRandomOpenTileFromRegion(map.CalRegionIndexFromPos(targetPos, regionSize), regionSize);

        Material tileMat = targetTile.GetComponent<Renderer>().material;
        Color initialColor = Color.white;
        Color flashColor = Color.green;
        float orderTimer = 0;

        while (orderTimer < setDelay)
        {
            tileMat.color = Color.Lerp(initialColor, flashColor, Mathf.PingPong(orderTimer * tileFlashSpeed, 1));
            orderTimer += Time.deltaTime;
            yield return null;
        }

        curSolider.SetDestination(targetTile.position + Vector3.up);
    }
}
