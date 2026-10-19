using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaschinenZelle : MonoBehaviour
{
    public string machineID;
    public bool isLager = false; // Kennzeichnet, ob dies die Lagerzelle ist
    public GameObject teileSpeicher; // Objekt, das Kinder für das Deaktivieren enthält
    private ProduktionsSteuerungNEU steuerung; // Referenz zur ProduktionsSteuerungNEU

    private Queue<ProduktNeu> inputQueue = new Queue<ProduktNeu>();
    private ProduktNeu aktuellesProdukt;

    private float sumBZ = 0;
    private float sumLeerzeit = 0;
    private float startZeit;
    private float inBearbeitungZeit = 0;
    private float leerzeit = 0;
    private float lagerYOffset = 0.0f; // Y-Versatz für gestapelte Produkte

    void Start()
    {
        steuerung = FindObjectOfType<ProduktionsSteuerungNEU>(); // Findet die ProduktionsSteuerungNEU im Szenen
        startZeit = Time.time;
        InvokeRepeating("UpdateZeiten", 1.0f, 1.0f);
    }

    public void ZurWarteschlangeHinzufuegen(ProduktNeu produkt)
    {
        Debug.Log("0528 zur warteschlange hinzufüge " + produkt.name);
        inputQueue.Enqueue(produkt);
        steuerung.AddLog("[" + (Time.time - startZeit) + "s] Produkt " + produkt.name + " wurde zur Warteschlange von " + this.name + " hinzugefügt. Warteschlange Größe: " + inputQueue.Count);
    }

    void Update()
    {
        if (!isLager && aktuellesProdukt == null && inputQueue.Count > 0)
        {
            steuerung.AddLog("[" + (Time.time - startZeit) + "s] Vor Produktionsprozessstarten: " + this.name);
            ProduktionsprozessStarten();
        }
        else if (isLager && inputQueue.Count > 0)
        {
            while (inputQueue.Count > 0)
            {
                ProduktNeu produkt = inputQueue.Dequeue();
                StartCoroutine(BewegeProduktZurLagerstation(produkt));
            }
        }
    }

    private void ProduktionsprozessStarten()
    {
        if (inputQueue.Count > 0)
        {
            aktuellesProdukt = inputQueue.Dequeue();
            steuerung.AddLog("[" + (Time.time - startZeit) + "s] Produkt " + aktuellesProdukt.name + " startet Produktionsprozess in Maschine " + this.name);
            StartCoroutine(BewegeProduktZurArbeitsstation(aktuellesProdukt));
        }
    }

    private IEnumerator BewegeProduktZurArbeitsstation(ProduktNeu produkt)
    {
        Vector3 startPosition = produkt.transform.position;
        Vector3 zielPosition = transform.position;
        float distanz = Vector3.Distance(startPosition, zielPosition);

        // Berechne die Transportzeit basierend auf der Distanz
        float transportzeit = produkt.arbeitsPlan[produkt.aktuellerSchrittIndex].BerechneTransportzeit(distanz);

        float elapsedTime = 0;

        while (elapsedTime < transportzeit)
        {
            produkt.transform.position = Vector3.Lerp(startPosition, zielPosition, elapsedTime / transportzeit);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        produkt.transform.position = zielPosition;
        steuerung.AddLog("[" + (Time.time - startZeit) + "s] Produkt " + produkt.name + " hat die Arbeitsstation erreicht: " + this.name);
        produkt.SetzeMaschinenZelle(this);
    }

    private IEnumerator BewegeProduktZurLagerstation(ProduktNeu produkt)
    {
        Vector3 startPosition = produkt.transform.position;
        Vector3 zielPosition = transform.position + new Vector3(0, lagerYOffset, 0);
        float transportzeit = 1.0f; // Feste Transportzeit für das Lager, falls gewünscht
        float elapsedTime = 0;

        while (elapsedTime < transportzeit)
        {
            produkt.transform.position = Vector3.Lerp(startPosition, zielPosition, elapsedTime / transportzeit);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        produkt.transform.position = zielPosition;
        lagerYOffset += 0.2f; // Y-Wert um 0.2 erhöhen für das nächste Produkt
        steuerung.AddLog("[" + (Time.time - startZeit) + "s] Produkt " + produkt.name + " hat das Lager erreicht: " + this.name);
        produkt.SetzeMaschinenZelle(this);
        produkt.istFertig = true; // Setze den Status auf "Fertig"
        BearbeitungAbgeschlossenLager(produkt); // Lagerbearbeitung sofort abschließen
    }

    private void UpdateZeiten()
    {
        if (aktuellesProdukt != null)
        {
            inBearbeitungZeit += Time.time - startZeit;
        }
        else
        {
            leerzeit += Time.time - startZeit;
        }
        startZeit = Time.time;
    }

    public bool InBearbeitung(ProduktNeu produkt)
    {
        return produkt == aktuellesProdukt;
    }

    public void BearbeitungAbgeschlossen()
    {
        if (aktuellesProdukt != null)
        {
            steuerung.AddLog("[" + (Time.time - startZeit) + "s] Bearbeitung von Produkt " + aktuellesProdukt.name + " abgeschlossen in Maschine " + this.name);

            int aktuellerIndex = aktuellesProdukt.GetAktuellerSchrittIndex();
            sumBZ += aktuellesProdukt.arbeitsPlan[aktuellerIndex].bearbeitungszeit;

            aktuellesProdukt = null;
        }

        if (inputQueue.Count > 0)
        {
            ProduktionsprozessStarten();
        }
    }

    public void BearbeitungAbgeschlossenLager(ProduktNeu produkt)
    {
        steuerung.AddLog("[" + (Time.time - startZeit) + "s] Bearbeitung von Produkt " + produkt.name + " im Lager abgeschlossen.");
        sumBZ += produkt.arbeitsPlan[produkt.aktuellerSchrittIndex].bearbeitungszeit;
    }

    public string GetMaschinenzellenText()
    {
        float totalTime = sumBZ + inBearbeitungZeit + leerzeit + sumLeerzeit;
        float auslastung = (totalTime > 0) ? ((sumBZ + inBearbeitungZeit) / totalTime) * 100 : 0;
        return $"{name}, Anzahl: {inputQueue.Count}, Sum BZ: {sumBZ}, Sum Leerzeit: {leerzeit + sumLeerzeit}, Auslastung: {auslastung}%\n";
    }

    public int GetQueueCount()
    {
        return inputQueue.Count;
    }

    public float GetSumBZ()
    {
        return sumBZ;
    }

    public float GetSumLeerzeit()
    {
        return leerzeit + sumLeerzeit;
    }

    public float GetAuslastung()
    {
        float totalTime = sumBZ + inBearbeitungZeit + leerzeit + sumLeerzeit;
        return (totalTime > 0) ? ((sumBZ + inBearbeitungZeit) / totalTime) * 100 : 0;
    }

    public void ResetTeileSpeicher()
    {
        if (teileSpeicher != null)
        {
            foreach (Transform child in teileSpeicher.transform)
            {
                child.gameObject.SetActive(true);
            }
        }
    }
}
