#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RayTest))]
[CanEditMultipleObjects]
public class RayTestEditor : Editor {

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Raycast"))
        {
            ((RayTest)target).DoRaycast();
        }
    }
}
#endif
