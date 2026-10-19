using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ProductController : MonoBehaviour
{
    public List<Toggle> machineToggles;
    public List<string> selectedMachines = new List<string>();
    public int currentMachineIndex = 0;
    public ProductMovement productMovement1;
    public bool startproduction1 = false;

    void Start()
    {
        foreach (var toggle in machineToggles)
        {
            toggle.onValueChanged.AddListener(delegate { UpdateMachineSelection(); });
            toggle.isOn = true;
        }
        if (startproduction1)
        {
            // Initialisiere die Maschinenauswahl
            UpdateMachineSelection();
            Invoke("StartProduction", 0.1f); // Verzögern Sie den Start leicht
            Debug.Log("Ausgewählte Maschinen: " + string.Join(", ", selectedMachines)); // Fügen Sie diese Zeile hinzu
            
        }


    }
    void StartProduction()
    {
        // Bewege das Produkt zur ersten Maschine
        productMovement1.MoveToNextMachine();
    }

    void UpdateMachineSelection()
    {
        selectedMachines.Clear();
        foreach (var toggle in machineToggles)
        {
            if (toggle.isOn)
            {
                selectedMachines.Add(toggle.name);
            }
        }
    }

    public string GetNextMachine()
    {
        if (currentMachineIndex < selectedMachines.Count)
        {
            return selectedMachines[currentMachineIndex++];
        }
        return null;
    }
}
