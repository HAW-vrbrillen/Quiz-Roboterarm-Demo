using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class FraskopfInteraction : MonoBehaviour
{
    public AudioSource m_AudioSource;
    public AudioSource m_AudioSource2;

    public AudioClip aclip150prozent;
    public AudioClip aclip2100prozent;
    public AudioClip aclipCutting;
    public TMP_Text infoText;
    public GameObject ProduktParent;
    public string substring = "Nut"; // Setze hier den Substring ein, nach dem gesucht werden soll

    public int correctCut = 0;
    public int falseCut = 0;
    private int totalWithSubstring;
    public HashSet<GameObject> countedObjects = new HashSet<GameObject>(); // HashSet für gezählte Objekte

    bool prozent50Flag;
    bool prozent100Flag;

    private void Start()
    {
        prozent50Flag = true;
        prozent100Flag = true;

        m_AudioSource = GetComponent<AudioSource>();
        totalWithSubstring = 0;

        // Zähle die Anzahl der Objekte mit dem Substring im Namen
        foreach (Transform child in ProduktParent.transform)
        {
            if (child.gameObject.name.Contains(substring))
            {
                totalWithSubstring++;
            }
        }

        UpdateInfoText();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Überprüfe, ob das kollidierte Objekt den Tag "cncproduct" hat und ob es noch nicht gezählt wurde
        if (other.gameObject.tag == "cncproduct" && !countedObjects.Contains(other.gameObject))
        {
            // Füge das Objekt zur HashSet hinzu
            countedObjects.Add(other.gameObject);

            // Option 1: Deaktiviere den Renderer, um das Objekt unsichtbar zu machen
            other.gameObject.GetComponent<Renderer>().enabled = false;

            if (!m_AudioSource2.isPlaying)
            {
                m_AudioSource2.PlayOneShot(aclipCutting);
            }

            // Überprüfe, ob der Name des Objekts den Substring enthält
            if (other.gameObject.name.Contains(substring))
            {
                correctCut++;
            }
            else
            {
                falseCut++;
            }

            UpdateInfoText();
        }
    }

    public void UpdateInfoText()
    {
        int totalProcessed = correctCut;
        float percentCorrect = (float)correctCut / totalWithSubstring * 100f;

        infoText.text = $"Aktuelle korrekt gefräst: {correctCut} Stück von {totalWithSubstring} (Gesamtzahl) also {percentCorrect:F2} Prozent\nFalsch gefräste Teile: {falseCut} Stück.";

        if (percentCorrect >= 50 && prozent50Flag)
        {
            PlayClip(aclip150prozent);
            prozent50Flag = false;
        }
        else if (percentCorrect == 100 && prozent100Flag)
        {
            PlayClip(aclip2100prozent);
            prozent100Flag = false;
        }
    }

    private void PlayClip(AudioClip clip)
    {
        if (m_AudioSource.clip != clip)
        {
            m_AudioSource.clip = clip;
            m_AudioSource.Play();
        }
    }
}
