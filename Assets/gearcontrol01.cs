using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gearcontrol01 : MonoBehaviour
{
    public bool gearToggle;
    
    // Start is called before the first frame update
    void Start()
    {
        gearToggle = false;
    }
    public void toggleGears()
    {
        gearToggle = !gearToggle;
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
