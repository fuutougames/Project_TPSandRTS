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
    public static float Accuracy;
    public static float FireRate;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        Accuracy = EditorGUILayout.FloatField("Accuracy: ", Accuracy);
        FireRate = EditorGUILayout.FloatField("FireRate: ", FireRate);
        if (GUILayout.Button("SetGunBattleData"))
        {
            GunBattleData data = new GunBattleData();
            data.Accuracy = Accuracy;
            data.FireRate = FireRate;
            ((AssultRifle) target).Init(data);
        }
    }
}
#endif
