using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class turnwithspeed : MonoBehaviour
{
    public gearcontrol01 gearcontrol1;
    public float speed;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (gearcontrol1.gearToggle)
        {
            transform.Rotate(0, 0, speed * Time.deltaTime);

        }    }
}
