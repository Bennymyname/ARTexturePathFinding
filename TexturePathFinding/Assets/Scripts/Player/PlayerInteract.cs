using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerInteract : MonoBehaviour
{
    public Camera cam;
    [SerializeField]
    private GameObject crossHair;

    private PlayerUI playerUI;
    private InputManager inputManager;

    void Start()
    {
        playerUI = GetComponent<PlayerUI>();
        inputManager = GetComponent<InputManager>();
    }

    void Update()
    {
        if (inputManager.onFoot.Interact.triggered)
        {
            SceneManager.LoadScene("MainMenu");
            UnityEngine.Cursor.lockState = CursorLockMode.None;
        }
        if (inputManager.onFoot.StageSelector.triggered)
        {
            SceneManager.LoadScene("StageSelector");
        }
    }
}
