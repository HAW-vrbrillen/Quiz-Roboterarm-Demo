using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using TMPro;

public class AgentController : MonoBehaviour
{
    public Transform goal; // Ziel, dem der Agent folgen soll
    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.destination = goal.position; // Setzt das Ziel des Agenten
    }

    void Update()
    {
        // Hier könntest du Logik hinzufügen, um das Ziel dynamisch zu ändern
    }
}
