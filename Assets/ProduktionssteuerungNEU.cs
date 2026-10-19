using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ProduktionsSteuerungNEU : MonoBehaviour
{
    private ProduktNeu[] produkte;
    private MaschinenZelle[] maschinenZellen;
    public GameObject textPrefab;
    public Transform panelTransform;

    public TMP_Text arbeitsplanText;
    public TMP_Text produktDatenText;
    public TMP_Text summenDatenText;
    public TMP_Text maschinenzellenText;
    public TMP_Text logText; // TextMeshPro-Textfeld für die Log-Ausgabe
    public TMP_Text ergebnisText; // TextMeshPro-Textfeld für die Ergebnis-Ausgabe
    public GameObject InfoArea;

    private bool showDetailValues = false;
    private List<string> logMeldungen = new List<string>(); // Liste der letzten Meldungen
    private List<string> ergebnisMeldungen = new List<string>(); // Liste der Ergebnismeldungen
    private const int maxMeldungen = 2; // Maximale Anzahl der gespeicherten Meldungen

    public bool flagStartProduction = false;

    void Start()
    {
        produkte = GameObject.Find("Produkte").GetComponentsInChildren<ProduktNeu>();
        maschinenZellen = GameObject.Find("MaschinenZellen").GetComponentsInChildren<MaschinenZelle>();
        GenerateTableHeader();
        InvokeRepeating("UpdateTextDisplay", 1.0f, 1.0f);
        
    }

    private void Update()
    {
        if (flagStartProduction)
        {
            UIAktualisierenUndProduktionStarten();
            flagStartProduction = false;
        }
    }

    public void UIAktualisierenUndProduktionStarten()
    {
        foreach (var produkt in produkte)
        {
            produkt.SetArbeitsplanVonUI();
            produkt.ProduktionStarten();
        }
    }

    public void AlleProdukteZuruecksetzen()
    {
        foreach (var produkt in produkte)
        {
            produkt.ResetProduktNeu();
        }
        foreach (var zelle in maschinenZellen)
        {
            zelle.ResetTeileSpeicher();
        }
        ergebnisMeldungen.Clear(); // Ergebnisse zurücksetzen
        UpdateErgebnisText(); // Ergebnis-Textfeld aktualisieren
    }

    public void AddLog(string meldung)
    {
        Debug.Log("0528 in addlog: " + meldung);
        logMeldungen.Add(meldung);
        if (logMeldungen.Count > maxMeldungen)
        {
            logMeldungen.RemoveAt(0); // Entfernt die älteste Meldung, um Platz für die neue zu schaffen
        }
        UpdateLogText();
    }

    public void AddErgebnis(string meldung)
    {
        ergebnisMeldungen.Add(meldung);
        UpdateErgebnisText();
    }

    private void UpdateLogText()
    {
        if (!showDetailValues)
        {
            Debug.Log("0528 logText setzen: " + logMeldungen);
            logText.text = string.Join("\n\n", logMeldungen);
        }
    }

    private void UpdateErgebnisText()
    {
        ergebnisText.text = string.Join("\n", ergebnisMeldungen);
    }

    public void ToggleDetailLogText()
    {
        showDetailValues = !showDetailValues;
        if (showDetailValues)
        {
            InfoArea.SetActive(true);
            logText.gameObject.SetActive(false);
        }
        else
        {
            logText.gameObject.SetActive(true);
            InfoArea.SetActive(false);
            UpdateLogText(); // Zeigt die Logs, wenn die Details ausgeblendet sind
        }
    }

    private void GenerateTableHeader()
    {
        string[] headers = new string[]
        {
            "Produkt", "MZ1/BZ/TZ", "MZ2/BZ/TZ", "MZ3/BZ/TZ", "MZ4/BZ/TZ", "Status: akt MZ", "Status", "FG", "SumBZ", "SummeTZ", "Sum DLZ", "BZ:DLZ"
        };

        foreach (var header in headers)
        {
            GameObject cell = Instantiate(textPrefab, panelTransform);
            TMP_Text cellText = cell.GetComponent<TMP_Text>();
            cellText.text = header;
        }
    }

    private void UpdateTextDisplay()
    {
        if (showDetailValues)
        {
            arbeitsplanText.text = GetArbeitsplanText();
            produktDatenText.text = GetProduktDatenText();
            summenDatenText.text = GetSummenDatenText();
            maschinenzellenText.text = GetMaschinenzellenText();
        }
    }

    private string GetArbeitsplanText()
    {
        string text = "";
        foreach (var produkt in produkte)
        {
            text += $"Produkt: {produkt.name}, AP: ";
            foreach (var schritt in produkt.arbeitsPlan)
            {
                text += $"{schritt.machineID} (BZ: {schritt.bearbeitungszeit}), ";
            }
            text = text.TrimEnd(',', ' ') + "\n";
        }
        text = "Arbeitspläne:\n" + text;
        return text;
    }

    private string GetProduktDatenText()
    {
        string text = "";
        foreach (var produkt in produkte)
        {
            text += $"Produkt: {produkt.name}, Aktueller Schritt: {produkt.GetAktuellerSchrittIndex()}, Fertig: {produkt.istFertig}, Aktuelle MZ: {(produkt.AktuelleMaschinenZelle != null ? produkt.AktuelleMaschinenZelle.name : "Keine")}\n";
        }
        text = "Produktstatus:\n" + text;
        return text;
    }

    private string GetSummenDatenText()
    {
        string text = "";
        foreach (var produkt in produkte)
        {
            float sumBZ = 0, sumTZ = 0;
            float gesamtTransportEntfernung = 0;
            Vector3 letztePosition = produkt.transform.position;
            foreach (var schritt in produkt.arbeitsPlan)
            {
                float distanz = Vector3.Distance(letztePosition, produkt.transform.position); // Hier richtige Distanz berechnen
                sumBZ += schritt.bearbeitungszeit;
                sumTZ += schritt.BerechneTransportzeit(distanz);
                gesamtTransportEntfernung += schritt.BerechneTransportentfernung(distanz);
                letztePosition = produkt.transform.position;
            }
            float sumDLZ = sumBZ + sumTZ;
            float anteilBZ = (sumDLZ > 0) ? (sumBZ / sumDLZ) * 100 : 0;
            text += $"Produkt: {produkt.name}, Summe BZ: {sumBZ}, Summe TZ: {sumTZ}, Transportentfernung: {gesamtTransportEntfernung:F2} km, CO2-Menge: {produkt.arbeitsPlan[produkt.GetAktuellerSchrittIndex() - 1].BerechneCO2Menge(gesamtTransportEntfernung):F2} kg, CO2-Kosten: {produkt.arbeitsPlan[produkt.GetAktuellerSchrittIndex() - 1].BerechneCO2Kosten(produkt.arbeitsPlan[produkt.GetAktuellerSchrittIndex() - 1].BerechneCO2Menge(gesamtTransportEntfernung)):F2} Euro\n";
        }
        text = "Produkt Zeit-Summen:\n" + text;

        return text;
    }

    private string GetMaschinenzellenText()
    {
        string text = "";
        foreach (var zelle in maschinenZellen)
        {
            text += $"{zelle.name}, Anzahl: {zelle.GetQueueCount()}, Sum BZ: {zelle.GetSumBZ()}, Sum Leerzeit: {zelle.GetSumLeerzeit()}, Auslastung: {zelle.GetAuslastung()}%\n";
        }
        text = "Maschinenzellenstatus:\n" + text;

        return text;
    }
}
