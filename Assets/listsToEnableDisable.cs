using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class listsToEnableDisable : MonoBehaviour
{
    [Header("Funktionen: DisableAll EnableAll")]
    public List<GameObject> allObjectsToEnable = new List<GameObject>();


    public List<GameObject> allObjectsToDisable = new List<GameObject>();


    //public TMP_Text debuginfo1;

    public void DisableAll()
    {
        foreach (GameObject obj in allObjectsToDisable)
        {
            obj.SetActive(false);
        }
    }

    // Methoden für spezifische Challenges

    public void EnableAll()
    {
        DisableAll();
        foreach (GameObject obj in allObjectsToEnable)
        {
            obj.SetActive(true);
        }
    }

}
