using UnityEngine;
using System;
using System.Collections.Generic;

public class FunctionNavigator : MonoBehaviour
{
    // Liste der GameObjects, die wir im Inspector setzen können
    public List<GameObject> objectsToControl;

    private List<Action> functions;
    private int currentIndex = -1;

    void Start()
    {
        // Initialisiere die Liste der Funktionen basierend auf den Objekten
        functions = new List<Action>();

        for (int i = 0; i < objectsToControl.Count; i++)
        {
            int index = i; // Lokale Kopie der Indexvariable, um Probleme in Lambda-Funktionen zu vermeiden

            functions.Add(() => ToggleObject(objectsToControl[index], true));
            functions.Add(() => ToggleObject(objectsToControl[index], false));
            // Weitere Funktionen für jedes Objekt hinzufügen, falls benötigt
        }
    }

    /*
    void Update()
    {
        // Überprüfen auf Tastenanschläge
        if (Input.GetKeyDown(KeyCode.F))
        {
            Forward();
        }
        else if (Input.GetKeyDown(KeyCode.B))
        {
            Backward();
        }
    }
    */

    void Forward()
    {
        if (currentIndex < functions.Count - 1)
        {
            currentIndex++;
            functions[currentIndex]?.Invoke();
        }
    }

    void Backward()
    {
        if (currentIndex > 0)
        {
            currentIndex--;
            functions[currentIndex]?.Invoke();
        }
    }

    void ToggleObject(GameObject obj, bool enable)
    {
        if (obj != null)
        {
            obj.SetActive(enable);
        }
        else
        {
            Debug.LogWarning("Object not found!");
        }
    }
}
