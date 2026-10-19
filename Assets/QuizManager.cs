using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class QuizManager : MonoBehaviour
{
    public int headerErgebnis;
    public TMP_Text tmpHeaderErgebnis;
    public int noCorrect;
    public TMP_Text tmpNoCorrect;
    public int noFalse;
    public TMP_Text tmpNoFalse;
    public int lastNoCorrect;
    public TMP_Text tmpLastNoCorrect;
    public int lastNoFalse;
    public TMP_Text tmpLastNoFalse;
    public Transform pins;
    public GameObject quizHelp;
    public int secToShow;
    public GameObject QuizMainPanel;

    public TMP_Text debuginfo1;

    public Transform solutionRegions; // NEU 20240225-1458-So

    // Eine Liste, um die ursprünglichen Positionen und Rotationen zu speichern
    private List<Vector3> originalPositions = new List<Vector3>();
    private List<Quaternion> originalRotations = new List<Quaternion>();

    PlayAudioClip playAudioClip;
    GameObject effects;

    //GameObject solutionRegions;

    // Start is called before the first frame update
    // Achtung START wird nicht durchlaufen wenn Objekt nicht Aktiv ist!!
    public void initialize()
    {
        if (originalPositions.Count > 0 && originalRotations.Count > 0)
        {
            //debuginfo1.text = debuginfo1.text + "\n falsch 2ter Aufruf von Ini";
            return; // Sie wurden bereits initialisiert
        }
        //debuginfo1.text = debuginfo1.text + "\n Ini Aufruf korrekt";
        // Initialisiere die Zähler
        noCorrect = 0;
        noFalse = 0;
        lastNoCorrect = 0;
        lastNoFalse = 0;

        playAudioClip = GameObject.Find("AudioObj1").GetComponent<PlayAudioClip>();
        effects = GameObject.Find("effects-FireworksTrigger");

        // Speichere die ursprünglichen Positionen und Rotationen aller Kinder von 'pins'
        foreach (Transform child in pins)
        {
            originalPositions.Add(child.position);
            originalRotations.Add(child.rotation);
          // debuginfo1.text = debuginfo1.text + "\n in ini child.name: " + child.name;
        }
        solutionRegions = GameObject.Find("solutionRegions").transform;

        //ToggleScale(quizHelp);
        //ToggleScale(QuizMainPanel);

    }
   
    

    public void showQuiz()
    {

        //toggleObjectEnable(QuizMainPanel);
        initialize(); // enable erfolt schon im Rahmen choose Challenge
    }

   

    
    // Funktion zum Umschalten der Skala
    // Methode zum Umschalten der Skala des übergebenen GameObjects
    public void ToggleScale(GameObject obj)
    {
        float scaleThreshold = 1f / 500f; // Definiere eine Schwelle für die Verkleinerung
        Vector3 currentScale = obj.transform.localScale;

        // Prüfe, ob die Skala des Objekts kleiner als die Schwelle ist
        if (currentScale.x < scaleThreshold && currentScale.y < scaleThreshold && currentScale.z < scaleThreshold)
        {
            // Wenn das Objekt verkleinert ist, vergrößere es
            obj.transform.localScale = currentScale * 10000f;
        }
        else
        {
            // Wenn das Objekt nicht verkleinert ist, verkleinere es
            obj.transform.localScale = currentScale / 10000f;
        }
    }

    

   

public void prüfeFelder()
    {
        // Aktualisiere lastXXX Werte vor dem Zurücksetzen der aktuellen Zählungen
        lastNoCorrect = noCorrect;
        lastNoFalse = noFalse;

        // Setze die aktuellen Zählungen zurück
        noCorrect = 0;
        noFalse = 0;

        // Führe die Prüfung für jeden Bereich durch
        foreach (Transform bereichTransform in solutionRegions)
        {
            QuizBereich bereich = bereichTransform.GetComponent<QuizBereich>();
            if (bereich != null)
            {
                bereich.prüfeBereich(); // Hier muss sichergestellt sein, dass die Logik korrekt implementiert ist
            }
        }

        // Aktualisiere die UI mit den neuen Werten
        tmpNoCorrect.text = "Anzahl korrekt: " + noCorrect.ToString();
        tmpNoFalse.text = "Anzahl falsch: " + noFalse.ToString();
        tmpLastNoCorrect.text = "Letzte Anzahl korrekt: " + lastNoCorrect.ToString();
        tmpLastNoFalse.text = "Letzte Anzahl falsch: " + lastNoFalse.ToString();
        tmpHeaderErgebnis.text = CheckValue(noCorrect - noFalse);
    }



    public string CheckValue(int result)
    {
        string resultText = "Ihr aktuelles Ergebnis: " + result.ToString();
        if (result < 6) { effects.GetComponent<FireworksTrigger>().StopFireworks(); }
        if (result < 2) { resultText += "\nKeine Sorge, weiter lernen und du wirst das Quiz meistern!"; }
        if (result == 2) { resultText += "\nDu bist auf dem Weg! Überprüfe und versuche es erneut für ein besseres Ergebnis."; }
        if (result == 3) { resultText += "\nGute Anstrengung! Du hast die Hälfte richtig, bleib dran."; }
        if (result == 4) { resultText += "\nGut gemacht! Du bist schon über die Hälfte hinaus."; }
        if (result == 5) { resultText += "\nTolle Arbeit! Du bist fast an der Spitze."; }
        if (result == 6)
        {
            resultText += "\nAusgezeichnet! Du hast alle Teile gemeistert!";
            playAudioClip.PlayClipFromTo();
            effects.GetComponent<FireworksTrigger>().TriggerFireworks();
        }
        return resultText;
    }



    public void ResetQuiz()
    {
        debuginfo1.text = debuginfo1.text + "\nbeginn resetQuiz";
        lastNoCorrect = noCorrect;
        tmpLastNoCorrect.text = "Letzte Anzahl korrekt: " + lastNoCorrect.ToString();
        noCorrect = 0;
        tmpNoCorrect.text = "Anzahl korrekt: 0";
        lastNoFalse = noFalse;
        tmpLastNoFalse.text = "Letzte Anzahl falsch: " + noFalse.ToString();
        noFalse = 0;
        tmpNoFalse.text = "Anzahl falsch : 0";

        // Setze die Positionen und Rotationen der Pins zurück
        int i = 0; // Index zum Durchlaufen der Listen
        foreach (Transform child in pins)
        {
            child.position = originalPositions[i];
            child.rotation = originalRotations[i];
            debuginfo1.text = debuginfo1.text + "\n pinno: " + i;
            i++;
        }
        // setze die quizBereiche zurück
        foreach (Transform child in solutionRegions.transform)
        {
            child.GetComponent<QuizBereich>().flagCorrect = false;

        }

    }

}
