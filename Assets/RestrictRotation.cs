using UnityEngine;

public class RestrictRotation : MonoBehaviour
{
    private void Update()
    {
        // Beschränke die Rotation nur auf die Y-Achse
        Vector3 currentRotation = transform.eulerAngles;
        transform.eulerAngles = new Vector3(0, currentRotation.y, 0);
    }
}
