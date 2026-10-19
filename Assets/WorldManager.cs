using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    [Header("Spezifische Listen für Challenges")]
    public List<GameObject> sceneSwitcher = new List<GameObject>();
    

    [Header("Allgemeine Liste für Deaktivierung")]
    public List<GameObject> allObjectsToDisable = new List<GameObject>();

    
    public TMP_Text debuginfo1;

    // Methode zum Deaktivieren aller Objekte
    private void Start()
    {
        sceneSwitcherFct();
    }

    public void DisableAll()
    {
        foreach (GameObject obj in allObjectsToDisable)
        {
            obj.SetActive(false);
        }
    }

    // Methoden für spezifische Challenges

    public void sceneSwitcherFct()
    {
        DisableAll();
        SetActiveFunction(sceneSwitcher);
    }


    // Allgemeine SetActiveFunction-Funktion, die eine Liste von Objekten aktiviert und dann die Intro-Funktion aufruft
    private void SetActiveFunction(List<GameObject> objects)
    {
        bool isActive = objects.Count > 0 && objects[0].activeSelf;
        if (!isActive)
        {
            SetActiveState(objects, true);
        }
        else
        {
            debuginfo1.text = "Achtung list empty.";
        }
    }

    // Hilfsmethode zum Setzen des Aktivierungszustands einer Liste von GameObjects
    private void SetActiveState(List<GameObject> objects, bool state)
    {
        foreach (GameObject obj in objects)
        {
            obj.SetActive(state);
        }
    }
}
