using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class readme : MonoBehaviour
{
    /*
     * todo
     * 1. 
     * 
     * was montiert wird
     * starte mit Blech und blende dann erst ein ... kompliziert du musst auch ausblenden
     * 
     * bei typ Fahrwerk sind einige Vorgänge länger als bei Typ Chassis
     * 2 und 3 laufe in unterschiedlicher Reihenfolge bei Fahrwerk udn Chassis
    wc0 Fahrwerksmontage ... hier nur typ1
    wc1 Engine beide
    wc2 Vorderwagen 
    wc3 Hinterwagen und Animation
    
    
    
    
    Aufrufreihenfolge:
    Die Aufrufreihenfolge der Funktionen in Ihrem Unity-Skript mit den wesentlichen Parametern lässt sich wie folgt zusammenfassen:

   1. controlProduction.Start()
   Ruft GameObject.Find("produkte") auf, um das GameObject zu finden, das alle Produkt-Instanzen enthält.
   Verwendet produkte.GetComponentsInChildren<Produkt>(), um alle Produkt-Instanzen zu sammeln.
   Optional kann hier die Produktion für alle Produkte gestartet werden (nicht im Code dargestellt).

   2. controlProduction.ResetAlleProdukte()
   Durchläuft alle Produkt-Instanzen und ruft für jede produkt.ResetProdukt() auf.
   
   3. controlProduction.AktualisiereUIUndStarteProduktion()
   Aktualisiert die UI für jedes Produkt mit produkt.SetArbeitsplanFromUI().
   Startet anschließend die Produktion für jedes Produkt mit produkt.InitiateProduction().
   
    4. Produkt.SetArbeitsplanFromUI()
   Erstellt einen Arbeitsplan basierend auf den UI-Einstellungen für das jeweilige Produkt.
   
    5. Produkt.InitiateProduction()
   Setzt den initialen Status und prüft den ersten Schritt im Arbeitsplan.
   Findet die entsprechende MachineCell mit FindMachineCellByID() und fügt das Produkt zur Warteschlange dieser Zelle hinzu.
   
    6. MachineCell.Update()
   Überprüft regelmäßig, ob Produkte in der Input-Queue vorhanden sind und startet den Produktionsprozess für das nächste Produkt in der Queue.
   
    7. MachineCell.StarteProduktionsprozess()
   Entnimmt das nächste Produkt aus der Input-Queue und bewegt es zur Workstation mit StartCoroutine(BewegeProduktZurWorkstation(produkt)).
   
    8. MachineCell.BewegeProduktZurWorkstation(Produkt produkt)
   Bewegt das Produkt zur Workstation und setzt es dort ein mit produkt.SetMachineCell(this).
   
    9. Produkt.SetMachineCell(MachineCell machineCell)
   Setzt die aktuelle MachineCell für das Produkt und startet die Bearbeitung mit StartBearbeitung().
   
    10. Produkt.Update()
   Wird kontinuierlich aufgerufen, um den Bearbeitungsfortschritt zu aktualisieren.
   Beendet die Bearbeitung, wenn der Fertigstellungsgrad 1 erreicht.
   
    11. Produkt.BearbeitungBeendet()
   Wird aufgerufen, wenn die Bearbeitung eines Schritts abgeschlossen ist.
   Überprüft, ob weitere Schritte im Arbeitsplan vorhanden sind und bewegt das Produkt gegebenenfalls zur nächsten MachineCell.
   Diese Reihenfolge skizziert den grundlegenden Ablauf der Produktionssteuerung in Ihrem Skript, beginnend mit der Initialisierung und dem Setzen der Arbeitspläne bis hin zum Durchlauf der einzelnen Bearbeitungsschritte.

    

    
    */

}
