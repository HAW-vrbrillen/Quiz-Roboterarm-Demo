/*
 * public class ProduktionsSteuerung : MonoBehaviour
 * public class ProduktNeu : MonoBehaviour
 * public class MaschinenZelle : MonoBehaviour
 * public class ArbeitsSchrittNeu
 * public class MaschinenZelle : MonoBehaviour

 Hier ist der typische Ablauf der Funktionen in der Reihenfolge, in der sie normalerweise aufgerufen werden:

### Typischer Ablauf der Produktion

1. **`ProduktionsSteuerung.Start()`**
   - Wird beim Starten des Spiels aufgerufen.
   - Sucht alle `ProduktNeu`-Objekte unter "Produkte" und speichert sie in `produkte`.

2. **`ProduktionsSteuerung.UIAktualisierenUndProduktionStarten()`**
   - Wird durch eine UI-Aktion (z.B. Knopfdruck) ausgelöst.
   - Für jedes Produkt in `produkte`:
     - **`ProduktNeu.SetArbeitsplanVonUI()`**
       - Liest Daten aus der UI und erstellt den `arbeitsPlan`.
     - **`ProduktNeu.ProduktionStarten()`**
       - Startet die Produktion, wenn der `arbeitsPlan` nicht leer ist.
       - **`ProduktNeu.NaechstenSchrittStarten()`**
         - Findet die passende `MaschinenZelle` für den aktuellen Arbeitsschritt.
         - **`ProduktNeu.FindeMaschinenZelleNachID(string machineID)`**
           - Sucht die `MaschinenZelle` mit der gegebenen `machineID` und gibt sie zurück.
         - **`MaschinenZelle.ZurWarteschlangeHinzufuegen(ProduktNeu produkt)`**
           - Fügt das Produkt zur Warteschlange der Maschinenzelle hinzu.

3. **`MaschinenZelle.Update()`**
   - Wird in jedem Frame aufgerufen.
   - Überprüft, ob die Warteschlange nicht leer ist, und startet den Produktionsprozess.
   - **`MaschinenZelle.ProduktionsprozessStarten()`**
     - Entnimmt das erste Produkt aus der Warteschlange.
     - **`MaschinenZelle.BewegeProduktZurArbeitsstation(ProduktNeu produkt)`**
       - Startet die Coroutine, die das Produkt zur Arbeitsstation bewegt.
       - Wartet die Transportzeit des aktuellen Arbeitsschritts.
       - **`ProduktNeu.SetzeMaschinenZelle(MaschinenZelle maschinenZelle)`**
         - Setzt die aktuelle Maschinenzelle und startet die Bearbeitung.
         - **`ProduktNeu.StartBearbeitung()`**
           - Startet die Coroutine für die Bearbeitung.
           - **`ProduktNeu.BearbeitungProzess()`**
             - Wartet die Bearbeitungszeit des aktuellen Arbeitsschritts.
             - **`ProduktNeu.BearbeitungAbgeschlossen()`**
               - Erhöht den `aktuellerSchrittIndex`.
               - Prüft, ob noch Arbeitsschritte übrig sind.
               - Wenn ja, ruft `NaechstenSchrittStarten` erneut auf.
               - Wenn nein, meldet die Fertigstellung des Produkts.

### Zusammenfassung

- Das Spiel startet und initialisiert alle `ProduktNeu`-Objekte.
- Eine UI-Aktion startet den Produktionsprozess für jedes Produkt.
- Jedes Produkt durchläuft seine Arbeitsschritte, indem es von Maschinenzelle zu Maschinenzelle bewegt wird.
- Nach jedem Arbeitsschritt wird die Bearbeitung abgeschlossen und der nächste Schritt gestartet.
- Der Prozess wiederholt sich, bis alle Arbeitsschritte abgeschlossen sind und das Produkt fertiggestellt ist.
 
 
 * Hier ist die Aufrufhierarchie des gegebenen Codes mit kurzen Kommentaren:

### `ProduktionsSteuerung`:
- **`Start()`**
  - Sucht alle `ProduktNeu`-Objekte unter "Produkte" und speichert sie in `produkte`.

- **`AlleProdukteZuruecksetzen()`**
  - Ruft `ResetProduktNeu` für jedes Produkt in `produkte` auf.

- **`UIAktualisierenUndProduktionStarten()`**
  - Ruft `SetArbeitsplanVonUI` und `ProduktionStarten` für jedes Produkt in `produkte` auf.

### `ProduktNeu`:
- **`ResetProduktNeu()`**
  - Setzt `aktuellerSchrittIndex` auf 0 und `aktuelleMaschinenZelle` auf null.

- **`SetArbeitsplanVonUI()`**
  - Liest Daten aus der UI und erstellt den `arbeitsPlan`.

- **`ProduktionStarten()`**
  - Startet die Produktion, wenn der `arbeitsPlan` nicht leer ist, durch Aufruf von `NaechstenSchrittStarten`.

- **`NaechstenSchrittStarten()`**
  - Findet die passende `MaschinenZelle` für den aktuellen Arbeitsschritt und fügt das Produkt zur Warteschlange der Maschinenzelle hinzu.

- **`BearbeitungAbgeschlossen()`**
  - Erhöht `aktuellerSchrittIndex` und startet den nächsten Schritt oder beendet die Produktion, wenn alle Schritte abgeschlossen sind.

- **`FindeMaschinenZelleNachID(string machineID)`**
  - Sucht die `MaschinenZelle` mit der gegebenen `machineID` und gibt sie zurück.

- **`SetzeMaschinenZelle(MaschinenZelle maschinenZelle)`**
  - Setzt die aktuelle Maschinenzelle und startet die Bearbeitung durch Aufruf von `StartBearbeitung`.

- **`StartBearbeitung()`**
  - Startet die Coroutine `BearbeitungProzess`.

- **`BearbeitungProzess()`**
  - Wartet die Bearbeitungszeit des aktuellen Arbeitsschritts und ruft `BearbeitungAbgeschlossen` auf.

### `MaschinenZelle`:
- **`ZurWarteschlangeHinzufuegen(ProduktNeu produkt)`**
  - Fügt das Produkt zur Warteschlange `inputQueue` hinzu.

- **`Update()`**
  - Überprüft, ob die Warteschlange nicht leer ist, und startet den Produktionsprozess.

- **`ProduktionsprozessStarten()`**
  - Entnimmt das erste Produkt aus der Warteschlange und startet die Coroutine `BewegeProduktZurArbeitsstation`.

- **`BewegeProduktZurArbeitsstation(ProduktNeu produkt)`**
  - Wartet die Transportzeit des aktuellen Arbeitsschritts und ruft `SetzeMaschinenZelle` des Produkts auf.
 * 
 * 
 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProduktNeu : MonoBehaviour
{
    public string produktTyp; // "Fahrwerk" oder "Chassis"
    public List<ArbeitsSchrittNeu> arbeitsPlan; // Liste der Arbeitsschritte
    public int aktuellerSchrittIndex = 0; // Machen wir öffentlich zugänglich
    private MaschinenZelle aktuelleMaschinenZelle;

    public void ResetProduktNeu()
    {
        aktuellerSchrittIndex = 0;
        aktuelleMaschinenZelle = null;
    }

    public void SetArbeitsplanVonUI()
    {
        // UI-Daten einlesen und Arbeitsplan erstellen
        // Beispiel: arbeitsPlan = new List<ArbeitsSchrittNeu> { new ArbeitsSchrittNeu("wc1", 5, 2), new ArbeitsSchrittNeu("wc2", 10, 3) };
    }

    public void ProduktionStarten()
    {
        if (arbeitsPlan.Count > 0)
        {
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
                maschinenZelle.ZurWarteschlangeHinzufuegen(this);
            }
        }
        else
        {
            BearbeitungAbgeschlossen();
        }
    }

    public void BearbeitungAbgeschlossen()
    {
        aktuellerSchrittIndex++;
        if (aktuellerSchrittIndex < arbeitsPlan.Count)
        {
            NaechstenSchrittStarten();
        }
        else
        {
            // Produkt ist fertig
            Debug.Log("Produktfertigstellung: " + this.name);
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

    // Hinzufügen der Methode SetzeMaschinenZelle
    public void SetzeMaschinenZelle(MaschinenZelle maschinenZelle)
    {
        aktuelleMaschinenZelle = maschinenZelle;
        StartBearbeitung();
    }

    // Implementierung der Methode StartBearbeitung
    private void StartBearbeitung()
    {
        StartCoroutine(BearbeitungProzess());
    }

    private IEnumerator BearbeitungProzess()
    {
        ArbeitsSchrittNeu schritt = arbeitsPlan[aktuellerSchrittIndex];
        yield return new WaitForSeconds(schritt.bearbeitungszeit);
        BearbeitungAbgeschlossen();
    }
}

[System.Serializable]
public class ArbeitsSchrittNeu
{
    public string machineID;
    public float bearbeitungszeit;
    public float transportzeit;

    public ArbeitsSchrittNeu(string id, float bearbeitungszeit, float transportzeit)
    {
        this.machineID = id;
        this.bearbeitungszeit = bearbeitungszeit;
        this.transportzeit = transportzeit;
    }
}

 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaschinenZelle : MonoBehaviour
{
    public string machineID;
    private Queue<ProduktNeu> inputQueue = new Queue<ProduktNeu>();

    public void ZurWarteschlangeHinzufuegen(ProduktNeu produkt)
    {
        inputQueue.Enqueue(produkt);
    }

    void Update()
    {
        if (inputQueue.Count > 0)
        {
            ProduktionsprozessStarten();
        }
    }

    private void ProduktionsprozessStarten()
    {
        if (inputQueue.Count > 0)
        {
            ProduktNeu produkt = inputQueue.Dequeue();
            StartCoroutine(BewegeProduktZurArbeitsstation(produkt));
        }
    }

    private IEnumerator BewegeProduktZurArbeitsstation(ProduktNeu produkt)
    {
        yield return new WaitForSeconds(produkt.arbeitsPlan[produkt.aktuellerSchrittIndex].transportzeit);
        produkt.SetzeMaschinenZelle(this); // Methode korrekt aufrufen
    }
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProduktionsSteuerung : MonoBehaviour
{
    private ProduktNeu[] produkte;

    void Start()
    {
        produkte = GameObject.Find("Produkte").GetComponentsInChildren<ProduktNeu>();
    }

    public void AlleProdukteZuruecksetzen()
    {
        foreach (var produkt in produkte)
        {
            produkt.ResetProduktNeu();
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
}

 * 
 * 
 * 
 */

