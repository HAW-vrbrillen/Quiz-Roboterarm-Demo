using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class helpertotest : MonoBehaviour
{
    public waterControl  watercontrol1;
    public int actChoice = 0;
           
    // Start is called before the first frame update
    void Start()
    {
        if(actChoice == 0)
        {
            watercontrol1.StartFactory();

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
