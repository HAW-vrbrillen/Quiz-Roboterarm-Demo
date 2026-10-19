using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hilfsScript : MonoBehaviour
{
    public bool flagDoIt = false;
    // Start is called before the first frame update
    void Start()
    {
        if (flagDoIt)
        {
            foreach (var device in Microphone.devices)
            {
                Debug.Log("Name: " + device);
            }

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
