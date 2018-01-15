using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameControl : MonoBehaviour
{
    public GameObject unitCommander;
    public UnitPlaceComponent placeCommander;
    public UnitSelectionComponent unitSelectionCom;
    public UnitCommandComponent unitCommanderCom;

    private void Start()
    {
        placeCommander = unitCommander.GetComponent<UnitPlaceComponent>();
        unitSelectionCom = unitCommander.GetComponent<UnitSelectionComponent>();
        unitCommanderCom = unitCommander.GetComponent<UnitCommandComponent>();

        placeCommander.OnPlaceComplete = () => {
            placeCommander.enabled = false;
            unitSelectionCom.enabled = true;
            unitCommanderCom.enabled = true;
        };
        unitSelectionCom.enabled = false;
        unitCommanderCom.enabled = false;
    }


}
