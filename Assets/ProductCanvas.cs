using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ProductCanvas : MonoBehaviour
{
    public List<Toggle> machineToggles;
    public List<string> selectedMachines = new List<string>();

    void Start()
    {
        foreach (var toggle in machineToggles)
        {
            toggle.onValueChanged.AddListener(delegate { UpdateMachineSelection(); });
        }
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
}
