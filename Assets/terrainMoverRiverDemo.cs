
using UnityEngine;

public class TerrainMoverRiverDemo : MonoBehaviour
{
    public GameObject terrain;
    public Transform cameraTransform; // Referenz zur VR-Kamera
    public float thresholdRadius = 0.2f; // Radius des Schwellbereichs
    public float movementSpeed = 2f; // Geschwindigkeit der Terrainbewegung

    private Vector3 initialLocalPosition; // Startlokaleposition des Objekts relativ zur Kamera
    private Renderer objectRenderer;
    private Color originalColor;
    private bool isOutsideThreshold = false; // Status, ob das Objekt außerhalb des Schwellenbereichs ist

    void Start()
    {
        // Speichere die Startlokaleposition des Objekts relativ zur Kamera
        initialLocalPosition = transform.localPosition;
        objectRenderer = GetComponent<Renderer>();
        if (objectRenderer != null)
        {
            originalColor = objectRenderer.material.color;
        }
    }

    void Update()
    {
        // Berechne die aktuelle lokale Position des Objekts relativ zur Kamera
        Vector3 currentLocalPosition = transform.localPosition;

        // Berechne die Distanz und Richtung vom aktuellen Punkt zum anfänglichen lokalen Nullpunkt
        Vector3 directionToLocalInitial = currentLocalPosition - initialLocalPosition;

        // Prüfen, ob das Objekt sich außerhalb des Schwellbereichs befindet
        isOutsideThreshold = directionToLocalInitial.sqrMagnitude > thresholdRadius * thresholdRadius;

        if (isOutsideThreshold)
        {
            // Bewegt das Terrain in die entgegengesetzte Richtung der lokalen Vektorbewegung
            terrain.transform.position -= directionToLocalInitial.normalized * movementSpeed * Time.deltaTime;
            ChangeColor(Color.red); // Ändert die Farbe zu Rot
        }
        else
        {
            ChangeColor(originalColor); // Setzt die Farbe zurück, wenn innerhalb des Bereichs
        }
    }

    void ChangeColor(Color newColor)
    {
        if (objectRenderer != null)
        {
            objectRenderer.material.color = newColor;
        }
    }
}


/* version mit bezug zu panel
using UnityEngine;

public class TerrainMoverRiverDemo : MonoBehaviour
{
    public GameObject terrain;
    public GameObject panel; // Das sich bewegende Panel
    public Vector3 thresholdCenter = Vector3.zero; // Mittelpunkt des Schwellbereichs, relativ zum Panel
    public float thresholdRadius = 0.2f; // Radius des Schwellbereichs
    public float movementSpeed = 2f; // Geschwindigkeit der Terrainbewegung

    private Vector3 lastPanelPosition; // Letzte Position des Panels
    private Vector3 initialObjToPanelOffset; // Initialer Abstand zwischen Objekt und Panel
    private Renderer objectRenderer;
    private Color originalColor;
    private bool isOutsideThreshold = false; // Status, ob das Objekt außerhalb des Schwellenbereichs ist

    void Start()
    {
        lastPanelPosition = panel.transform.position;
        initialObjToPanelOffset = transform.position - panel.transform.position;
        objectRenderer = GetComponent<Renderer>();
        if (objectRenderer != null)
        {
            originalColor = objectRenderer.material.color;
        }
    }

    void Update()
    {
        Vector3 panelMovement = panel.transform.position - lastPanelPosition; // Bewegung des Panels seit dem letzten Frame
        Vector3 currentPositionRelativeToPanel = transform.position - panel.transform.position; // Aktuelle Position relativ zum Panel
        Vector3 adjustedPositionRelativeToPanel = currentPositionRelativeToPanel - initialObjToPanelOffset; // Korrigierte Position unter Berücksichtigung des initialen Abstands
        Vector3 thresholdCenterRelativeToPanel = thresholdCenter + panel.transform.position; // Schwellenmittelpunkt relativ zum Panel

        // Prüfen, ob das Objekt sich außerhalb des Schwellbereichs befindet
        isOutsideThreshold = (adjustedPositionRelativeToPanel - thresholdCenter).sqrMagnitude > thresholdRadius * thresholdRadius;

        if (isOutsideThreshold)
        {
            // Bewegt das Terrain in die entgegengesetzte Richtung der Vektorbewegung (Vorzeichen umgekehrt), abzüglich der Panel-Bewegung
            terrain.transform.position -= (adjustedPositionRelativeToPanel.normalized * movementSpeed * Time.deltaTime) + panelMovement;
            ChangeColor(Color.red); // Ändert die Farbe zu Rot
        }
        else
        {
            ChangeColor(originalColor); // Setzt die Farbe zurück, wenn innerhalb des Bereichs
        }

        lastPanelPosition = panel.transform.position; // Aktualisieren der letzten Panel-Position für den nächsten Frame
    }

    void ChangeColor(Color newColor)
    {
        if (objectRenderer != null)
        {
            objectRenderer.material.color = newColor;
        }
    }
}
*/