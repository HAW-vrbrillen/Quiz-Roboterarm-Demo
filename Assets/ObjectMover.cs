using UnityEngine;

public class ObjectMover : MonoBehaviour
{
    public float speed = 5f;
    private Vector3 movementDirection = Vector3.zero;

    // Methode, um die Bewegungsrichtung zu setzen. Richtung wird als Vector3 erwartet.
    public void SetMovementDirection(string directionString)
    {
        if (directionString == "up") { movementDirection = Vector3.up; }
        if (directionString == "down") { movementDirection = Vector3.down; }
        if (directionString == "left") { movementDirection = Vector3.left; }
        if (directionString == "right") { movementDirection = Vector3.right; }
        if (directionString == "forward") { movementDirection = Vector3.forward; }
        if (directionString == "backward") { movementDirection = Vector3.back; }

    }

    // Methode, um die Bewegung zu stoppen.
    public void StopMovement()
    {
        movementDirection = Vector3.zero;
    }

    void Update()
    {
        // Bewegt das Objekt in die gesetzte Richtung mit der gegebenen Geschwindigkeit.
        transform.Translate(movementDirection * speed * Time.deltaTime, Space.World);
    }
}
