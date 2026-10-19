using UnityEngine;

public class driveCar01 : MonoBehaviour
{
    public float speed = 1500f; // Geschwindigkeit des Autos
    public float rotationSpeed = 100f; // Drehgeschwindigkeit des Autos

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        float moveVertical = Input.GetAxis("Vertical"); // W und S Tasten
        float moveHorizontal = Input.GetAxis("Horizontal"); // A und D Tasten

        // Bewegt das Fahrzeug vorwärts oder rückwärts
        rb.AddForce(transform.forward * moveVertical * speed * Time.deltaTime);

        // Dreht das Fahrzeug links oder rechts
        if (moveVertical != 0) // Dreht nur, wenn das Fahrzeug sich bewegt
        {
            transform.Rotate(0, moveHorizontal * rotationSpeed * Time.deltaTime, 0);
        }
    }
}
