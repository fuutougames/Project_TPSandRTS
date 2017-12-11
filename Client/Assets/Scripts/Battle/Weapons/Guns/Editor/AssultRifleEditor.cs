#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using Battle.Guns;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AssultRifle))]
[CanEditMultipleObjects]
public class AssultRifleEditor : Editor
{
    //public float Accuracy;
    //public float FireRate;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        //Accuracy = EditorGUILayout.FloatField("Accuracy: ", Accuracy);
        //FireRate = EditorGUILayout.FloatField("FireRate: ", FireRate);
        AssultRifle ins = (AssultRifle) target;
        if (GUILayout.Button("SetGunBattleData"))
        {
            GunBattleData data = new GunBattleData();
            data.Accuracy = ins.Accuracy;
            data.FireRate = ins.FireRate;
            ins.SetFireMode(ins.FireMode);
            ins.MagCapacity = 100;
            ins.Init(data);
        }
        if (GUILayout.Button("Reload"))
        {
            ins.Reload();
        }
    }
}
#endif
