using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ArbeitsSchrittNeu
{
    public string machineID;
    public float bearbeitungszeit;
    public List<GameObject> zuAktivierendeObjekteNachSchritt;
    public List<GameObject> zuDeaktivierenImTeilespeicher;

    private const float transportgeschwindigkeit = 0.65f / 6.0f; // 0.65m in 6 Sekunden

    public ArbeitsSchrittNeu(string id, float bearbeitungszeit, List<GameObject> zuAktivierendeObjekte)
    {
        this.machineID = id;
        this.bearbeitungszeit = bearbeitungszeit;
        this.zuAktivierendeObjekteNachSchritt = zuAktivierendeObjekte ?? new List<GameObject>();
        this.zuDeaktivierenImTeilespeicher = new List<GameObject>();
    }

    public float BerechneTransportzeit(float distanzInMetern)
    {
        return distanzInMetern / transportgeschwindigkeit; // Zeit in Sekunden
    }

    public float BerechneTransportentfernung(float distanzInMetern)
    {
        // 0,65m in der Simulation entsprechen 9650km in der Realität.
        float entfernungInKilometern = (distanzInMetern / 0.56f) * 13000.0f; // in Kilometern
        return entfernungInKilometern;
    }

    public float BerechneCO2Menge(float entfernungInKilometern)
    {
        // CO2-Menge: 50 kg CO2 pro 100 km
        return (50.0f / 100.0f) * entfernungInKilometern;
    }

    public float BerechneCO2Kosten(float co2Menge)
    {
        // CO2-Kompensationskosten: 0,03 Euro pro kg CO2
        return co2Menge * 0.03f;
    }
}


public class ProduktNeu : MonoBehaviour
{
    public string produktTyp; // "Fahrwerk" oder "Chassis"
    public List<ArbeitsSchrittNeu> arbeitsPlan;
    public int aktuellerSchrittIndex = 0;
    private MaschinenZelle _aktuelleMaschinenZelle;

    public bool istFertig = false;

    private float startZeit; // Startzeit für die DLZ

    private ProduktionsSteuerungNEU steuerung; // Hinzufügen der Referenz zur ProduktionsSteuerungNEU

    private Vector3 initialPosition; // Ursprüngliche Position
    private Quaternion initialRotation; // Ursprüngliche Rotation

    private Dictionary<Transform, bool> initialActiveStates = new Dictionary<Transform, bool>(); // Ursprüngliche Aktivierungszustände

    void Start()
    {
        steuerung = FindObjectOfType<ProduktionsSteuerungNEU>(); // Findet die ProduktionsSteuerungNEU im Szenen
        initialPosition = transform.position; // Ursprüngliche Position speichern
        initialRotation = transform.rotation; // Ursprüngliche Rotation speichern
        SaveInitialActiveStates(transform); // Ursprüngliche Aktivierungszustände speichern
    }

    private void SaveInitialActiveStates(Transform parent)
    {
        foreach (Transform child in parent)
        {
            initialActiveStates[child] = child.gameObject.activeSelf;
            SaveInitialActiveStates(child); // Rekursiv für Kindeskinder
        }
    }

    private void RestoreInitialActiveStates(Transform parent)
    {
        foreach (Transform child in parent)
        {
            if (initialActiveStates.ContainsKey(child))
            {
                child.gameObject.SetActive(initialActiveStates[child]);
            }
            RestoreInitialActiveStates(child); // Rekursiv für Kindeskinder
        }
    }

    public void ProduktionStarten()
    {
        if (arbeitsPlan.Count > 0)
        {
            startZeit = Time.time; // Startzeit setzen
            NaechstenSchrittStarten();
        }
    }

    private void NaechstenSchrittStarten()
    {
        if (aktuellerSchrittIndex < arbeitsPlan.Count)
        {
            ArbeitsSchrittNeu schritt = arbeitsPlan[aktuellerSchrittIndex];
            MaschinenZelle maschinenZelle = FindeMaschinenZelleNachID(schritt.machineID);
            if (maschinenZelle != null)
            {
                Debug.Log($"Produkt {this.name} wird zur Warteschlange von Maschine {maschinenZelle.name} hinzugefügt.");
                maschinenZelle.ZurWarteschlangeHinzufuegen(this);
            }
            else
            {
                Debug.LogError($"MaschinenZelle mit ID {schritt.machineID} nicht gefunden für Produkt {this.name}");
            }
        }
        else
        {
            BearbeitungAbgeschlossen();
        }
    }

    public void ResetProduktNeu()
    {
        aktuellerSchrittIndex = 0;
        AktuelleMaschinenZelle = null;
        istFertig = false;
        transform.position = initialPosition; // Position zurücksetzen
        transform.rotation = initialRotation; // Rotation zurücksetzen
        RestoreInitialActiveStates(transform); // Aktivierungszustände zurücksetzen
    }

    public MaschinenZelle AktuelleMaschinenZelle
    {
        get { return _aktuelleMaschinenZelle; }
        private set { _aktuelleMaschinenZelle = value; }
    }

    public void SetzeMaschinenZelle(MaschinenZelle maschinenZelle)
    {
        AktuelleMaschinenZelle = maschinenZelle; // Eigenschaften sollten groß geschrieben werden.
        StartBearbeitung();
    }

    private void StartBearbeitung()
    {
        StartCoroutine(BearbeitungProzess());
    }

    private IEnumerator BearbeitungProzess()
    {
        if (AktuelleMaschinenZelle != null)
        {
            ArbeitsSchrittNeu schritt = arbeitsPlan[aktuellerSchrittIndex];
            Debug.Log($"Produkt {this.name} startet Bearbeitung an Maschine {AktuelleMaschinenZelle.name}");
            yield return new WaitForSeconds(schritt.bearbeitungszeit);
            BearbeitungAbgeschlossen();
        }
        else
        {
            Debug.LogError("AktuelleMaschinenZelle ist null!");
        }
    }

    public void BearbeitungAbgeschlossen()
    {
        aktuellerSchrittIndex++;
        if (aktuellerSchrittIndex < arbeitsPlan.Count)
        {
            AktiviereObjekteNachSchritt();
            NaechstenSchrittStarten();
            if (AktuelleMaschinenZelle != null)
            {
                AktuelleMaschinenZelle.BearbeitungAbgeschlossen();
            }
        }
        else
        {
            float endZeit = Time.time;
            float durchlaufzeit = endZeit - startZeit; // DLZ berechnen
            float sumBZ = 0, sumTZ = 0;
            float gesamtTransportentfernung = 0;
            Vector3 letztePosition = initialPosition; // Startposition des Produkts
            foreach (var schritt in arbeitsPlan)
            {
                float distanz = Vector3.Distance(letztePosition, transform.position); // Hier richtige Distanz berechnen
                sumBZ += schritt.bearbeitungszeit;
                sumTZ += schritt.BerechneTransportzeit(distanz);
                gesamtTransportentfernung += schritt.BerechneTransportentfernung(distanz);
                letztePosition = transform.position;
            }
            float wartezeit = durchlaufzeit - sumBZ - sumTZ;

            float transportentfernung = gesamtTransportentfernung;
            float co2Menge = arbeitsPlan[aktuellerSchrittIndex - 1].BerechneCO2Menge(transportentfernung);
            float co2Kosten = arbeitsPlan[aktuellerSchrittIndex - 1].BerechneCO2Kosten(co2Menge);

            Debug.Log($"Produkt {this.name} hat die Endbearbeitung erreicht. DLZ: {durchlaufzeit}, BZ: {sumBZ}, TZ: {sumTZ}, WZ: {wartezeit}");
            Debug.Log($"Transportentfernung: {transportentfernung} km, CO2-Menge: {co2Menge} kg, CO2-Kosten: {co2Kosten} Euro");
            istFertig = true;
            AktiviereObjekteNachSchritt();
            steuerung.AddErgebnis($"Zeit der Fertigstellung: {Mathf.FloorToInt(durchlaufzeit)}s\n{this.name} / DLZ = {durchlaufzeit:F2} / BZ = {sumBZ:F2} / TZ = {sumTZ:F2} / WZ = {(durchlaufzeit - sumBZ - sumTZ):F2}\nTransportentfernung: {transportentfernung:F2} km, CO2-Menge: {co2Menge:F2} kg, CO2-Kosten: {co2Kosten:F2} Euro");
            if (AktuelleMaschinenZelle != null)
            {
                AktuelleMaschinenZelle.BearbeitungAbgeschlossen();
            }
        }
    }

    public void SetArbeitsplanVonUI()
    {
        // Beispiel für das Setzen des Arbeitsplans von der UI
    }

    private void AktiviereObjekteNachSchritt()
    {
        if (arbeitsPlan[aktuellerSchrittIndex - 1].zuAktivierendeObjekteNachSchritt != null)
        {
            foreach (GameObject obj in arbeitsPlan[aktuellerSchrittIndex - 1].zuAktivierendeObjekteNachSchritt)
            {
                if (obj == null)
                {
                    Debug.LogError("Ein zu aktivierendes Objekt ist null.");
                }
                else
                {
                    obj.SetActive(true);
                    Debug.Log($"Objekt {obj.name} wurde aktiviert.");
                }
            }
            foreach (GameObject obj in arbeitsPlan[aktuellerSchrittIndex - 1].zuDeaktivierenImTeilespeicher)
            {
                obj.SetActive(false);
                Debug.Log($"Objekt {obj.name} wurde deaktiviert.");
            }
        }
        else
        {
            Debug.Log("Keine Objekte zu aktivieren für diesen Schritt.");
        }
    }

    private MaschinenZelle FindeMaschinenZelleNachID(string machineID)
    {
        MaschinenZelle[] maschinenZellen = GameObject.Find("MaschinenZellen").GetComponentsInChildren<MaschinenZelle>();
        foreach (MaschinenZelle zelle in maschinenZellen)
        {
            if (zelle.machineID == machineID)
            {
                return zelle;
            }
        }
        return null;
    }

    public int GetAktuellerSchrittIndex()
    {
        return aktuellerSchrittIndex;
    }
}
