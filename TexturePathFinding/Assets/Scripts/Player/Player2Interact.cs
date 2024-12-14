using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Player2Interact : MonoBehaviour
{
    [Header("Camera-Based Ray")]
    private Camera cam;
    [SerializeField] private float distance = 10f; // Increased
    [SerializeField] private LayerMask mask;

    [Header("VR Settings")]
    public OVRControllerHelper leftControllerHelper;
    public bool isVRActive = false;

    private PlayerUI playerUI;
    private InputManager inputManager;

    public GameObject button1;
    public GameObject button2;
    public GameObject button3;
    public GameObject button4;
    public GameObject button5;
    public GameObject button6;
    public GameObject button7;
    public GameObject button8;
    public GameObject button9;
    public GameObject button10;

    void Start()
    {
        cam = GetComponent<PlayerLook>().cam;
        playerUI = GetComponent<PlayerUI>();
        inputManager = GetComponent<InputManager>();
    }

    void Update()
    {
        playerUI.UpdateText(string.Empty);

        OVRRayHelper RayHelper = (leftControllerHelper != null) ? leftControllerHelper.RayHelper : null;

        Ray ray;
        if (isVRActive && RayHelper != null)
        {
            ray = new Ray(RayHelper.transform.position, RayHelper.transform.forward);
        }
        else
        {
            ray = new Ray(cam.transform.position, cam.transform.forward);
        }

        RaycastHit hitInfo;
        bool hitSomething = Physics.Raycast(ray, out hitInfo, distance, mask);

        // Basic rayData initialization
        OVRInputRayData rayData = new OVRInputRayData
        {
            IsActive = (inputManager.VRClickTriggered || inputManager.DesktopInteractTriggered),
            ActivationStrength = (inputManager.VRClickTriggered || inputManager.DesktopInteractTriggered) ? 1f : 0f,
            IsOverCanvas = false,
            DistanceToCanvas = (RayHelper != null && RayHelper.DefaultLength > 0f) ? RayHelper.DefaultLength : 2f,
            WorldPosition = (RayHelper != null) ?
                (RayHelper.transform.position + RayHelper.transform.forward * ((RayHelper.DefaultLength > 0f) ? RayHelper.DefaultLength : 2f))
                : (transform.position + transform.forward * 2f),
            WorldNormal = (RayHelper != null) ? -RayHelper.transform.forward : -transform.forward
        };

        if (hitSomething)
        {
            Debug.Log("Hit detected at distance: " + hitInfo.distance + " on object: " + hitInfo.collider.name);
            Interactable interactable = hitInfo.collider.GetComponent<Interactable>();
            
            if (interactable != null)
            {
                playerUI.UpdateText(interactable.promptMessage);
            }

            // Remove offset by setting normal to zero before positioning
            rayData.WorldNormal = Vector3.zero;
            rayData.IsOverCanvas = true;
            rayData.DistanceToCanvas = hitInfo.distance;
            rayData.WorldPosition = hitInfo.point;
            Debug.Log("Cursor position set to hit point: " + hitInfo.point);

            // Interaction
            if (inputManager.VRClickTriggered || inputManager.DesktopInteractTriggered)
            {
                if (hitInfo.collider.gameObject == button1) SceneManager.LoadScene(PlayerStats.TT);
                else if (hitInfo.collider.gameObject == button2) SceneManager.LoadScene(PlayerStats.AH);
                else if (hitInfo.collider.gameObject == button3) SceneManager.LoadScene(PlayerStats.AM);
                else if (hitInfo.collider.gameObject == button4) SceneManager.LoadScene(PlayerStats.AL);
                else if (hitInfo.collider.gameObject == button5) SceneManager.LoadScene(PlayerStats.BH);
                else if (hitInfo.collider.gameObject == button6) SceneManager.LoadScene(PlayerStats.BM);
                else if (hitInfo.collider.gameObject == button7) SceneManager.LoadScene(PlayerStats.BL);
                else if (hitInfo.collider.gameObject == button8) SceneManager.LoadScene(PlayerStats.CH);
                else if (hitInfo.collider.gameObject == button9) SceneManager.LoadScene(PlayerStats.CM);
                else if (hitInfo.collider.gameObject == button10) SceneManager.LoadScene(PlayerStats.CL);

                if (interactable != null)
                {
                    interactable.BaseInteract();
                }
            }
        }
        else
        {
            Debug.Log("No hit detected, cursor remains at default position.");
        }

        if (RayHelper != null)
        {
            RayHelper.UpdatePointerRay(rayData);
        }

        if (inputManager.onFoot.StageSelector.triggered)
        {
            SceneManager.LoadScene("StageSelector");
        }

        if (inputManager.onFoot.Escape.triggered)
        {
            SceneManager.LoadScene("MainMenu");
            Cursor.lockState = CursorLockMode.None;
        }
    }
}
