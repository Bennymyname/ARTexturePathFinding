using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public static class BladeTools
{
    [MenuItem("Tools/Grass/Refresh All Grass Tiles")]
    public static void RefreshAllTiles()
    {
        int count = 0;
        BladeSpawner[] spawners = GameObject.FindObjectsOfType<BladeSpawner>();

        foreach (BladeSpawner spawner in spawners)
        {
            if (!spawner.gameObject.scene.IsValid()) continue;

            spawner.ClearBlades();
            spawner.SpawnBlades();
            count++;
        }

        Debug.Log($"✅ Refreshed {count} grass tile(s) in the scene.");
        EditorSceneManager.MarkAllScenesDirty();
    }

    [MenuItem("Tools/Grass/Bake Grass Into Static Meshes")]
    public static void BakeAllTiles()
    {
        int count = 0;
        BladeSpawner[] spawners = GameObject.FindObjectsOfType<BladeSpawner>();

        foreach (BladeSpawner spawner in spawners)
        {
            if (!spawner.gameObject.scene.IsValid()) continue;
            if (spawner.bladesParent == null || spawner.bladesParent.childCount == 0)
            {
                Debug.LogWarning($"⚠️ Skipping '{spawner.name}' - no blades to bake.");
                continue;
            }

            // Mark each blade static so it can be batched
            foreach (Transform blade in spawner.bladesParent)
            {
                blade.gameObject.isStatic = true;
            }

            // Perform static batching
            StaticBatchingUtility.Combine(spawner.bladesParent.gameObject);

            // Disable the spawner so blades don't get regenerated
            spawner.enabled = false;
            count++;
        }

        Debug.Log($"✅ Baked {count} grass tile(s) into static geometry.");
        EditorSceneManager.MarkAllScenesDirty();
    }
}
