using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class ShapeSetCreatorWindow : EditorWindow
{
    private List<string> shapeTexts = new List<string> { "" };
    private List<Vector2> textScrolls = new List<Vector2>();

    private Vector2 windowScroll;

    [MenuItem("Tools/Shape Set Creator")]
    public static void ShowWindow()
    {
        GetWindow<ShapeSetCreatorWindow>("Shape Set Creator");
    }

    void OnGUI()
    {
        // 전체 윈도우 스크롤뷰
        windowScroll = EditorGUILayout.BeginScrollView(windowScroll);

        GUILayout.Label("Multiple Shapes Input", EditorStyles.boldLabel);

        for (int i = 0; i < shapeTexts.Count; i++)
        {
            EditorGUILayout.LabelField("Shape " + (i + 1));

            // 스크롤 리스트 크기 보정
            while (textScrolls.Count <= i)
                textScrolls.Add(Vector2.zero);

            textScrolls[i] = EditorGUILayout.BeginScrollView(textScrolls[i], GUILayout.Height(150));
            shapeTexts[i] = EditorGUILayout.TextArea(shapeTexts[i], GUILayout.ExpandHeight(true));
            EditorGUILayout.EndScrollView();
        }

        if (GUILayout.Button("Add New Shape"))
        {
            shapeTexts.Add("");
            textScrolls.Add(Vector2.zero);
        }

        if (GUILayout.Button("Create ShapeSetData Asset"))
        {
            CreateShapeSetAsset();
        }

        EditorGUILayout.EndScrollView();
    }

    void CreateShapeSetAsset()
    {
        ShapeSetData shapeSetData = ScriptableObject.CreateInstance<ShapeSetData>();
        shapeSetData.shapeTexts = new List<string>(shapeTexts);

        string path = EditorUtility.SaveFilePanelInProject("Save ShapeSetData", "NewShapeSetData", "asset", "Save shape set data");
        if (!string.IsNullOrEmpty(path))
        {
            AssetDatabase.CreateAsset(shapeSetData, path);
            AssetDatabase.SaveAssets();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = shapeSetData;
        }
    }
}
