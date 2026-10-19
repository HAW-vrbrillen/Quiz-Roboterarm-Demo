using UnityEngine;

public class CharacterController1 : MonoBehaviour
{
    public Transform targetObject;
    public float moveSpeed = 5f;
    public float raycastForwardOffset = 0.002f;
    public float raycastHeight = 1f;

    void Update()
    {
        // Raycast-Position vor dem Charakter
        Vector3 raycastOrigin = transform.position + transform.forward * raycastForwardOffset + Vector3.up * raycastHeight;

        RaycastHit hit;
        if (Physics.Raycast(raycastOrigin, Vector3.down, out hit))
        {
            // Anpassung der Höhe des Charakters
            transform.position = new Vector3(transform.position.x, hit.point.y, transform.position.z);
        }

        // Bewegung zum Zielobjekt
        Vector3 targetPosition = new Vector3(targetObject.position.x, transform.position.y, targetObject.position.z);
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
    }

    void OnDrawGizmos()
    {
        // Zeichnet den Raycast im Editor für Debugging-Zwecke
        Gizmos.color = Color.red;
        Vector3 raycastOrigin = transform.position + transform.forward * raycastForwardOffset + Vector3.up * raycastHeight;
        Gizmos.DrawLine(raycastOrigin, raycastOrigin + Vector3.down * 2);
    }
}
