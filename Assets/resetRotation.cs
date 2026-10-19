using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class resetRotation : MonoBehaviour
{

    public float resetSpeed = 1.0f;
    public float maxRotation = 5f;
    public bool resetRotation1 = false;

    void Update()
    {
        // Aktuelle Rotation holen
        Quaternion currentRotation = transform.rotation;
        Vector3 currentPosition = transform.position;



        if (resetRotation1)
        {
            if (currentRotation.eulerAngles.y > maxRotation)
            {
                transform.rotation = Quaternion.Euler(0, currentRotation.eulerAngles.y, 0);
                transform.position = currentPosition;
            } 
        }
        


        // Langsam zur Zielrotation interpolieren
        //transform.rotation = Quaternion.Lerp(currentRotation, targetRotation, resetSpeed * Time.deltaTime);
    }
}
