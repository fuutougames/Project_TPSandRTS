using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfTestRoutine : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Dictionary<int, Config.ContractConf>.Enumerator iter = Config.ContractConfIndex.GetInstance().GetData().GetEnumerator();
        while (iter.MoveNext())
        {
            Debug.Log("ConfItem: " + iter.Current.Key + "; " + iter.Current.Value.ToString());
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
