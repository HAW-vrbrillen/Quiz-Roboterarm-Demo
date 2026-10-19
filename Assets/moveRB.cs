using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moveRB : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void rbMove1()
    {
        GetComponent<Rigidbody>().AddForce(10f, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
