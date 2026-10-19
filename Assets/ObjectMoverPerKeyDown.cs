using UnityEngine;

public class ObjectMoverPerKeyDown : MonoBehaviour
{
    [Header("AD linksRechts WS forwardBackw XC DownUP")]
    public float moveDistance = 0.0003f; // Bewegungsdistanz in Metern (0,3mm)

    void Update()
    {
        // Nach links bewegen
        if (Input.GetKeyDown(KeyCode.A))
        {
            transform.Translate(-moveDistance, 0, 0);
        }

        // Nach rechts bewegen
        if (Input.GetKeyDown(KeyCode.D))
        {
            transform.Translate(moveDistance, 0, 0);
        }

        // Vorwärts bewegen
        if (Input.GetKeyDown(KeyCode.W))
        {
            transform.Translate(0, 0, moveDistance);
        }

        // Rückwärts bewegen
        if (Input.GetKeyDown(KeyCode.S))
        {
            transform.Translate(0, 0, -moveDistance);
        }

        // Nach unten bewegen
        if (Input.GetKeyDown(KeyCode.X))
        {
            transform.Translate(0, -moveDistance, 0);
        }

        // Nach oben bewegen
        if (Input.GetKeyDown(KeyCode.C))
        {
            transform.Translate(0, moveDistance, 0);
        }
    }
}
