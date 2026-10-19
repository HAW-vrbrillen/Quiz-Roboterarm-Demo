using UnityEngine;
using System.Linq;

public class controlProduction : MonoBehaviour
{
    private Produkt[] alleProdukte;
    private GameObject produkte;
    public bool startProduction1 = false;

    void Start()
    {
        // Hiermit werden alle Produkt-Instanzen, die Kinder des GameObjects sind, auf das dieses Skript angewendet ist, gesammelt.
        produkte = GameObject.Find("produkte");
        alleProdukte = produkte.GetComponentsInChildren<Produkt>();
        if (alleProdukte == null) { Debug.Log("keine produkte gefunden"); return; }


        // Optional: Starte die Produktion sofort nach dem Finden aller Produkte
        if (startProduction1)
        {
            StarteProduktionFuerAlleProdukte();
        }
    }

    public void ResetAlleProdukte()
    {
        foreach (var produkt in alleProdukte)
        {
            //produkt.ResetProdukt();
        }
    }


    public void AktualisiereUIUndStarteProduktion()
    {
        foreach (var produkt in alleProdukte)
        {
            // Aktualisiere die UI für jedes Produkt
            //produkt.SetArbeitsplanFromUI();
        }
        Debug.Log("controlProduction: alle arbeitspläne initialisiert");
        // Optional: Starte die Produktion nach der Aktualisierung der UI
        Debug.Log("StarteProduktionFuerAlleProdukte()");
        StarteProduktionFuerAlleProdukte();
    }

    private void StarteProduktionFuerAlleProdukte()
    {
        foreach (var produkt in alleProdukte)
        {
            // Starte die Produktion für jedes Produkt
            Debug.Log("controlProduction: product.InitiateProduction: " + produkt.name);
            //produkt.InitiateProduction();
        }
    }
}
