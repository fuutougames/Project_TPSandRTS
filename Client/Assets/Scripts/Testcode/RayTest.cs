using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayTest : MonoBehaviour
{

    public Transform CachedTrans;

    public void DoRaycast()
    {
        Ray ray = new Ray(CachedTrans.position, CachedTrans.forward);
        Physics.queriesHitBackfaces = true;
        RaycastHit[] hits = Physics.RaycastAll(ray, 100000);
        for (int i = 0; i < hits.Length; ++i)
        {
            Debug.Log(hits[i].point);
        }
        Physics.queriesHitBackfaces = false;
    }
}
