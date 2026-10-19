/*
 * (c) 2024 Randolf Isenberg. Alle Rechte vorbehalten.
 *
 * Dieser Code ist das exklusive Eigentum von Randolf Isenberg und urheberrechtlich geschützt unter
 * deutschen und internationalen Urheberrechtsgesetzen. Dieser Code darf für nicht kommerzielle
 * Zwecke unter der Bedingung, dass der ursprüngliche Urheber und die Urheberschaft klar und deutlich
 * angegeben werden, weitergegeben und modifiziert werden. Die Verwendung dieses Codes, ganz oder teilweise,
 * für kommerzielle Zwecke ohne die vorherige schriftliche Genehmigung von Randolf Isenberg ist verboten.
 *
 * Durch die Verwendung dieses Codes erkennen Sie an, dass Sie die oben genannten Bedingungen
 * gelesen, verstanden und akzeptiert haben. Zuwiderhandlungen können zivil- und/oder strafrechtlich
 * verfolgt werden.
 */

// Dieses Skript als Komponente an ein beliebiges Objekt hängen.
// Schieben Sie ein beliebiges Objekt in den Slot für die Bewegung in der Fläche und 
// das gleiche oder ein anderes Objekt in den Slot für die Bewegung. 
// so lässt sich hieraus z.B. einfach eine sich bewegender Roboter mit einem bewegten Arm erstellen.

using UnityEngine;

public class MoveAndRotateWithKeys : MonoBehaviour
{
    public float moveSpeed = 5.0f; // Geschwindigkeit der Bewegung
    public float rotationSpeed = 100.0f; // Geschwindigkeit der Rotation

    // KeyCodes für Bewegung
    public KeyCode moveForwardKey = KeyCode.W;
    public KeyCode moveBackwardKey = KeyCode.S;
    public KeyCode moveLeftKey = KeyCode.A;
    public KeyCode moveRightKey = KeyCode.D;

    // KeyCodes für Rotation
    public KeyCode rotateLeftKey = KeyCode.Q;
    public KeyCode rotateRightKey = KeyCode.E;

    public GameObject objectToRotate;
    public GameObject objectToMove;

    void Update()
    {
        // Bewegung
        if (Input.GetKey(moveForwardKey))
        {
            objectToMove.transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
        }
        if (Input.GetKey(moveBackwardKey))
        {
            objectToMove.transform.Translate(Vector3.back * moveSpeed * Time.deltaTime);
        }
        if (Input.GetKey(moveLeftKey))
        {
            objectToMove.transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);
        }
        if (Input.GetKey(moveRightKey))
        {
            objectToMove.transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);
        }

        // Rotation
        if (Input.GetKey(rotateLeftKey))
        {
            // Drehung nach links um die Y-Achse
            objectToRotate.transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
        }
        if (Input.GetKey(rotateRightKey))
        {
            // Drehung nach rechts um die Y-Achse
            objectToRotate.transform.Rotate(0, 0, -rotationSpeed * Time.deltaTime);
        }
    }
}
