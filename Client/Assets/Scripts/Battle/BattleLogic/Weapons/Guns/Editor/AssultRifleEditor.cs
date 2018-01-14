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
            ins.SetFireMode(ins.FireMode);
            ins.Init();
        }
        if (GUILayout.Button("Reload"))
        {
            ins.Reload();
        }
    }
}
#endif
