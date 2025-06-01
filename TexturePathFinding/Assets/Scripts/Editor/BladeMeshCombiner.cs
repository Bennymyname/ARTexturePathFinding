using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class BladeMeshCombiner
{
    [MenuItem("Tools/Grass/Combine Grass Meshes Into One")]
    public static void CombineAllTiles()
    {
        int count = 0;
        BladeSpawner[] spawners = Object.FindObjectsOfType<BladeSpawner>();

        foreach (BladeSpawner spawner in spawners)
        {
            if (!spawner.gameObject.scene.IsValid()) continue;
            if (spawner.bladesParent == null || spawner.bladesParent.childCount == 0) continue;

            GameObject parentObj = spawner.bladesParent.gameObject;

            var meshFilters = parentObj.GetComponentsInChildren<MeshFilter>();
            if (meshFilters.Length <= 1)
            {
                Debug.LogWarning($"⚠️ Skipping '{spawner.name}' – not enough meshes to combine.");
                continue;
            }

            CombineInstance[] combine = new CombineInstance[meshFilters.Length - 1];
            Material sharedMat = null;

            for (int i = 1; i < meshFilters.Length; i++) // Skip the parent at index 0
            {
                MeshFilter mf = meshFilters[i];
                if (mf.sharedMesh == null) continue;

                combine[i - 1] = new CombineInstance
                {
                    mesh = mf.sharedMesh,
                    transform = parentObj.transform.worldToLocalMatrix * mf.transform.localToWorldMatrix
                };

                if (sharedMat == null)
                {
                    MeshRenderer mr = mf.GetComponent<MeshRenderer>();
                    if (mr != null)
                        sharedMat = mr.sharedMaterial;
                }
            }

            Mesh combinedMesh = new Mesh();
            combinedMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            combinedMesh.CombineMeshes(combine, true, true);

            // Remove all children
            for (int i = parentObj.transform.childCount - 1; i >= 0; i--)
            {
                Object.DestroyImmediate(parentObj.transform.GetChild(i).gameObject);
            }

            // Add mesh components
            MeshFilter finalMF = parentObj.GetComponent<MeshFilter>();
            if (finalMF == null) finalMF = parentObj.AddComponent<MeshFilter>();
            finalMF.sharedMesh = combinedMesh;

            MeshRenderer finalMR = parentObj.GetComponent<MeshRenderer>();
            if (finalMR == null) finalMR = parentObj.AddComponent<MeshRenderer>();
            finalMR.sharedMaterial = sharedMat;

            // Disable spawner
            spawner.enabled = false;

            Debug.Log($"✅ Combined blades under {spawner.name}.");
            count++;
        }

        EditorSceneManager.MarkAllScenesDirty();
        Debug.Log($"🌿 Done! Combined grass meshes for {count} tile(s).");
    }
}
