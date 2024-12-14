using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    // Desktop input system
    private PlayerInput playerInput;
    public PlayerInput.OnFootActions onFoot;

    // VR input system
    private MetaControls metaControls;

    private PlayerMotor motor;         
    private PlayerLook look;           

    private void Awake()
    {
        // Initialize desktop input
        playerInput = new PlayerInput();
        onFoot = playerInput.OnFoot;

        // Initialize VR input
        metaControls = new MetaControls();

        motor = GetComponent<PlayerMotor>();
        look = GetComponent<PlayerLook>();

        Debug.Log("InputManager initialized successfully.");
    }

    private void FixedUpdate()
    {
        // Movement Input
        Vector2 moveInput = Vector2.zero;

        // Try VR move input first
        if (metaControls.PlayerControls.Move != null)
        {
            moveInput = metaControls.PlayerControls.Move.ReadValue<Vector2>();
        }

        // Fallback to desktop if VR input is zero
        if (moveInput == Vector2.zero)
        {
            moveInput = onFoot.Movement.ReadValue<Vector2>();
        }

        motor.ProcessMove(moveInput);
    }

    private void LateUpdate()
    {
        // Look Input
        Vector2 lookInput = Vector2.zero;

        // Try VR look input first (if available)
        if (metaControls.PlayerControls.Look != null)
        {
            lookInput = metaControls.PlayerControls.Look.ReadValue<Vector2>();
        }

        // Fallback to desktop look if VR look is zero
        if (lookInput == Vector2.zero)
        {
            lookInput = onFoot.Look.ReadValue<Vector2>();
        }

        look.ProcessLook(lookInput);

        // Headset Orientation
        if (metaControls.PlayerControls.TrackedDeviceOrientation != null)
        {
            Quaternion headsetRotation = metaControls.PlayerControls.TrackedDeviceOrientation.ReadValue<Quaternion>();

            // If headset is rotated differently from the identity, assume VR active
            if (Quaternion.Angle(Quaternion.identity, headsetRotation) > 0.1f)
            {
                look.isVRActive = true;
                look.ApplyHeadsetOrientation(headsetRotation);
                Debug.Log("Headset Orientation: " + headsetRotation.eulerAngles);
            }
        }
    }

    private void OnEnable()
    {
        // Enable desktop input
        onFoot.Enable();

        // Enable VR input
        metaControls.PlayerControls.Enable();
    }

    private void OnDisable()
    {
        // Disable desktop input
        onFoot.Disable();

        // Disable VR input
        metaControls.PlayerControls.Disable();
    }

    // Add a helper property to check VR Click and Desktop Interact
    public bool VRClickTriggered
    {
        get
        {
            // Check if the VR click action exists and is triggered
            return metaControls.PlayerControls.Click != null && metaControls.PlayerControls.Click.triggered;
        }
    }

    public bool DesktopInteractTriggered
    {
        get
        {
            return onFoot.Interact.triggered;
        }
    }
}
