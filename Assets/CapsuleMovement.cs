using UnityEngine;

public class CapsuleMovement : MonoBehaviour
{
    public float speed = 5.0f;
    public float gravity = -9.81f;
    private CharacterController controller;
    private Vector3 velocity;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 direction = new Vector3(horizontal, 0, vertical);

        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Ein kleiner negativer Wert, um sicherzustellen, dass die Kapsel am Boden bleibt.
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(direction * speed * Time.deltaTime);
        controller.Move(velocity * Time.deltaTime); // Anwendung der Schwerkraft.
    }
}
