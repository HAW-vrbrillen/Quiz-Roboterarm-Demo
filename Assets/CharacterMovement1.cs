using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    public CharacterController characterController;
    public float speed = 0.1f;
    public GameObject[] pathObjects;
    private int currentTargetIndex = 0;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (pathObjects.Length == 0 || currentTargetIndex >= pathObjects.Length) return;

        // Bewegung in Richtung des Pfads
        Vector3 moveDirection = (pathObjects[currentTargetIndex].transform.position - transform.position).normalized;
        characterController.Move(moveDirection * speed * Time.deltaTime);

        // Prüfen Sie, ob der Charakter das Ziel erreicht hat und aktualisieren Sie gegebenenfalls den Index
        if (Vector3.Distance(transform.position, pathObjects[currentTargetIndex].transform.position) < 0.05f)
        {
            currentTargetIndex = (currentTargetIndex + 1) % pathObjects.Length;
        }
    }
}
