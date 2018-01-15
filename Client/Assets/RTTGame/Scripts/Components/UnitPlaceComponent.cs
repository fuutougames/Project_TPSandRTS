using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UnitPlaceComponent : MonoBehaviour
{
    public GameObject unitPrefab;
    public int maxUnitNum;
    private Ray ray;
    private RaycastHit hitInfo;

    public Action OnPlaceComplete;

    private int curUnitNum = 0;
	// Update is called once per frame
	void Update ()
    {
		if(Input.GetMouseButtonDown(0))
        {
            if (curUnitNum >= maxUnitNum) return;

            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(ray, out hitInfo))
            {
                if (hitInfo.transform.gameObject.layer != 8) return;
                GameObject go_unit = Instantiate(unitPrefab) as GameObject;
                go_unit.transform.position = new Vector3(hitInfo.point.x, hitInfo.point.y, hitInfo.point.z);
                go_unit.AddComponent<UnitComponent>();
                go_unit.SetActive(true);
                curUnitNum += 1;
                if (curUnitNum >= maxUnitNum) OnPlaceComplete();
            }
        }
    }
}
