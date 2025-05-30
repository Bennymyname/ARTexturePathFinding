using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BladeSpawner))]
public class BladeSpawnerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        BladeSpawner spawner = (BladeSpawner)target;

        GUILayout.Space(10);
        if (GUILayout.Button("🌱 Refresh Blades"))
        {
            spawner.ClearBlades();
            spawner.SpawnBlades();
        }

        if (GUILayout.Button("🧹 Clear Blades"))
        {
            spawner.ClearBlades();
        }
    }
}
