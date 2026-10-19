//20240224-2333-Sa neue version - status wird nur nach prüfeFelder Aufruf aktualisiert

using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class QuizBereich : MonoBehaviour
{
    public AudioSource as1;
    public AudioClip aclip1;
    public int bereichsID; // Eindeutige ID für den Bereich
    private QuizManager quizManager;
    public bool flagCorrect;

    TMP_Text info22;


    private List<QuizObjekt> objekteInDiesemBereich = new List<QuizObjekt>();

    private void Start()
    {
        flagCorrect = false;
        quizManager = GameObject.Find("QuizManagerObj").GetComponent<QuizManager>();
        if (GameObject.Find("CanvasPCPLAY") != null)
        {
            info22 = GameObject.Find("info22").GetComponent<TMP_Text>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        QuizObjekt quizObjekt = other.GetComponent<QuizObjekt>();
        if (quizObjekt != null)
        {
            objekteInDiesemBereich.Add(quizObjekt);
            info22.text = "triggerenter: " + quizObjekt.name;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        QuizObjekt quizObjekt = other.GetComponent<QuizObjekt>();
        if (quizObjekt != null)
        {
            objekteInDiesemBereich.Remove(quizObjekt);
            info22.text = "triggerEXIT: " + quizObjekt.name;
        }
    }

    public void prüfeBereich()
    {
        flagCorrect = false; // Zurücksetzen für diese Überprüfung

        foreach (var objekt in objekteInDiesemBereich)
        {
            if (objekt.objektID == bereichsID)
            {
                if (!flagCorrect) // Nur einmal pro Bereich zählen
                {
                    quizManager.noCorrect++;
                    flagCorrect = true;
                }
            }
            else
            {
                // Zählt jedes falsch zugeordnete Objekt in diesem Bereich
                quizManager.noFalse++;
            }
        }
    }




}



// Code version: jedes enter aktualisiert status
/* using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

public class QuizBereich : MonoBehaviour
{
    public AudioSource as1;
    public AudioClip aclip1;
    public int bereichsID; // Eindeutige ID für den Bereich
    private QuizManager quizManager;
    public bool flagCorrect;
    PlayAudioClip playAudioClip;
    GameObject effects;

    private void Start()
    {


        flagCorrect = false;
        quizManager = GameObject.Find("QuizManagerObj").GetComponent<QuizManager>();
        playAudioClip = GameObject.Find("AudioObj1").GetComponent<PlayAudioClip>();
        effects = GameObject.Find("effects-FireworksTrigger");

    }
    private void OnTriggerEnter(Collider other)
    {
        QuizObjekt quizObjekt = other.GetComponent<QuizObjekt>();
        if (quizObjekt != null)
        {
            Debug.Log($"Objekt {quizObjekt.objektID} ist in Bereich {bereichsID} eingetreten.");
            // Hier können Sie Logik hinzufügen, um zu überprüfen, ob das Objekt im richtigen Bereich ist
            if (quizObjekt.objektID == bereichsID)
            {
                if (!flagCorrect)
                {
                    ++quizManager.noCorrect;
                    quizManager.tmpNoCorrect.text = "Anzahl korrekt: " + quizManager.noCorrect.ToString();
                    flagCorrect = true;
                }
            }
            else
            {
                ++quizManager.noFalse;
                quizManager.tmpNoFalse.text = "Anzahl falsch: " + quizManager.noFalse.ToString();
            }
            quizManager.headerErgebnis = quizManager.noCorrect - quizManager.noFalse;

            quizManager.tmpHeaderErgebnis.text = CheckValue(quizManager.headerErgebnis);


        }
    }
    public string CheckValue(int result)
    {
        string resultText = "Ihr aktuelles Ergebnis: " + quizManager.headerErgebnis.ToString();
        if (result < 6) { effects.GetComponent<FireworksTrigger>().StopFireworks(); }
        if (result < 2) { resultText = resultText + "\nKeine Sorge, weiter lernen \nund du wirst das Quiz meistern!"; }
        if (result == 2) { resultText = resultText + "\nDu bist auf dem Weg! \nÜberprüfe und versuche es erneut für ein besseres Ergebnis."; }
        if (result == 3) { resultText = resultText + "\nGute Anstrengung! \nDu hast die Hälfte richtig, bleib dran."; }
        if (result == 4) { resultText = resultText + "\nGut gemacht! \nDu bist schon über die Hälfte hinaus."; }
        if (result == 5) { resultText = resultText + "\nTolle Arbeit! \nDu bist fast an der Spitze."; }
        if (result == 6)
        {
            resultText = resultText + "\nAusgezeichnet! \nDu hast alle Teile gemeistert!";
            playAudioClip.PlayClipFromTo();
            GameObject effects = GameObject.Find("effects-FireworksTrigger");
            effects.GetComponent<FireworksTrigger>().TriggerFireworks();

        }
        return resultText;

    }

} */


