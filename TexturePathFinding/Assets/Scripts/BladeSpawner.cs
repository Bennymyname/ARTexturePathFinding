using UnityEngine;

[ExecuteAlways]
public class BladeSpawner : MonoBehaviour
{
    [Header("Prefab Settings")]
    public GameObject bladePrefab;

    [Header("Blade Placement")]
    [Range(10, 2000)] public int bladeCount = 500;
    [Tooltip("Offsets blade vertically to fix pivot position (use small values like 0.0 to 0.1).")]
    [Range(-1f, 1f)] public float verticalOffset = 0f;

    [Header("Blade Height (Y Scale)")]
    [Range(0.1f, 2f)] public float minBladeHeight = 0.8f;
    [Range(0.1f, 2f)] public float maxBladeHeight = 1.2f;

    [Header("Random Rotation")]
    [Range(0f, 45f)] public float minXRot = 0f;
    [Range(0f, 45f)] public float maxXRot = 10f;
    [Range(0f, 360f)] public float minYRot = 0f;
    [Range(0f, 360f)] public float maxYRot = 360f;
    [Range(0f, 45f)] public float minZRot = 0f;
    [Range(0f, 45f)] public float maxZRot = 10f;

    [HideInInspector] public Transform bladesParent;

    public void ClearBlades()
    {
#if UNITY_EDITOR
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
        if (bladePrefab == null) return;

        if (bladesParent == null)
        {
            ClearBlades();
        }

        // Get ground bounds
        Renderer groundRenderer = GetComponentInChildren<Renderer>();
        if (groundRenderer == null) return;

        Bounds bounds = groundRenderer.bounds;
        Vector3 tileMin = bounds.min;
        Vector3 tileMax = bounds.max;

        for (int i = 0; i < bladeCount; i++)
        {
            float randX = Random.Range(tileMin.x, tileMax.x);
            float randZ = Random.Range(tileMin.z, tileMax.z);
            float baseY = bounds.min.y;

            Vector3 spawnPos = new Vector3(randX, baseY, randZ);

            GameObject blade = (GameObject)UnityEditor.PrefabUtility.InstantiatePrefab(bladePrefab);
            blade.transform.SetParent(bladesParent);
            blade.transform.position = spawnPos + new Vector3(0, verticalOffset, 0);

            // Blade scaling
            Vector3 baseScale = bladePrefab.transform.localScale;
            float randomHeight = Random.Range(minBladeHeight, maxBladeHeight);
            blade.transform.localScale = new Vector3(baseScale.x, baseScale.y * randomHeight, baseScale.z);

            // Full random rotation
            float xRot = Random.Range(minXRot, maxXRot);
            float yRot = Random.Range(minYRot, maxYRot);
            float zRot = Random.Range(minZRot, maxZRot);
            blade.transform.rotation = Quaternion.Euler(xRot, yRot, zRot);
        }
#endif
    }
}
