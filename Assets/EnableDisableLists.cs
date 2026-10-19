using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableDisableLists : MonoBehaviour
{
    [Header("Liste der zu aktivierende Objekte beim Start")]
    public List<GameObject> StartEnableObjects = new List<GameObject>();

    [Header("Liste der zu deaktivierende Objekte beim Start")]
    public List<GameObject> StartdisableObjects = new List<GameObject>();


    [Header("Liste der zu aktivierenden Objekte")]
    public List<GameObject> enableObjects = new List<GameObject>();

    [Header("Liste der zu deaktivierenden Objekte")]
    public List<GameObject> disableObjects = new List<GameObject>();

    

    private void Start()
    {
        // Aktivieren der Objekte in enableObjects
        foreach (GameObject obj in StartEnableObjects)
        {
            obj.SetActive(true);
        }

        // De-Aktivieren der Objekte in enableObjects
        foreach (GameObject obj in StartdisableObjects)
        {
            obj.SetActive(false);
        }
        
    }

    public void doEnableDisableLists()
    {
        // Aktivieren der Objekte in enableObjects
        foreach (GameObject obj in enableObjects)
        {
            obj.SetActive(true);
        }

        // Deaktivieren der Objekte in disableObjects
        foreach (GameObject obj in disableObjects)
        {
            obj.SetActive(false);
        }

        


    }
}
