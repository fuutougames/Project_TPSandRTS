#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(StaticObstacleData))]
[CanEditMultipleObjects]
public class StaticObstacleDataEditor : Editor {

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Setup"))
        {
            ((StaticObstacleData)target).Initialize();
        }
    }
}
#endif

