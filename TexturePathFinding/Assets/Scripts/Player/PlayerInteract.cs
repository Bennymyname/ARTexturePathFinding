using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerInteract : MonoBehaviour
{
    private Camera cam;
    [SerializeField]
    public GameObject crossHair;
    // [SerializeField]
    // private float distance = 3f;
    [SerializeField]
    private LayerMask mask;
    private PlayerUI playerUI;
    private InputManager inputManager;
    
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
