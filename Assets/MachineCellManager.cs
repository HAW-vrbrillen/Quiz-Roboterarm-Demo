using System.Linq;
using UnityEngine;

public class MachineCellManager : MonoBehaviour
{
    public MachineCell[] MachineCells;

    void Awake()
    {
        MachineCells = FindObjectsOfType<MachineCell>();
    }

    public MachineCell GetMachineByID(int id)
    {
        return MachineCells.FirstOrDefault(m => m.MachineID == id);
    }
}
