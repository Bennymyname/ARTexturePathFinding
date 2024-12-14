using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    public Camera cam;
    private float xRotation = 0f;
    public static float sensitivity = 25f;
    public bool isVRActive = false;

    // Desktop look logic
    public void ProcessLook(Vector2 input)
    {
        // If VR is active, skip manual camera rotation
        if (isVRActive) 
        {
            return;
        }

        float lookY = input.y * sensitivity * Time.deltaTime;
        xRotation -= lookY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);

        if (cam != null)
        {
            cam.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
        }

        float lookX = input.x * sensitivity * Time.deltaTime;
        transform.Rotate(Vector3.up * lookX);
    }

    // VR headset orientation logic
    public void ApplyHeadsetOrientation(Quaternion headsetRotation)
    {
        if (cam == null) return;

        Vector3 headsetEulerAngles = headsetRotation.eulerAngles;
        // Rotate only around Y-axis to align player facing with the headset
        transform.rotation = Quaternion.Euler(0, headsetEulerAngles.y, 0);

        // Do NOT set cam.transform.localRotation here for VR mode.
        // The OVR system automatically handles pitch and roll for the camera.
    }
}
