using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Entrance))]
[CanEditMultipleObjects]
public class EntranceEditor : Editor {

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Add BAWPPU"))
        {
            Entrance ctrl = (Entrance)target;
            ctrl.AddBAWPPU();
        }

        if (GUILayout.Button("Remove BAWPPU"))
        {
            Entrance ctrl = (Entrance)target;
            ctrl.RemoveBAWPPU();
        }
    }
}
