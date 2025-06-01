using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class BladeSpawnerAutoRefresher : AssetPostprocessor
{
    // Called whenever a prefab is saved
    static void OnPostprocessPrefab(GameObject prefab)
    {
        if (prefab.name.StartsWith("GrassTile"))
        {
            Debug.Log($"📦 Prefab '{prefab.name}' was saved — refreshing scene instances...");

            // Find all scene instances
            BladeSpawner[] spawners = GameObject.FindObjectsOfType<BladeSpawner>();

            foreach (BladeSpawner spawner in spawners)
            {
                GameObject prefabRef = PrefabUtility.GetCorrespondingObjectFromSource(spawner.gameObject);

                if (prefabRef != null && prefabRef.name == prefab.name)
                {
                    spawner.ClearBlades();
                    spawner.SpawnBlades();
                }
            }

            // Mark scene dirty so user can save
            if (!Application.isPlaying)
            {
                EditorSceneManager.MarkAllScenesDirty();
            }
        }
    }
}
