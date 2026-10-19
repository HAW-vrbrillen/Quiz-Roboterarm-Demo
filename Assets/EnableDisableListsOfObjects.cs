using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableDisableListsOfObjects : MonoBehaviour
{
    [Header("A disable B enable dann C disable D enable")]
    // Listen von GameObjects, die abhängig vom Aufrufzustand aktiviert oder deaktiviert werden sollen
    public List<GameObject> listAoff = new List<GameObject>();
    public List<GameObject> listBon = new List<GameObject>();
    public List<GameObject> listCoff = new List<GameObject>();
    public List<GameObject> listDon = new List<GameObject>();

    // Zähler für die Anzahl der Aufrufe der Methode EnableDisableObjects
    private int callCount = 0;

    // Methode zum Aktivieren und Deaktivieren der Objekte
    public void EnableDisableObjects()
    {
        // Erhöhe den Aufrufzähler
        callCount++;

        // Bestimme den aktuellen Modus basierend auf dem Wert von callCount
        int mode = callCount % 3; // Verwende Modulo 3, da das Muster alle drei Aufrufe wiederholt

        // Logik zum Aktivieren und Deaktivieren basierend auf dem aktuellen Modus
        switch (mode)
        {
            case 1: // Erster Modus: Liste A deaktivieren, Liste B aktivieren
                SetActiveState(listAoff, false);
                SetActiveState(listBon, true);
                break;
            case 2: // Zweiter Modus: Liste C deaktivieren, Liste D aktivieren
                SetActiveState(listCoff, false);
                SetActiveState(listDon, true);
                break;
            case 0: // Dritter Modus: Liste A aktivieren, Liste B deaktivieren (zurück zum ersten Zustand wie beim Start)
                SetActiveState(listAoff, true);
                SetActiveState(listBon, false);
                break;
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
