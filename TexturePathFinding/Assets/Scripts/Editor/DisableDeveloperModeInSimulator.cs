using UnityEngine;
using UnityEngine.XR;

#if UNITY_EDITOR
[UnityEditor.InitializeOnLoad]
#endif
public class DisableDeveloperModeInSimulator
{
    static DisableDeveloperModeInSimulator()
    {
#if UNITY_EDITOR
        if (IsMetaSimulatorRunning())
        {
            Debug.Log("[Meta XR] Meta Simulator detected. Skipping Developer Mode setup.");
            Application.SetStackTraceLogType(LogType.Error, StackTraceLogType.None);
        }
#endif
    }

    private static bool IsMetaSimulatorRunning()
    {
        // Check if simulator is active (simple check: deviceName contains "Meta XR Simulator")
        return XRSettings.isDeviceActive && XRSettings.loadedDeviceName.Contains("Simulator");
    }
}
