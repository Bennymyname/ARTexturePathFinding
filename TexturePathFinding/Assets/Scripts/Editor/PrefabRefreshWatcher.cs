using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PrefabRefreshWatcher : AssetPostprocessor
{
    static void OnPostprocessPrefab(GameObject prefab)
    {
        // Only respond to prefab saves from Project window
        if (!PrefabUtility.IsPartOfPrefabAsset(prefab)) return;

        string prefabName = prefab.name;

        // You can customize this to match specific prefab names
        if (!prefabName.StartsWith("GrassTile")) return;

        // Search all open scenes for matching instances
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            if (!scene.isLoaded) continue;

            GameObject[] rootObjects = scene.GetRootGameObjects();

            foreach (GameObject root in rootObjects)
            {
                BladeSpawner[] spawners = root.GetComponentsInChildren<BladeSpawner>(true);

                foreach (BladeSpawner spawner in spawners)
                {
                    GameObject source = PrefabUtility.GetCorrespondingObjectFromSource(spawner.gameObject);
                    if (source != null && source.name == prefabName)
                    {
                        Debug.Log($"🔁 Auto-refreshing '{spawner.name}' in scene due to prefab update.");

                        spawner.ClearBlades();
                        spawner.SpawnBlades();
                        EditorUtility.SetDirty(spawner.gameObject);
                    }
                }
            }
        }

        EditorSceneManager.MarkAllScenesDirty(); // Prompt save dialog
    }
}