using System.Collections;
using System.Collections.Generic;
using TMPro;
//using Unity.VisualScripting;
using UnityEngine;

public class triggerSensor : MonoBehaviour
{

    // Verweis auf Skript, muss reingeschoben werden !! oder mit Find gesucht werden
    ElementController ec1;

    private void Start()
    {
        ec1 = GetComponentInParent<ElementController>();    
    }


    private void OnTriggerEnter(Collider other)
    {
        
        if (ec1 != null)
        {
            if (other.gameObject.tag == "inputJoint")
            {
                ec1.Predecessor = other.gameObject;
                ec1.infoPredecessor.text = "Predecessor: " + other.gameObject.transform.parent.gameObject.name;
                GetComponent<Renderer>().material.color = Color.green;

            }
            if (other.gameObject.tag == "outputJoint")
            {
                ec1.Follower = other.gameObject;
                ec1.infoFollower.text = "Follower: " + other.gameObject.transform.parent.gameObject.name;
                GetComponent<Renderer>().material.color = Color.green;
            }
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (ec1 != null)
        {
            if (other.gameObject.tag == "inputJoint")
            {
                ec1.Predecessor = other.gameObject;
                ec1.infoPredecessor.text = "Predecessor: " + other.gameObject.transform.parent.gameObject.name;
                GetComponent<Renderer>().material.color = Color.green;
            }
            if (other.gameObject.tag == "outputJoint")
            {
                ec1.Follower = other.gameObject;
                ec1.infoFollower.text = "Follower: " + other.gameObject.transform.parent.gameObject.name;
                GetComponent<Renderer>().material.color = Color.green;
            }
        }

    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "inputJoint")
        {
            ec1.Predecessor = ec1.MainStorage;
            ec1.infoPredecessor.text = "Predecessor: " + ec1.MainStorage.name;
            GetComponent<Renderer>().material.color = Color.cyan;
        }
        if (other.gameObject.tag == "outputJoint")
        {
            ec1.Follower = ec1.MainStorage;
            ec1.infoFollower.text = "Follower: " + ec1.MainStorage.name;
            GetComponent<Renderer>().material.color = Color.cyan;
        }

    }

}
