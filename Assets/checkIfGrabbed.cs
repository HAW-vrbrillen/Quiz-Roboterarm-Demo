using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class checkIfGrabbed : MonoBehaviour
{
    public bool grabbed;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        grabbed = true;

    }
    private void OnTriggerExit(Collider other)
    {
        grabbed = false;
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
