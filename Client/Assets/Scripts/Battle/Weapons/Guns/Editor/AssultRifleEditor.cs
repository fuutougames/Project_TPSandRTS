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
        if (GUILayout.Button("SetGunBattleData"))
        {
            GunBattleData data = new GunBattleData();
            data.Accuracy = ((AssultRifle) target).Accuracy;
            data.FireRate = ((AssultRifle) target).FireRate;
            ((AssultRifle) target).Init(data);
        }
    }
}
#endif
