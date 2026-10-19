using UnityEngine;

public class FlugzeugBewegung : MonoBehaviour
{
    public float geschwindigkeit = 1f;
    public float kreisRadius = 3f;
    public Vector3 kreisZentrum = new Vector3(0, 0, 0);

    private float aktuelleZeit = 0;

    void Start()
    {
        // Setze die Startposition auf 3 Uhr relativ zum Kreiszentrum
        transform.position = kreisZentrum + new Vector3(kreisRadius, 0, 0);

        // Initialisiere die aktuelle Zeit so, dass das Flugzeug bei 3 Uhr startet
        aktuelleZeit = Mathf.PI / 2; // 3 Uhr entspricht 90 Grad oder PI/2 im Bogenmaß
    }

    void Update()
    {
        // Aktualisiere die Zeit basierend auf der Geschwindigkeit
        aktuelleZeit += Time.deltaTime * geschwindigkeit;

        // Berechne die neue Position des Flugzeugs
        float x = Mathf.Cos(aktuelleZeit) * kreisRadius + kreisZentrum.x;
        float z = Mathf.Sin(aktuelleZeit) * kreisRadius + kreisZentrum.z;
        Vector3 neuePosition = new Vector3(x, transform.position.y, z);

        // Bewege das Flugzeug zur neuen Position
        transform.position = neuePosition;

        // Berechne die Richtung der Tangente an der aktuellen Position
        Vector3 tangentenRichtung = new Vector3(-Mathf.Sin(aktuelleZeit), 0, Mathf.Cos(aktuelleZeit));
        Vector3 neueBlickrichtung = neuePosition + tangentenRichtung;

        // Rotiere das Flugzeug, damit seine X-Achse tangential zur Kreisbahn bleibt
        transform.rotation = Quaternion.LookRotation(tangentenRichtung);
    }
}
