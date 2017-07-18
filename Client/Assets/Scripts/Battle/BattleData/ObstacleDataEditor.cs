#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ObstacleData))]
[CanEditMultipleObjects]
public class ObstacleDataEditor : Editor {

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Setup"))
        {
            ((ObstacleData)target).Initialize();
        }
    }
}
#endif

