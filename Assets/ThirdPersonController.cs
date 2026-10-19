using UnityEngine;

public class ThirdPersonController : MonoBehaviour
{
    public CharacterController controller;
    public float speed = 6.0f;
    public float turnSmoothTime = 0.1f;
    public float slowTurnSpeed = 30f; // Langsame Drehgeschwindigkeit mit Shift
    float turnSmoothVelocity;

    public Animator animator;
    private static readonly int IsWalking = Animator.StringToHash("isWalking");

    private void Update()
    {
        float horizontal = 0f;
        if (Input.GetKey(KeyCode.RightArrow) && !Input.GetKey(KeyCode.RightShift)) horizontal = 1f;
        if (Input.GetKey(KeyCode.LeftArrow) && !Input.GetKey(KeyCode.LeftShift)) horizontal = -1f;

        float vertical = 0f;
        if (Input.GetKey(KeyCode.UpArrow)) vertical = 1f;
        if (Input.GetKey(KeyCode.DownArrow)) vertical = -1f;

        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        float angle = transform.eulerAngles.y;

        if (direction.magnitude >= 0.1f)
        {
            angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
        }

        // Langsame Drehung mit Shift-Tasten, nur wenn auch eine Richtungstaste gedrückt ist
        if (direction.magnitude >= 0.1f)
        {
            if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.LeftArrow))
            {
                angle -= slowTurnSpeed * Time.deltaTime;
            }
            else if (Input.GetKey(KeyCode.RightShift) && Input.GetKey(KeyCode.RightArrow))
            {
                angle += slowTurnSpeed * Time.deltaTime;
            }
        }

        transform.rotation = Quaternion.Euler(0f, angle, 0f);

        // Bewegung
        if (direction.magnitude >= 0.1f)
        {
            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDir.normalized * speed * Time.deltaTime);
            animator.SetBool(IsWalking, true);
        }
        else
        {
            animator.SetBool(IsWalking, false);
        }
    }
}
