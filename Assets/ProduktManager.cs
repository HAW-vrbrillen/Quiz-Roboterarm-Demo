using UnityEngine;
using System.Linq;

public class ProduktManager : MonoBehaviour
{
    void Start()
    {
        // Überprüfen, ob alle Produkte eindeutige IDs haben
        Produkt[] alleProdukte = FindObjectsOfType<Produkt>();
        if (alleProdukte.Length != alleProdukte.Select(p => p.ID).Distinct().Count())
        {
            Debug.LogError("Produkte haben nicht eindeutige IDs! Spiel wird gestoppt.");
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        // Überprüfen, ob alle MachineCells eindeutige IDs haben
        MachineCell[] alleMachineCells = FindObjectsOfType<MachineCell>();
        if (alleMachineCells.Length != alleMachineCells.Select(m => m.MachineID).Distinct().Count())
        {
            Debug.LogError("MachineCells haben nicht eindeutige IDs! Spiel wird gestoppt.");
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
