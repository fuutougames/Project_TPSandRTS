using UnityEngine;
using UnityEditor;

public class YuME_editorConfig : EditorWindow
{
    public static YuME_editorData editorData;
    static float previousGlobalScale;

    [MenuItem("Window/Yuponic/Utils/Editor Config")]
    static void Initialize()
    {
        YuME_editorConfig editorConfig = EditorWindow.GetWindow<YuME_editorConfig>(true, "Editor Config");
        editorConfig.titleContent.text = "Editor Config";
    }

    void OnEnable()
    {
        editorData = ScriptableObject.CreateInstance<YuME_editorData>();
        string[] guids = AssetDatabase.FindAssets("YuME_editorSetupData");
        editorData = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guids[0]), typeof(YuME_editorData)) as YuME_editorData;
        previousGlobalScale = editorData.gridScaleFactor;
        checkForValidArrays();
    }

    void checkForValidArrays()
    {
        if(editorData.layerNames.Count < 8)
        {
            editorData.layerNames.Clear();
            for(int i = 1; i < 9; i++)
            {
                editorData.layerNames.Add("layer" + i);
            }
        }
        if (editorData.layerStatic.Count < 8)
        {
            editorData.layerStatic.Clear();
            for (int i = 1; i < 9; i++)
            {
                editorData.layerStatic.Add(true);
            }
        }
        if (editorData.layerFreeze.Count < 8)
        {
            editorData.layerFreeze.Clear();
            for (int i = 1; i < 9; i++)
            {
                editorData.layerFreeze.Add(true);
            }
        }
    }

    Vector2 _scrollPosition;

    void OnGUI()
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Editor Settings", EditorStyles.boldLabel);

        _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

        EditorGUILayout.BeginVertical("box");

        EditorGUILayout.LabelField("Cursor Color Settings", EditorStyles.boldLabel);

        editorData.brushCursorColor = EditorGUILayout.ColorField("Brush Cursor Color", editorData.brushCursorColor);
        editorData.pickCursorColor = EditorGUILayout.ColorField("Brush Picker Color", editorData.pickCursorColor);
        editorData.eraseCursorColor = EditorGUILayout.ColorField("Brush Erase Color", editorData.eraseCursorColor);

        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical("box");

        EditorGUILayout.LabelField("Icon Size", EditorStyles.boldLabel);
        editorData.iconWidth = (int)EditorGUILayout.Slider("Icon Size", editorData.iconWidth, 16f, 64f);

        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical("box");

        EditorGUILayout.LabelField("Invert Mouse wheel scrolling", EditorStyles.boldLabel);
        editorData.invertMouseWheel = EditorGUILayout.Toggle("Invert Mouse Wheel", editorData.invertMouseWheel);

        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical("box");

        EditorGUILayout.LabelField("Global Scale Setting", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("Warning. Changing the grid scale will have a knock on effect to all your maps. The default scale is 1. If you are seeing issues with your maps, please reset the scale.", MessageType.Warning);
        editorData.gridScaleFactor = EditorGUILayout.Slider("Grid Size", editorData.gridScaleFactor, 1f, 10f);

        if (previousGlobalScale != editorData.gridScaleFactor)
        {
            YuME_mapEditor.gridHeight = 0;
            editorData.gridScaleFactor = editorData.gridScaleFactor * 2;
            editorData.gridScaleFactor = Mathf.Round(editorData.gridScaleFactor) / 2;
            if (editorData.gridScaleFactor < 1) { editorData.gridScaleFactor = 1; }
            if (editorData.gridScaleFactor > 10) { editorData.gridScaleFactor = 10; }
            previousGlobalScale = editorData.gridScaleFactor;
        }
        
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical("box");

        EditorGUILayout.LabelField("Grid Color Settings", EditorStyles.boldLabel);

        editorData.gridColorNormal = EditorGUILayout.ColorField("Grid Color", editorData.gridColorNormal);
        editorData.gridColorFill = EditorGUILayout.ColorField("Fill Color", editorData.gridColorFill);
        editorData.gridColorBorder = EditorGUILayout.ColorField("Border Color", editorData.gridColorBorder);

        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("Grid Spawn Position", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        editorData.initialOffset = EditorGUILayout.Vector3Field("", editorData.initialOffset);
        editorData.initialOffset.x = editorData.initialOffset.x * 2;
        editorData.initialOffset.x = Mathf.Round(editorData.initialOffset.x) / 2;
        editorData.initialOffset.y = editorData.initialOffset.y * 2;
        editorData.initialOffset.y = Mathf.Round(editorData.initialOffset.y) / 2;
        editorData.initialOffset.z = editorData.initialOffset.z * 2;
        editorData.initialOffset.z = Mathf.Round(editorData.initialOffset.z) / 2;
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("Grid Offset", EditorStyles.boldLabel);
        YuME_mapEditor.gridOffset = (float)EditorGUILayout.Slider("Grid Offset", YuME_mapEditor.gridOffset, -0.25f, 0.25f);
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Layer Name", EditorStyles.boldLabel, GUILayout.Width(125));
        EditorGUILayout.LabelField("Freeze", EditorStyles.boldLabel, GUILayout.Width(75));
        EditorGUILayout.LabelField("Static", EditorStyles.boldLabel, GUILayout.Width(75));
        EditorGUILayout.EndHorizontal();

        for (int i = 0; i < YuME_mapEditor.editorData.layerNames.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            YuME_mapEditor.editorData.layerNames[i] = EditorGUILayout.TextField(YuME_mapEditor.editorData.layerNames[i], GUILayout.Width(125));
            YuME_mapEditor.editorData.layerFreeze[i] = EditorGUILayout.Toggle(YuME_mapEditor.editorData.layerFreeze[i], GUILayout.Width(75));
            YuME_mapEditor.editorData.layerStatic[i] = EditorGUILayout.Toggle(YuME_mapEditor.editorData.layerStatic[i], GUILayout.Width(75));
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndScrollView();

        if (GUI.changed)
        {
            SceneView.RepaintAll();
        }

    }
}
