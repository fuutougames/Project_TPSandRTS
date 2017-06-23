
#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ProjectileBase))]
[CanEditMultipleObjects]
public class ProjectileBaseEditor : Editor
{
    private Transform _StartTransform;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        _StartTransform = (Transform) EditorGUILayout.ObjectField(_StartTransform, typeof (Transform));
        if (GUILayout.Button("Trigger"))
        {
            //if (_StartTransform != null)
            //    ((ProjectileBase) target).TriggerProjectile(_StartTransform);
        }
    }
}
#endif