using UnityEngine;
using UnityEngine.AI;

public class MultiTargetPathfinding : MonoBehaviour
{
    public GameObject[] targets; // Definieren Sie Ihre Ziele hier im Unity-Editor
    private int currentTargetIndex = 0;
    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.destination = targets[currentTargetIndex].transform.position;
    }

    void Update()
    {
        // Wenn der Agent sein Ziel erreicht hat, wechseln Sie zum nächsten Ziel
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            currentTargetIndex = (currentTargetIndex + 1) % targets.Length;
            agent.destination = targets[currentTargetIndex].transform.position;
        }
    }
}
