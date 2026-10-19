using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SourceToTargetMover : MonoBehaviour
{
     GameObject Source;
     GameObject Target;

    private void Start()
    {
        if (GameObject.Find("NoQuest") == null)
        {
            Source = GameObject.Find("OVRPlayerEnvironment");
        }
        else { Source = GameObject.Find("NoQuest"); }
    }
    public void moveSourcetoTarget(GameObject target)
    {
        Source.transform.position = target.transform.position;
        Source.transform.rotation = target.transform.rotation; 
    }
}
