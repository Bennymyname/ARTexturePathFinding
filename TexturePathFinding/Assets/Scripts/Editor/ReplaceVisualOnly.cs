using UnityEngine;
using UnityEditor;

public class ReplaceVisualWindow : EditorWindow
{
    private GameObject visualPrefab;

    [MenuItem("Tools/Replace Visuals with Nested Prefab")]
    static void ShowWindow()
    {
        GetWindow<ReplaceVisualWindow>("Replace Visuals");
    }

    void OnGUI()
    {
        GUILayout.Label("Prefab to Nest", EditorStyles.boldLabel);
        visualPrefab = (GameObject)EditorGUILayout.ObjectField("Visual Prefab", visualPrefab, typeof(GameObject), false);

        if (GUILayout.Button("Nest into Selected"))
        {
            if (visualPrefab == null)
            {
                Debug.LogError("Please assign a prefab.");
                return;
            }

            foreach (GameObject obj in Selection.gameObjects)
            {
                if (PrefabUtility.IsPartOfPrefabInstance(obj)) continue;

                // Optional: remove old visuals
                var rend = obj.GetComponent<MeshRenderer>();
                if (rend) DestroyImmediate(rend);
                var filter = obj.GetComponent<MeshFilter>();
                if (filter) DestroyImmediate(filter);

                GameObject nested = (GameObject)PrefabUtility.InstantiatePrefab(visualPrefab);
                nested.transform.SetParent(obj.transform);
                nested.transform.localPosition = Vector3.zero;
                nested.transform.localRotation = Quaternion.identity;
                nested.transform.localScale = Vector3.one;

                nested.name = "Visual_GrassTile";
                Undo.RegisterCreatedObjectUndo(nested, "Nest Visual");
            }
        }
    }
}
