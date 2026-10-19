using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class keepswimming : MonoBehaviour
{
    
    // water y = 0.023 und swimobject y =  1.1269 immer plus 1,107 vom wasser
    public GameObject water;
    public GameObject swimobject;
    public float maxdistance = 0.01f;
    float distance;

    // Update is called once per frame
    void Update()
    {
        //distance = Vector3.Distance(water.transform.position, swimobject.transform.position);
        swimobject.transform.position = new Vector3(swimobject.transform.position.x, water.transform.position.y+1.107f, swimobject.transform.position.z);
        

        
    }
}
