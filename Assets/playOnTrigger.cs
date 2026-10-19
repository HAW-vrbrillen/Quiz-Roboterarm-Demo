using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playOnTrigger : MonoBehaviour
{
    public GameObject water1;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        water1.GetComponent<AudioSource>().Play();
    }
    private void OnTriggerExit(Collider other)
    {
        water1.GetComponent<AudioSource>().Stop();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
