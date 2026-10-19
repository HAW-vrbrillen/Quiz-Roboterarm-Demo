using System.Collections.Generic;
using UnityEngine;

public class FlugzeugBezierPfad : MonoBehaviour //FlugzeugCatmullRomPfad
{
    public List<Transform> waypoints; // Liste der Wegpunkte
    public float geschwindigkeit = 5f;
    private float t = 0; // Parametrischer Wert für die Interpolation
    private int aktuellerWegpunktIndex = 0;

    void Update()
    {
        if (waypoints.Count < 4) return; // Benötigt mindestens 4 Punkte

        Vector3 p0 = waypoints[(aktuellerWegpunktIndex + waypoints.Count - 1) % waypoints.Count].position;
        Vector3 p1 = waypoints[aktuellerWegpunktIndex % waypoints.Count].position;
        Vector3 p2 = waypoints[(aktuellerWegpunktIndex + 1) % waypoints.Count].position;
        Vector3 p3 = waypoints[(aktuellerWegpunktIndex + 2) % waypoints.Count].position;

        t += Time.deltaTime * geschwindigkeit;

        if (t >= 1f)
        {
            t = 0;
            aktuellerWegpunktIndex++;
            if (aktuellerWegpunktIndex >= waypoints.Count - 2)
            {
                aktuellerWegpunktIndex = 0; // Zurück zum Anfang, wenn das Ende erreicht ist
            }
        }

        transform.position = CatmullRom(p0, p1, p2, p3, t);

        // Ausrichtung basierend auf der Bewegungsrichtung
        Vector3 nextPosition = CatmullRom(p0, p1, p2, p3, t + 0.05f);
        transform.forward = (nextPosition - transform.position).normalized;
    }

    Vector3 CatmullRom(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        float t2 = t * t;
        float t3 = t2 * t;

        float b1 = 0.5f * (-t3 + 2f*t2 - t);
        float b2 = 0.5f * (3f*t3 - 5f*t2 + 2f);
        float b3 = 0.5f * (-3f*t3 + 4f*t2 + t);
        float b4 = 0.5f * (t3 - t2);

        return p0 * b1 + p1 * b2 + p2 * b3 + p3 * b4;
    }
}
