using UnityEngine;

public class ObjektAControllerSmallWorld : MonoBehaviour
{
    public Transform objektB;
    public Transform referenzObjekt; // Das Referenzobjekt, das die ursprüngliche Position markiert
    public float threshold = 1f;
    private Vector3 startRelativePositionAzuReferenz; // Startposition von A relativ zum Referenzobjekt
    private bool farbeGeaendert = false;
    public float scaleDownSpeed = 0.03f; // reduktion weil in Miniwelt

    void Start()
    {
        // Initialisiert die Startposition von A relativ zum Referenzobjekt
        startRelativePositionAzuReferenz = transform.position - referenzObjekt.position;
    }

    void Update()
    {
        // Aktualisiert die relative Position von A zu seinem Referenzobjekt
        Vector3 aktuellePositionAzuReferenz = transform.position - referenzObjekt.position;

        // Berechnet die Veränderung relativ zur Startposition
        if ((aktuellePositionAzuReferenz - startRelativePositionAzuReferenz).magnitude > threshold)
        {
            if (!farbeGeaendert)
            {
                // Ändert die Farbe zu Rot und bereitet die Bewegung vor
                GetComponent<Renderer>().material.color = Color.red;
                farbeGeaendert = true;
            }

            // Berechnet die Bewegungsrichtung basierend auf der aktuellen Position von A relativ zum Referenzobjekt
            Vector3 bewegungsrichtung = aktuellePositionAzuReferenz.normalized;
            objektB.position += bewegungsrichtung * Time.deltaTime * scaleDownSpeed; // Führt die Bewegung aus
        }
        else if (farbeGeaendert)
        {
            // Setzt die Farbe zurück, wenn A wieder innerhalb des Thresholds ist
            GetComponent<Renderer>().material.color = Color.white;
            farbeGeaendert = false;
        }
    }
}
