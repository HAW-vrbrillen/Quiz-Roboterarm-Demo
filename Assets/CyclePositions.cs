using UnityEngine;
using System.Collections.Generic;

public class CyclePositions : MonoBehaviour
{
    public GameObject positionsParent;
    private List<Transform> positions = new List<Transform>();
    private int currentPositionIndex = 0;
    public float movementSpeed = 1f;
    public float gravity = -9.81f;
    private CharacterController controller;
    private Vector3 velocity;
    private Renderer characterRenderer; // Renderer zum Ändern der Farbe

    // Einstellungen für die Neigungsüberprüfung
    public float maxSlopeAngle = 60f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        characterRenderer = GetComponent<Renderer>(); // Stellen Sie sicher, dass der Charakter ein Renderer-Komponent hat

        foreach (Transform child in positionsParent.transform)
        {
            positions.Add(child);
        }
    }

    void Update()
    {
        if (positions.Count == 0)
            return;

        Transform targetPosition = positions[currentPositionIndex];
        Vector3 targetPositionXZ = new Vector3(targetPosition.position.x, transform.position.y, targetPosition.position.z);
        Vector3 direction = (targetPositionXZ - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, targetPositionXZ);

        if (distance > 0.1f)
        {
            controller.Move(direction * movementSpeed * Time.deltaTime);
        }
        else
        {
            currentPositionIndex = (currentPositionIndex + 1) % positions.Count;
        }

        // Schwerkraft
        if (!controller.isGrounded)
        {
            velocity.y += gravity * Time.deltaTime;
        }
        else
        {
            velocity.y = -2f;
            CheckSlope(); // Überprüfen Sie die Steigung
        }

        controller.Move(velocity * Time.deltaTime);
    }

    void CheckSlope()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, controller.height / 2 * 1.1f))
        {
            float slopeAngle = Vector3.Angle(hit.normal, Vector3.up);
            if (slopeAngle > maxSlopeAngle)
            {
                characterRenderer.material.color = Color.red; // Charakter rot färben
            }
            else
            {
                characterRenderer.material.color = Color.white; // Zurück zu normaler Farbe
            }
        }
    }
}
