#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ProjectileTest))]
[CanEditMultipleObjects]
public class ProjectileTestEditor : Editor {


    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Register Objects"))
        {
            ((ProjectileTest)target).RegisterObjects();
        }
        if (GUILayout.Button("Trigger"))
        {
            ((ProjectileTest)target).TriggerProjectiles();
        }
    }
}
#endif