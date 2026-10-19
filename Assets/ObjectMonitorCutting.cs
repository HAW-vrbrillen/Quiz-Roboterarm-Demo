using TMPro;
using UnityEngine;

public class ObjectMonitor : MonoBehaviour
{
    public string substring = "Nut";  // Substring, nach dem gesucht wird
    public AudioClip audioClip80;        // Audio-Clip bei 80% deaktivierten Objekten
    public AudioClip audioClip100;       // Audio-Clip bei 100% deaktivierten Objekten
    public GameObject fireworkPrefab;    // Prefab des Feuerwerks

    private GameObject[] targetObjects;
    private AudioSource audioSource;
    private bool played80PercentClip = false;
    private bool played100PercentClip = false;

    public TMP_Text ResultCutText;

    void Start()
    {
        // Initialisiere AudioSource
        audioSource = GetComponent<AudioSource>();

        // Finde alle Kinderobjekte, die den Substring im Namen haben
        targetObjects = FindObjectsWithSubstring(transform, substring);
    }

    void Update()
    {
        int disabledCount = 0;

        // Zähle die deaktivierten Objekte
        foreach (GameObject obj in targetObjects)
        {
            if (obj != null && !obj.activeInHierarchy)
            {
                disabledCount++;
            }
        }

        float disabledPercentage = (float)disabledCount / targetObjects.Length * 100;
        ResultCutText.text ="Erfolg von 100% \n aktuell: " + disabledPercentage + "%";

        // Überprüfe, ob 80% erreicht sind
        if (disabledPercentage >= 80 && !played80PercentClip)
        {
            audioSource.PlayOneShot(audioClip80);
            played80PercentClip = true;
        }

        // Überprüfe, ob 100% erreicht sind
        if (disabledPercentage == 100 && !played100PercentClip)
        {
            audioSource.PlayOneShot(audioClip100);
            Instantiate(fireworkPrefab, transform.position, Quaternion.identity); // Firework instanziieren
            played100PercentClip = true;
        }
    }

    // Methode, um alle Kinderobjekte mit bestimmtem Substring zu finden
    GameObject[] FindObjectsWithSubstring(Transform parent, string substring)
    {
        Transform[] allChildren = parent.GetComponentsInChildren<Transform>(true);
        System.Collections.Generic.List<GameObject> matchingObjects = new System.Collections.Generic.List<GameObject>();

        foreach (Transform child in allChildren)
        {
            if (child.name.Contains(substring))
            {
                matchingObjects.Add(child.gameObject);
            }
        }

        return matchingObjects.ToArray();
    }
}
