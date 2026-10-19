using System.Collections.Generic;
using UnityEngine;

public class flugzeugZielPfad : MonoBehaviour
{
    public Transform zieleParent; // Referenz zum Parent-Objekt der Ziele
    private List<Transform> ziele = new List<Transform>();
    public float geschwindigkeit = 0.2f;
    public float drehGeschwindigkeit = 0.4f;
    private Transform aktuellesZiel;
    private int aktuellesZielIndex = 0;
    public float drehBeginnSchwelle = 0.5f;

    void Start()
    {
        // Füge alle Kindobjekte des Parent-Objekts der Ziele-Liste hinzu
        foreach (Transform kind in zieleParent)
        {
            ziele.Add(kind);
        }

        // Initialisiere das erste Ziel
        if (ziele.Count > 0)
        {
            aktuellesZiel = ziele[0];
            AusrichtungBeiStart();
        }
    }

    void Update()
    {
        if (aktuellesZiel == null) return;

        BewegeZumZiel();
        //DrehungZumZielMitVorausschau();
    }

    void BewegeZumZiel()
    {
        Vector3 richtungZumZiel = (aktuellesZiel.position - transform.position).normalized;
        transform.position += richtungZumZiel * geschwindigkeit * Time.deltaTime;

        // Überprüfe, ob das Ziel erreicht wurde
        if (Vector3.Distance(transform.position, aktuellesZiel.position) < 0.1f)
        {
            aktuellesZielIndex++; // Gehe zum nächsten Ziel
            if (aktuellesZielIndex >= ziele.Count)
            {
                aktuellesZielIndex = 0; // Starte von vorne, wenn alle Ziele erreicht wurden
            }
            aktuellesZiel = ziele[aktuellesZielIndex];
        }
    }

    /* void DrehungZumZielMitVorausschau()
    {
        Vector3 zielRichtung = (aktuellesZiel.position - transform.position).normalized;
        // Bestimme die Position des übernächsten Ziels, um die Ausrichtung vorherzusagen
        int naechsterZielIndex = (aktuellesZielIndex + 1) % ziele.Count;
        Vector3 uebernaechstesZielRichtung = (ziele[naechsterZielIndex].position - transform.position).normalized;

        // Mische die Richtungen zum aktuellen und übernächsten Ziel für eine sanftere Kurve
        Vector3 gemischteRichtung = Vector3.Lerp(zielRichtung, uebernaechstesZielRichtung, 0.5f).normalized;
        Quaternion zielRotation = Quaternion.LookRotation(gemischteRichtung) * Quaternion.Euler(0, 180, 0);
        transform.rotation = Quaternion.Slerp(transform.rotation, zielRotation, drehGeschwindigkeit * Time.deltaTime);
    } */
    /* void DrehungZumZielMitVorausschau() // gut aber leichter ruck in drehung
    {
        if (aktuellesZiel == null || ziele.Count < 2) return; // Sicherstellen, dass es mindestens 2 Ziele gibt

        // Berechnung der Richtung zum aktuellen Ziel
        Vector3 richtungZumAktuellenZiel = (aktuellesZiel.position - transform.position).normalized;
        // Bestimmung des übernächsten Ziels
        int indexDesUebernaechstenZiels = (aktuellesZielIndex + 1) % ziele.Count;
        Vector3 richtungZumUebernaechstenZiel = (ziele[indexDesUebernaechstenZiels].position - transform.position).normalized;

        // Berechnung der Distanz zum aktuellen Ziel
        float distanzZumAktuellenZiel = Vector3.Distance(transform.position, aktuellesZiel.position);
        // Ermittlung der Gesamtdistanz zwischen dem aktuellen und dem übernächsten Ziel für die Skalierung
        float gesamtDistanzZwischenZielen = Vector3.Distance(aktuellesZiel.position, ziele[indexDesUebernaechstenZiels].position);

        // Skalieren der Distanz zum aktuellen Ziel auf einen Wert zwischen 0 und 1, basierend auf der Gesamtdistanz
        float skalierteDistanz = Mathf.Clamp01(distanzZumAktuellenZiel / gesamtDistanzZwischenZielen);

        // Verwendung der skalierten Distanz, um zu bestimmen, wann mit der Drehung begonnen werden soll
        // siehe oben: drehBeginnSchwelle = 0.5f; // Beginne die Drehung, wenn 50% der Distanz zum aktuellen Ziel zurückgelegt sind
        if (skalierteDistanz < drehBeginnSchwelle)
        {
            // Berechnung einer interpolierten Richtung, die mehr Gewicht auf das übernächste Ziel legt, je näher wir dem aktuellen Ziel kommen
            float interpolationsFaktor = skalierteDistanz / drehBeginnSchwelle; // Dieser Faktor steigt von 0 bis 1, während das Ziel angenähert wird
            Vector3 interpolierteRichtung = Vector3.Lerp(richtungZumAktuellenZiel, richtungZumUebernaechstenZiel, interpolationsFaktor).normalized;

            // Anwendung der interpolierten Richtung für die Drehung
            Quaternion zielRotation = Quaternion.LookRotation(interpolierteRichtung) * Quaternion.Euler(0, 180, 0); // Anpassung, falls nötig
            transform.rotation = Quaternion.Slerp(transform.rotation, zielRotation, drehGeschwindigkeit * Time.deltaTime);
        }
        else
        {
            // Standarddrehung zum aktuellen Ziel, wenn die Schwelle noch nicht erreicht ist
            Quaternion zielRotation = Quaternion.LookRotation(richtungZumAktuellenZiel) * Quaternion.Euler(0, 180, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, zielRotation, drehGeschwindigkeit * Time.deltaTime);
        }
    } */
    /* void DrehungZumZielMitVorausschau() // schon gut aber immer noch kleiner ruck
    {
        if (aktuellesZiel == null || ziele.Count < 2) return; // Sicherstellen, dass mindestens 2 Ziele vorhanden sind

        // Bestimmung der aktuellen und der nächsten Zielindexe
        int naechsterZielIndex = (aktuellesZielIndex + 1) % ziele.Count;

        // Berechnung der interpolierten Zielposition
        float interpolation = Mathf.InverseLerp(0, Vector3.Distance(transform.position, aktuellesZiel.position), Vector3.Distance(transform.position, ziele[naechsterZielIndex].position));
        Vector3 interpolierteZielPosition = Vector3.Lerp(aktuellesZiel.position, ziele[naechsterZielIndex].position, interpolation);

        // Ausrichtung auf die interpolierte Zielposition
        Vector3 zielRichtung = (interpolierteZielPosition - transform.position).normalized;
        Quaternion zielRotation = Quaternion.LookRotation(zielRichtung) * Quaternion.Euler(0, 180, 0); // Anpassung der Rotation, falls erforderlich
        transform.rotation = Quaternion.Slerp(transform.rotation, zielRotation, drehGeschwindigkeit * Time.deltaTime);
    } */
    // jetzt Spline Kurve durch punkte
    




    void AusrichtungBeiStart()
    {
        Vector3 startRichtung = (aktuellesZiel.position - transform.position).normalized;
        Quaternion startRotation = Quaternion.LookRotation(startRichtung) * Quaternion.Euler(0, 180, 0);
        transform.rotation = startRotation;
    }
}
