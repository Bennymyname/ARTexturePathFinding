using UnityEngine;
using System.Collections;
using System.IO;
using System.Linq;

public class NormalMapBatchCapture : MonoBehaviour
{
    public Renderer targetRenderer;
    public Camera captureCamera;
    public GameObject targetObject;
    public string normalMapFolder = "NormalMaps"; // Folder inside Resources
    public string saveFolder = "Screenshots";

    private string[] normalMapNames;

    void Start()
    {
        LoadNormalMapNames();
        StartCoroutine(CaptureAll());
    }

    void LoadNormalMapNames()
    {
        normalMapNames = Resources.LoadAll<Texture2D>(normalMapFolder)
                                   .Select(t => t.name)
                                   .ToArray();
    }

    IEnumerator CaptureAll()
{
    string fullPath = Path.Combine(Application.dataPath, saveFolder);
    if (!Directory.Exists(fullPath))
    {
        Directory.CreateDirectory(fullPath);
    }

    foreach (string name in normalMapNames)
    {
        Texture2D normalMap = Resources.Load<Texture2D>($"{normalMapFolder}/{name}");
        if (normalMap == null)
        {
            Debug.LogError($"❌ Missing normal map: {name}");
            continue;
        }

        // Apply the normal map
        Material mat = targetRenderer.material;
        mat.EnableKeyword("_NORMALMAP");
        mat.SetTexture("_BumpMap", normalMap);

        yield return new WaitForEndOfFrame();

        // Full save path
        string path = Path.Combine(fullPath, $"{name}.png");
        ScreenCapture.CaptureScreenshot(path);
        Debug.Log($"✅ Captured: {path}");

        yield return new WaitForSeconds(0.5f);
    }

    Debug.Log("🎉 Done capturing all normal maps.");
}

}
