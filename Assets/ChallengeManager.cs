using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChallengeManager : MonoBehaviour
{
    [Header("Spezifische Listen für Challenges")]
    public List<GameObject> findTheLadyObjects = new List<GameObject>();
    public List<GameObject> flyA340Objects = new List<GameObject>();
    public List<GameObject> quizHighLiftObjects = new List<GameObject>();
    public List<GameObject> cabinA330Objects = new List<GameObject>();
    public List<GameObject> introObjects = new List<GameObject>();

    [Header("Allgemeine Liste für Deaktivierung")]
    public List<GameObject> allObjectsToDisable = new List<GameObject>();

    public QuizManager quizManager;
    public TMP_Text debuginfo1;

    public GameObject OvrCameraRig;
    public GameObject ControlTableSphere;

    // Methode zum Deaktivieren aller Objekte
    private void Start()
    {
        Intro();
    }

    public void DisableAll()
    {
        foreach (GameObject obj in allObjectsToDisable)
        {
            obj.SetActive(false);
        }
    }

    // Methoden für spezifische Challenges

    public void moveToControlTable()
    {
        OvrCameraRig.transform.position = new Vector3(ControlTableSphere.transform.position.x, OvrCameraRig.transform.position.y,
            ControlTableSphere.transform.position.z);
        OvrCameraRig.transform.rotation = ControlTableSphere.transform.rotation;
    }
    public void FindTheLady()
    {
        moveToControlTable();
        DisableAll();
        ToggleFunction(findTheLadyObjects);
        

    }

    public void FlyA304()
    {
        moveToControlTable();
        DisableAll();
        ToggleFunction(flyA340Objects);
    }

    public void QuizHighLift()
    {
        DisableAll();
        //debuginfo1.text = debuginfo1.text + "\nbeginn resetQuiz";
        ToggleFunction(quizHighLiftObjects);
        quizManager.initialize();
        moveToControlTable(); 
    }

    public void CabinA330()
    {
        DisableAll();
        ToggleFunction(cabinA330Objects);
        moveToControlTable();
    }

    public void Intro()
    {
        DisableAll();
        ToggleFunction(introObjects);
        moveToControlTable();
    }

    // Allgemeine Toggle-Funktion, die eine Liste von Objekten aktiviert und dann die Intro-Funktion aufruft
    private void ToggleFunction(List<GameObject> objects)
    {
        bool isActive = objects.Count > 0 && objects[0].activeSelf;
        if (!isActive)
        {
            SetActiveState(objects, true);
        }
        else
        {
            Intro();
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
