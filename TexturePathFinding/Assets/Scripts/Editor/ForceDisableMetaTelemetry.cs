using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class ForceDisableMetaTelemetryComplete
{
    static ForceDisableMetaTelemetryComplete()
    {
        // Disable Meta Telemetry sending
        const string telemetryKey = "Meta.XR.Telemetry.Enabled";
        if (EditorPrefs.HasKey(telemetryKey))
        {
            if (EditorPrefs.GetBool(telemetryKey))
            {
                Debug.Log("[ForceDisableMetaTelemetry] Telemetry was enabled. Disabling now...");
                EditorPrefs.SetBool(telemetryKey, false);
            }
            else
            {
                Debug.Log("[ForceDisableMetaTelemetry] Telemetry already disabled.");
            }
        }
        else
        {
            Debug.Log("[ForceDisableMetaTelemetry] No telemetry setting found. Setting to disabled...");
            EditorPrefs.SetBool(telemetryKey, false);
        }

        // Fake consent: Pretend user already said no, so pop-up won't appear
        const string telemetryConsentKey = "Meta.XR.Telemetry.ConsentAnswer";
        if (!EditorPrefs.HasKey(telemetryConsentKey))
        {
            Debug.Log("[ForceDisableMetaTelemetry] Setting fake consent to block pop-up...");
            EditorPrefs.SetInt(telemetryConsentKey, 0);  // 0 = Don't send
        }
    }
}
