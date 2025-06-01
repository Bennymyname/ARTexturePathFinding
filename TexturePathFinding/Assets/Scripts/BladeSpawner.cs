using UnityEngine;
using UnityEditor;

[ExecuteAlways]
public class BladeSpawner : MonoBehaviour
{
    [Header("Prefab Settings")]
    public GameObject bladePrefab;

    [Header("Blade Count")]
    [Range(10, 2000)] public int bladeCount = 500;

    [Header("Blade Height (Y Scale)")]
    [Range(0.1f, 2f)] public float minBladeHeight = 0.8f;
    [Range(0.1f, 2f)] public float maxBladeHeight = 1.2f;

    [Header("Random Rotation (Euler Degrees)")]
    public Vector3 minRotation = new Vector3(0f, 0f, 0f);
    public Vector3 maxRotation = new Vector3(10f, 360f, 10f);

    [Header("Placement Adjustment")]
    [Range(-1f, 1f)] public float verticalOffset = 0f;
    [Range(-1f, 1f)] public float zOffset = 0f;

    [HideInInspector]
    public Transform bladesParent;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode) return;
        if (!gameObject.scene.IsValid()) return; // ✅ Only in-scene objects

        EditorApplication.delayCall += () =>
        {
            if (this != null && gameObject.scene.IsValid())
            {
                ClearBlades();
                SpawnBlades();
            }
        };
    }
#endif

    public void ClearBlades()
    {
#if UNITY_EDITOR
        if (!gameObject.scene.IsValid())
        {
            Debug.LogWarning($"⛔ Skipping ClearBlades(): {name} is a prefab asset.");
            return;
        }

        if (bladesParent != null)
        {
            DestroyImmediate(bladesParent.gameObject);
        }

        GameObject newParent = new GameObject("BladesParent");
        bladesParent = newParent.transform;
        bladesParent.SetParent(transform);
        bladesParent.localPosition = Vector3.zero;
        bladesParent.localRotation = Quaternion.identity;
        bladesParent.localScale = Vector3.one;
#endif
    }

    public void SpawnBlades()
    {
#if UNITY_EDITOR
        if (!gameObject.scene.IsValid())
        {
            Debug.LogWarning($"⛔ Skipping SpawnBlades(): {name} is a prefab asset.");
            return;
        }

        if (bladePrefab == null)
        {
            Debug.LogWarning("❗ Blade prefab not assigned.");
            return;
        }

        if (bladesParent == null)
        {
            ClearBlades();
        }

        MeshRenderer renderer = GetComponentInChildren<MeshRenderer>();
        MeshFilter filter = renderer?.GetComponent<MeshFilter>();
        if (renderer == null || filter == null) return;

        Mesh mesh = filter.sharedMesh;
        if (mesh == null) return;

        Bounds localBounds = mesh.bounds;

        for (int i = 0; i < bladeCount; i++)
        {
            float randX = Random.Range(localBounds.min.x, localBounds.max.x);
            float randZ = Random.Range(localBounds.min.z, localBounds.max.z);
            float localY = localBounds.min.y;

            Vector3 localPos = new Vector3(randX, localY, randZ);
            Vector3 worldPos = renderer.transform.TransformPoint(localPos)
                                + new Vector3(0f, verticalOffset, zOffset);

            GameObject blade = (GameObject)PrefabUtility.InstantiatePrefab(bladePrefab);
            blade.transform.SetParent(bladesParent);
            blade.transform.position = worldPos;

            Vector3 baseScale = bladePrefab.transform.localScale;
            float randomHeight = Random.Range(minBladeHeight, maxBladeHeight);
            blade.transform.localScale = new Vector3(baseScale.x, baseScale.y * randomHeight, baseScale.z);

            Vector3 randEuler = new Vector3(
                Random.Range(minRotation.x, maxRotation.x),
                Random.Range(minRotation.y, maxRotation.y),
                Random.Range(minRotation.z, maxRotation.z)
            );
            blade.transform.rotation = Quaternion.Euler(randEuler);
        }
#endif
    }
}
