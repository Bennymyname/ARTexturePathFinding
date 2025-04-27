using UnityEngine;
using UnityEditor;

public class TelemetryConsentAssetCreator
{
    [MenuItem("Meta XR/Disable Telemetry Consent Pop-Up")]
    public static void CreateConsentAsset()
    {
        TelemetryConsent asset = ScriptableObject.CreateInstance<TelemetryConsent>();

        asset.HasUserApprovedTelemetry = false;
        asset.HasPromptedForTelemetry = true;

        string path = "Assets/Resources/TelemetryConsent.asset";
        AssetDatabase.CreateAsset(asset, path);
        AssetDatabase.SaveAssets();
        
        Debug.Log("[Meta XR] Created fake TelemetryConsent.asset to block popup.");
    }
}

// Class that matches the internal Meta structure
public class TelemetryConsent : ScriptableObject
{
    public bool HasUserApprovedTelemetry;
    public bool HasPromptedForTelemetry;
}
