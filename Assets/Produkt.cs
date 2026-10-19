using System.Collections;
using System.Linq;
using UnityEngine;

public class Produkt : MonoBehaviour
{
    public int ID; // Eindeutige ID des Produkts
    public string Typ; // Typ des Produkts
    public int[] Arbeitsplan; // Array von MachineCell-IDs, die den Arbeitsplan darstellen
    public MachineCell currentMachineCell; // Die MachineCell, in der sich das Produkt derzeit befindet
    private int aktuellerSchritt = 0; // Der aktuelle Schritt im Arbeitsplan
    public float bearbeitungsTimer; // Timer für die Bearbeitungsdauer
    public float Bearbeitungszeit = 5f; // Festgelegte Bearbeitungszeit für das Produkt
    public float bewegungsgeschwindigkeit = 1f; // Geschwindigkeit, mit der sich das Produkt bewegt

    public enum ProduktStatus { InMaschinenzelle, InWarteschlange, AufTransport, InWorkstation, Notdefined }


    public ProduktStatus aktuellerStatus; // Öffentliche Variable für den Inspektor



    private void Start()
    {
        // Setze den Status initial auf 'notdefined'
        aktuellerStatus = ProduktStatus.Notdefined;

        // Überprüfen, ob ein Arbeitsplan existiert
        if (Arbeitsplan.Length > 0)
        {
            var startMachineCell = FindMachineCellByID(Arbeitsplan[0]);
            if (startMachineCell != null)
            {
                // Fügen Sie das Produkt zur InputQueue der Start-MachineCell hinzu
                startMachineCell.AddToInputQueue(this);
                // Setzen Sie den Status auf 'InWarteschlange', da das Produkt in der InputQueue ist
                aktuellerStatus = ProduktStatus.InWarteschlange;
                currentMachineCell = startMachineCell;
            }
            else
            {
                Debug.LogError("Start-MachineCell nicht gefunden.");
            }
        }
        else
        {
            Debug.LogError("Arbeitsplan für Produkt ist leer.");
        }
    }



    private void Update()
    {
        if (aktuellerStatus == ProduktStatus.InWorkstation)
        {
            bearbeitungsTimer += Time.deltaTime;
            if (bearbeitungsTimer >= Bearbeitungszeit)
            {
                BearbeitungBeendet();
            }
        }
    }



    public void SetMachineCell(MachineCell machineCell)
    {
        // Aktualisieren Sie den Status basierend auf dem aktuellen Zustand der Workstation
        aktuellerStatus = machineCell.workstation.childCount == 0 ? ProduktStatus.InWorkstation : ProduktStatus.InWarteschlange;
        currentMachineCell = machineCell;

        // Wenn das Produkt in der Warteschlange ist, fügen Sie es der Input-Queue hinzu
        if (aktuellerStatus == ProduktStatus.InWarteschlange)
        {
            currentMachineCell.AddToInputQueue(this);
        }
        else
        {
            // Starten Sie die Bearbeitung sofort, wenn die Workstation frei ist
            StartBearbeitung();
        }
    }

    public void StartBearbeitung()
    {
        bearbeitungsTimer = 0;
        Debug.Log($"Produkt {ID} beginnt Bearbeitung in MachineCell {currentMachineCell.MachineID}.");
    }

    // Methode, um das Produkt zur Zielstation zu bewegen
    public void BewegeZurStation(MachineCell zielMachineCell)
    {
        // Bestimmen Sie, wohin das Produkt bewegt werden soll
        Transform ziel = zielMachineCell.workstation;

        // Beginnen Sie die Coroutine, um das Produkt zu bewegen
        StartCoroutine(BewegeProdukt(ziel));
    }

    private IEnumerator BewegeProdukt(Transform ziel)
    {
        Vector3 startposition = transform.position;
        Vector3 endposition = ziel.position;
        float abstand = Vector3.Distance(startposition, endposition);

        // Die Geschwindigkeit wird bereits in Metern pro Sekunde angegeben, wie zuvor berechnet (0.167 m/s).
        float dauer = abstand / bewegungsgeschwindigkeit; // Berechnung der Dauer aus Abstand und Geschwindigkeit
        float verstricheneZeit = 0f;

        while (verstricheneZeit < dauer)
        {
            verstricheneZeit += Time.deltaTime;
            float progress = verstricheneZeit / dauer;
            transform.position = Vector3.Lerp(startposition, endposition, progress);
            yield return null;
        }

        transform.position = endposition; // Abschließende Positionierung zur Sicherheit
        transform.parent = ziel; // Setzen Sie das Produkt als Kind des Zieltransforms
    }




    private IEnumerator WarteUndBewegeProdukt(MachineCell naechsteMachine)
    {
        // Hier keine Wartezeit, da die Bearbeitung bereits abgeschlossen ist
        yield return null; // Dies sorgt dafür, dass die Bewegung erst im nächsten Frame beginnt
        SetMachineCell(naechsteMachine);
        BewegeZurStation(naechsteMachine);
    }

    private void BearbeitungBeendet()
    {
        Debug.Log($"Produkt {ID} hat Bearbeitung in MachineCell {currentMachineCell.MachineID} beendet.");

        aktuellerSchritt++;
        if (aktuellerSchritt < Arbeitsplan.Length)
        {
            var naechsteMachine = FindMachineCellByID(Arbeitsplan[aktuellerSchritt]);
            // Warten Sie, bis die aktuelle Bearbeitung vollständig abgeschlossen ist
            StartCoroutine(WarteUndBewegeProdukt(naechsteMachine));
        }
        else
        {
            Debug.Log($"Produktionsprozess für Produkt {ID} abgeschlossen.");
            // Setzen Sie hier den Status entsprechend, dass die Bearbeitung abgeschlossen ist
            aktuellerStatus = ProduktStatus.InMaschinenzelle; // oder einen anderen Status, wenn nötig
        }
    }

    private MachineCell FindMachineCellByID(int id)
    {
        // Implementieren Sie die Logik, um die MachineCell anhand ihrer ID zu finden
        return FindObjectsOfType<MachineCell>().FirstOrDefault(m => m.MachineID == id);
    }
}
