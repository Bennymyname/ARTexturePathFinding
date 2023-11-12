using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using static PlayerStats;

public class Player2Interact : MonoBehaviour
{
    private Camera cam;
    [SerializeField]
    public GameObject crossHair;
    [SerializeField]
    private float distance = 3f;
    [SerializeField]
    private LayerMask mask;
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
    
    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<PlayerLook>().cam;
        playerUI = GetComponent<PlayerUI>();
        inputManager = GetComponent<InputManager>();
    }

    // Update is called once per frame
    void Update()
    {
        playerUI.UpdateText(string.Empty);
        crossHair.SetActive(false);
        //create a ray at the center of the camera, shooting outwards.
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        Debug.DrawRay(ray.origin, ray.direction * distance);
        RaycastHit hitInfo; //variable to store our collision information;
        if(Physics.Raycast(ray, out hitInfo, distance, mask))
        {
            if(hitInfo.collider.GetComponent<Interactable>() != null)
            {
                Interactable interactable = hitInfo.collider.GetComponent<Interactable>();
                playerUI.UpdateText(interactable.promptMessage);
                crossHair.SetActive(true);
                if (inputManager.onFoot.Interact.triggered)
                { //Refer PlayerStats variables for Stages { TT, AH, AM, AL, BH, BM, BL, CH, CM, CL, SS}
                    if(hitInfo.collider.gameObject == button1)
                    {
                        SceneManager.LoadScene(TT);
                    }
                    if(hitInfo.collider.gameObject == button2)
                    {
                        SceneManager.LoadScene(AH);
                    }
                    if(hitInfo.collider.gameObject == button3)
                    {
                        SceneManager.LoadScene(AM);
                    }
                    if(hitInfo.collider.gameObject == button4)
                    {
                        SceneManager.LoadScene(AL);
                    }
                    if(hitInfo.collider.gameObject == button5)
                    {
                        SceneManager.LoadScene(BH);
                    }
                    if(hitInfo.collider.gameObject == button6)
                    {
                        SceneManager.LoadScene(BM);
                    }
                    if(hitInfo.collider.gameObject == button7)
                    {
                        SceneManager.LoadScene(BL);
                    }
                    if(hitInfo.collider.gameObject == button8)
                    {
                        SceneManager.LoadScene(CH);
                    }
                    if(hitInfo.collider.gameObject == button9)
                    {
                        SceneManager.LoadScene(CM);
                    }
                    if(hitInfo.collider.gameObject == button10)
                    {
                        SceneManager.LoadScene(CL);
                    }
                    
                    interactable.BaseInteract();
                }
            }
        }

        // if (inputManager.onFoot.LoadScene.triggered)
        // {
        //     PlayerStats.Instance.LoadScene();
            
        // }
        if (inputManager.onFoot.StageSelector.triggered)
        {
            SceneManager.LoadScene("StageSelector");
            
        }
        if (inputManager.onFoot.Escape.triggered)
        {
            SceneManager.LoadScene("MainMenu");
            UnityEngine.Cursor.lockState = CursorLockMode.None;
            
        }

    }

}
