using UnityEngine;

public class PlayerMotor : MonoBehaviour
{
    public Camera cam; 
    private CharacterController controller;
    private Vector3 playerVelocity;
    private bool isGrounded;

    public float speed = 5f; 
    public float gravity = -9.8f; 

    private void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    public void ProcessMove(Vector2 input)
    {
        isGrounded = controller.isGrounded;

        Vector3 moveDirection = Vector3.zero;
        moveDirection.x = input.x;
        moveDirection.z = input.y;

        if (cam != null)
        {
            Vector3 forward = cam.transform.forward;
            Vector3 right = cam.transform.right;

            forward.y = 0f;
            right.y = 0f;

            forward.Normalize();
            right.Normalize();

            Vector3 desiredMove = forward * moveDirection.z + right * moveDirection.x;
            controller.Move(desiredMove * speed * Time.deltaTime);
        }

        playerVelocity.y += gravity * Time.deltaTime;
        if (isGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = -2f;
        }
        controller.Move(playerVelocity * Time.deltaTime);
    }
}
