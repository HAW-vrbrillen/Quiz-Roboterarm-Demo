using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AgentController2 : MonoBehaviour
{
    private NavMeshAgent agent;
    // public Transform goal;
    public GameObject goalGO;
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        //agent.destination = goal.position;
        agent.destination = goalGO.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
