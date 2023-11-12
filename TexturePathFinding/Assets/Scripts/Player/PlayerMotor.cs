using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMotor : MonoBehaviour
{  
    public Camera cam;
    [SerializeField]
    public GameObject blink;
    
    
    

    private CharacterController controller;
    private Vector3 playerVelocity;
    private bool isGrounded;
    public float speed = 5f;
    public float gravity = -9.8f;
    public float jumpHeight = 3f;
    public bool lerpCrouch, crouching, sprinting, respawn, zooming = false;
    public float crouchTimer = 1;
    public float defaultFov = 90;
    public Vector3 respawnPosition = new Vector3(0f, 1f, 0f);
    //mouse
    public Vector2 mouseDelta;
    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        //locks mouse on center
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        

    
    }

    // Update is called once per frame
    void Update()
    {
        //click on screen to make the cursor invisible
        mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        isGrounded = controller.isGrounded;
        
    }
    
    //receive the inputs for our InputManager.cs and apply them to our character controller
    public void ProcessMove(Vector2 input)
    {
    Vector3 moveDirection = Vector3.zero;
    moveDirection.x = input.x;
    moveDirection.z = input.y;
    controller.Move(transform.TransformDirection(moveDirection) * speed * Time.deltaTime);
    playerVelocity.y += gravity * Time.deltaTime;

    if (isGrounded && playerVelocity.y < 0)
        playerVelocity.y = -2f;

    controller.Move(playerVelocity * Time.deltaTime);
    // Debug.Log(playerVelocity.y);
    }
}
