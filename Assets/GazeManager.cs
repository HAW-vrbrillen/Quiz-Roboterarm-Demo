/*
 * 
In diesem Skript:

- `Update()` verarbeitet den Gaze-Eingang und kontrolliert die Skalierung des roten Punkts.
- `ActivateObject()` zeigt die Erklärung des anvisierten Objekts auf dem zentralen Panel an und hebt das Objekt hervor.
- `HighlightObject()` und `ResetHighlightIfNotNull()` steuern das visuelle Hervorheben und Zurücksetzen des Hervorhebens für Objekte.

Stellen Sie sicher, dass alle Komponenten und Objekte, auf die im Skript verwiesen wird, korrekt in Ihrem Unity-Projekt eingerichtet und im Inspector zugewiesen sind.

 */

using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Rendering;

[System.Serializable]
public class GazeTargetInfo
{
    public GameObject targetObject;
    public string questionText;
    public string answerText;
}

public class GazeManager : MonoBehaviour
{
    public Material highlightMaterial;
    [Header("Werden in Start gesetzt:")]
    public TMP_Text answerTmptext;
    public TMP_Text questionTmptext;
    public TMP_Text headerTmptext;
    public GameObject redDotPrefab;

    private GameObject lastHighlighted;
    private GameObject lastRedDot;
    private GameObject currentGazeTarget;
    private Vector3 redDotOriginalSize;
    private Dictionary<GameObject, Material> originalMaterials = new Dictionary<GameObject, Material>();
    private Dictionary<string, string> objectExplanations = new Dictionary<string, string>();
    private int gazeTargetLayer;
    private float gazeTime = 0f;
    private const float requiredGazeTime = 3f;
    private List<GazeTargetInfo> gazeTargets = new List<GazeTargetInfo>(); // Hinzugefügt

    private void InitializeObjectExplanations()
    {
        // Initialisieren Sie die Objekterklärungen mit Fragen und Antworten
        gazeTargets.Add(new GazeTargetInfo
        {
            targetObject = GameObject.Find("ViewWheel_FR1"),
            questionText = "Was ist das Besondere an diesem Rad?",
            answerText = "1.Lauffläche: Außenschicht für Straßenkontakt, mit tiefem Profil für Traktion." +
            "\n2.Seitenwand: Flexibler Reifenteil zwischen Lauffläche und Felge.\n" +
            "3.Gürtelschichten: Stahl- oder Textilgürtel unter der Lauffläche für Stabilität.\r\n" +
            "4.Karkasse: Mehrschichtige Gewebeschicht, gibt dem Reifen Struktur.\r\n" +
            "5.Innenschicht: Luftundurchlässige Gummischicht, hält Luft im Reifen.\r\n" +
            "6.Wulst: Stahlseilbereich, sichert den Reifen auf der Felge.\r" +
            "\n7.Spezialprofil: Für verbesserte Leistung auf verschiedenen Untergründen."

            // ... Rest der Erklärung
        });
        gazeTargets.Add(new GazeTargetInfo
        {
            targetObject = GameObject.Find("RightWindow1"),
            questionText = "Was ist besonders an PKW Fenstern?",
            answerText = "Sicherheitsglas: PKW-Fenster bestehen meist aus Sicherheitsglas, " +
            "\ndas bei einem Bruch in stumpfe Stücke zerfällt oder " +
            "\ndurch eine Kunststoffschicht zusammengehalten wird, um Verletzungen zu vermeiden." +
            "\nUV-Schutz und Tönung: " +
            "Viele Autofenster schützen vor UV-Strahlen und sind getönt, " +
            "was die Insassen vor Sonne schützt und für mehr Privatsphäre sorgt." +
            "\nIntegrierte Sensoren: " +
            "Moderne PKW-Fenster können mit Sensoren ausgestattet sein, " +
            "die beispielsweise Regen erkennen und das Schließen der Fenster automatisch auslösen, oder Einklemmschutz bieten, der das Fenster stoppt, wenn ein Hindernis im Weg ist."

            // ... Rest der Erklärung
        });


        // Wiederholen Sie den Vorgang für jedes andere Objekt, das Sie hinzufügen möchten
        // Stellen Sie sicher, dass Sie das entsprechende GameObject im Szenario finden
    }


    public void ShowAnswer()
    {
        if (currentGazeTarget != null)
        {
            foreach (var targetInfo in gazeTargets)
            {
                if (targetInfo.targetObject == currentGazeTarget)
                {
                    answerTmptext.text = targetInfo.answerText;
                    break;
                }
            }
        }
    }




    private void Start()
    {
        gazeTargetLayer = LayerMask.NameToLayer("GazeTarget");
        //Debug.Log("im start: gazeTargetLayer=" + gazeTargetLayer);
        
        redDotOriginalSize = redDotPrefab.transform.localScale;
        questionTmptext = GameObject.Find("ProduktInfoZentraleQuestion").GetComponent<TMP_Text>();
        answerTmptext = GameObject.Find("ProduktInfoZentraleAnswer").GetComponent<TMP_Text>();
        headerTmptext = GameObject.Find("ProduktInfoZentraleHeader").GetComponent<TMP_Text>();

        InitializeObjectExplanations();
        AssignTextToObjects();
    }

    // Methode zum Hervorheben aller Objekte im Layer "GazeTarget" für 5 Sekunden
    public void HighlightAllGazeTargets()
    {
        StartCoroutine(HighlightAllGazeTargetsCoroutine());
    }

    private IEnumerator HighlightAllGazeTargetsCoroutine()
    {
        // Zuerst alle Objekte im "GazeTarget" Layer in Grün hervorheben
        foreach (GameObject obj in FindObjectsOfType<GameObject>())
        {
            if (obj.layer == gazeTargetLayer)
            {
                Renderer renderer = obj.GetComponent<Renderer>();
                if (renderer != null)
                {
                    originalMaterials[obj] = renderer.material;
                    renderer.material.color = Color.red; // Objekte im Layer "GazeTarget" in Grün hervorheben
                }
            }
        }

        // Dann die speziellen GazeTargets in Gelb hervorheben
        foreach (var targetInfo in gazeTargets)
        {
            Renderer renderer = targetInfo.targetObject.GetComponent<Renderer>();
            if (renderer != null)
            {
                originalMaterials[targetInfo.targetObject] = renderer.material;
                renderer.material.color = Color.yellow; // Spezielle GazeTargets in Gelb hervorheben
            }
        }

        yield return new WaitForSeconds(5);

        // Zurücksetzen der Farben
        foreach (GameObject obj in FindObjectsOfType<GameObject>())
        {
            if (originalMaterials.ContainsKey(obj))
            {
                Renderer renderer = obj.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material = originalMaterials[obj];
                }
            }
        }
    }




    private void AssignTextToObjects()
    {
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        foreach (var obj in allObjects)
        {
            if (obj.layer == gazeTargetLayer)
            {
                foreach (var targetInfo in gazeTargets)
                {
                    if (targetInfo.targetObject == obj)
                    {
                        // Hier könnten Sie zusätzliche Aktionen durchführen, falls erforderlich
                        // Zum Beispiel: Speichern von zusätzlichen Informationen oder Initialisieren von Zuständen
                        break; // Das entsprechende Objekt wurde gefunden und behandelt
                    }
                }
            }
        }
    }



    void Update()
    {
        Transform cameraTransform = Camera.main.transform;
        Vector3 gazeDirection = cameraTransform.forward;

        Vector3 redDotPosition = cameraTransform.position + gazeDirection * 0.31f; // 31 cm vor der Kamera

        if (lastRedDot == null)
        {
            lastRedDot = Instantiate(redDotPrefab, redDotPosition, Quaternion.identity);
            lastRedDot.transform.localScale = redDotOriginalSize * 0.1f; // Startet klein
        }
        else
        {
            lastRedDot.transform.position = redDotPosition;
        }

        RaycastHit hit;
        if (Physics.Raycast(cameraTransform.position, gazeDirection, out hit, Mathf.Infinity))
        {
            // Überprüfen, ob das aktuelle Zielobjekt geändert wurde
            if (currentGazeTarget != hit.collider.gameObject)
            {
                currentGazeTarget = hit.collider.gameObject;
                gazeTime = 0f; // Zurücksetzen der Gaze-Zeit, wenn neues Objekt anvisiert wird
            }

            if (hit.collider.gameObject.layer == gazeTargetLayer)
            {
                if (gazeTime < requiredGazeTime)
                {
                    gazeTime += Time.deltaTime;
                    float scaleFraction = Mathf.Lerp(0.1f, 1.0f, gazeTime / requiredGazeTime);
                    lastRedDot.transform.localScale = redDotOriginalSize * scaleFraction;
                }

                if (gazeTime >= requiredGazeTime && !lastRedDot.transform.localScale.Equals(redDotOriginalSize * 0.1f))
                {
                    ActivateObject(hit.collider.gameObject);
                    lastRedDot.transform.localScale = redDotOriginalSize * 0.1f; // Verkleinern und beibehalten
                }
            }
            else
            {
                ResetRedDotSize();
            }
        }
        else
        {
            ResetRedDotSize();
            currentGazeTarget = null;
        }
    }

    void ResetRedDotSize()
    {
        if (lastRedDot != null)
        {
            gazeTime = 0f; // Zeit zurücksetzen
            lastRedDot.transform.localScale = redDotOriginalSize * 0.1f; // Zurücksetzen auf kleine Größe
        }
    }

    void ActivateObject(GameObject obj)
    {
        questionTmptext.text = "Frage zu " + obj.name + ": Noch nicht definiert.";
        answerTmptext.text = "nicht definiert";
        headerTmptext.text = "Ausgewähltes Objekt: " + obj.name;
        foreach (var targetInfo in gazeTargets)
        {
            if (targetInfo.targetObject == obj)
            {
                questionTmptext.text = "Frage zu " + obj.name + ":\n" + targetInfo.questionText;
                answerTmptext.text = "Wissen Sie es? Durch klick auf Button - Zeige Antwort - prüfen.";
                HighlightObject(obj);
                break;
            }
        }
    }


    void HighlightObject(GameObject obj)
    {
        if (lastHighlighted != obj)
        {
            ResetHighlightIfNotNull();
            lastHighlighted = obj;

            Renderer objRenderer = obj.GetComponent<Renderer>();
            if (objRenderer != null)
            {
                if (!originalMaterials.ContainsKey(obj))
                {
                    originalMaterials[obj] = objRenderer.material;
                }
                objRenderer.material = highlightMaterial;
            }
        }
    }

    void ResetHighlightIfNotNull()
    {
        if (lastHighlighted != null)
        {
            Renderer objRenderer = lastHighlighted.GetComponent<Renderer>();
            if (objRenderer != null && originalMaterials.ContainsKey(lastHighlighted))
            {
                objRenderer.material = originalMaterials[lastHighlighted];
            }

            lastHighlighted = null;
        }
    }
}

