using UnityEngine;
using UnityEditor;

public class GrassTileRefresher : MonoBehaviour
{
    [MenuItem("Tools/Grass Tiles/Refresh All GrassTile_01")]
    static void RefreshAllTile01()
    {
        RefreshTilesByName("GrassTile_01");
    }

    [MenuItem("Tools/Grass Tiles/Refresh All GrassTile_02")]
    static void RefreshAllTile02()
    {
        RefreshTilesByName("GrassTile_02");
    }

    static void RefreshTilesByName(string prefabName)
    {
        int count = 0;
        BladeSpawner[] allSpawners = GameObject.FindObjectsOfType<BladeSpawner>(true);

        foreach (BladeSpawner spawner in allSpawners)
        {
            if (PrefabUtility.GetCorrespondingObjectFromSource(spawner.gameObject)?.name == prefabName)
            {
                spawner.ClearBlades();
                spawner.SpawnBlades();
                count++;
            }
        }

        Debug.Log($"✅ Refreshed {count} tiles for prefab: {prefabName}");
    }
}
