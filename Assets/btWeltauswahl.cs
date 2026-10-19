using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class btWeltauswahl : MonoBehaviour
{
    [Header("Liste der beim Start zu deaktivierenden Objekte")]
    public List<GameObject> startDisableObjects = new List<GameObject>();

    [Header("Liste der zu aktivierenden Objekte")]
    public List<GameObject> enableObjects = new List<GameObject>();

    [Header("Liste der zu deaktivierenden Objekte")]
    public List<GameObject> disableObjects = new List<GameObject>();

    /*
    private TTSManager tts;

    private void Start()
    {
        tts = FindObjectOfType<TTSManager>();
        
    }
    */

    private void Start()
    {
        // Aktivieren der Objekte in enableObjects
        foreach (GameObject obj in startDisableObjects)
        {
            obj.SetActive(false);
        }
    }

    public void Weltauswahl()
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
        /*
        if (tts != null)
        {
            tts.Speak("Hallo, dies ist ein plattformübergreifender TTS-Test.");
        }
        */
    }
}
