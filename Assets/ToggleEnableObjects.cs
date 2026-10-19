using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleEnableObjects : MonoBehaviour
{
    // Liste von GameObjects, die getoggelt werden sollen
    public List<GameObject> objects = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        // Alle Objekte in der Liste initial deaktivieren
        foreach (GameObject obj in objects)
        {
            obj.SetActive(false);
        }
    }

    // Methode zum Toggeln der aktiven Zustände der Objekte in der Liste
    public void ToggleOnOffObjects()
    {
        foreach (GameObject obj in objects)
        {
            // Toggle the active state of each object
            obj.SetActive(!obj.activeSelf);
        }
    }
}
