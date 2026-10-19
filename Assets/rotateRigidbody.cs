using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotateRigidbody : MonoBehaviour
{
    public bool flagRotRB1;
    public float torquePower = 1f;
    Rigidbody rb1;// Start is called before the first frame update
    void Start()
    {
        rb1 = GetComponent<Rigidbody>();
        flagRotRB1 = false;

        
    }

    // Update is called once per frame
    void Update()
    {
        if (flagRotRB1)
        {
            rb1.AddTorque(torquePower * Time.deltaTime,0 , 0);
        }
    }
}
