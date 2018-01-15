using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitCommandComponent : MonoBehaviour
{
    public UnitSelectionComponent selectionComp;

    //private UnitMode curLeaderMode = UnitMode.IDLE;
    //private UnitMode curMateMode = UnitMode.IDLE;
    private UnitMode commanderMode = UnitMode.MOVE;

    private Ray ray;
    private float rayDistance;
    private RaycastHit hitInfo;
    private Plane groundPlane;
    private GameObject movetarget;
    private Vector3 hitPoint;

    // Use this for initialization
    void Start()
    {
        selectionComp = this.GetComponent<UnitSelectionComponent>();
        groundPlane = new Plane(Vector3.up, Vector3.up * 0.58f);
        movetarget = new GameObject();
        movetarget.name = "movetarget";

        selectionComp.OnSelectionRest = OnSelectionRest;
    }

    private void OnSelectionRest()
    {
        foreach (SelectableUnitComponent unit in selectionComp.selectedObjects)
        {
            UnitModeController unitCtrl = unit.gameObject.GetComponent<UnitModeController>();
            //unitCtrl.SetMoveTarget(null);
            //unitCtrl.SetFollowTarget(null);
            if(unitCtrl._mode != UnitMode.AIM)
                unitCtrl.SwitchMode(UnitMode.IDLE);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            commanderMode = UnitMode.PATHMOVE;
            int count = 0;
            foreach (SelectableUnitComponent unit in selectionComp.selectedObjects)
            {
                UnitModeController unitCtrl = unit.gameObject.GetComponent<UnitModeController>();
                if (count == 0)
                {
                    unitCtrl.SwitchMode(UnitMode.PATHMOVE);
                }
                else
                {
                    unitCtrl.SwitchMode(UnitMode.FOLLOW);
                }
                count++;
            }
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            commanderMode = UnitMode.MOVE;
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            foreach (SelectableUnitComponent unit in selectionComp.selectedObjects)
            {
                ShootableUnitComponent shootunit = unit.gameObject.GetComponent<ShootableUnitComponent>();
                StartCoroutine(shootunit.Shoot());
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            if (selectionComp.selectedObjects.Count == 0) return;
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hitInfo))
            {
                if (hitInfo.transform.tag == "Enemy")
                {
                    // TODO : Set All Unit to Aim Mode and pass the target
                    foreach (SelectableUnitComponent unit in selectionComp.selectedObjects)
                    {
                        UnitModeController unitCtrl = unit.gameObject.GetComponent<UnitModeController>();
                        unitCtrl.SwitchMode(UnitMode.AIM);
                        unitCtrl.SetAimTarget(hitInfo.transform.gameObject);
                    }
                    return;
                }
            }

            //if (groundPlane.Raycast(ray, out rayDistance))
            if (Physics.Raycast(ray, out hitInfo))
            {
                if (hitInfo.transform.gameObject.layer != 8 &&
                    hitInfo.transform.gameObject.layer != 11) return;
                //hitPoint = ray.GetPoint(rayDistance);
                int count = 0;
                if(hitInfo.transform.gameObject.layer == 11)
                {

                    //return;
                }
                if (commanderMode == UnitMode.MOVE)
                {
                    //movetarget.transform.position = hitPoint;
                    movetarget.transform.position = hitInfo.point;
                    // TODO : Set All Unit to Move Mode and pass the target    
                    foreach (SelectableUnitComponent unit in selectionComp.selectedObjects)
                    {
                        UnitModeController unitCtrl = unit.gameObject.GetComponent<UnitModeController>();
                        if (count == 0)
                        {
                            unitCtrl.SwitchMode(UnitMode.MOVE);
                            unitCtrl.SetMoveTarget(movetarget);
                        }
                        else
                        {
                            unitCtrl.SwitchMode(UnitMode.FOLLOW);
                            unitCtrl.SetFollowTarget(selectionComp.selectedObjects[0].gameObject);
                        }
                        count += 1;
                    }
                }
                else if (commanderMode == UnitMode.PATHMOVE)
                {
                    GameObject go_wayPoint = new GameObject();
                    //go_wayPoint.transform.position = hitPoint;
                    go_wayPoint.transform.position = hitInfo.point;
                    foreach (SelectableUnitComponent unit in selectionComp.selectedObjects)
                    {
                        UnitModeController unitCtrl = unit.gameObject.GetComponent<UnitModeController>();
                        if (count == 0)
                        {
                            unitCtrl.SwitchMode(UnitMode.PATHMOVE);
                            unitCtrl.SetMovePath(go_wayPoint);
                        }
                        else
                        {
                            unitCtrl.SwitchMode(UnitMode.FOLLOW);
                            unitCtrl.SetFollowTarget(selectionComp.selectedObjects[0].gameObject);
                        }
                        count += 1;
                    }
                }
            }
        }
    }
}
