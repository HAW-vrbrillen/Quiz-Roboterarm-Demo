using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MachineCell : MonoBehaviour
{
    public int MachineID;
    public Transform workstation;
    public Transform outputPosition;
    public Transform inputQueueTransform;
    //public Produkt[] startProdukte;
    private Queue<Produkt> inputQueue = new Queue<Produkt>();

    public List<Produkt> inputQueueList; // Liste für die Input Queue
    public Produkt aktuellesProduktInWorkstation; // Aktuelles Produkt in der Workstation

    //public int maxProdukte = 10; // Maximale Anzahl der Produkte, die gehalten werden können
    public float produktwidth = 0.02f;

    private void Start()
    {

    }

    private void Update()
    {
        if (workstation.childCount == 0 && inputQueue.Count > 0)
        {
            StarteProduktionsprozess();
        }

        inputQueueList = inputQueue.ToList();
        aktuellesProduktInWorkstation = workstation.childCount > 0 ? workstation.GetChild(0).GetComponent<Produkt>() : null;
    }

    private void StarteProduktionsprozess()
    {
        Produkt produkt = inputQueue.Dequeue(); // Entfernen Sie das Produkt aus der Queue
        StartCoroutine(BewegeProduktZurWorkstation(produkt));
    }

    public void AddToInputQueue(Produkt produkt)
    {
        //if (inputQueue.Count + workstation.childCount < maxProdukte)
        //{
            inputQueue.Enqueue(produkt);
            Debug.Log($"Produkt {produkt.ID} zur Input-Queue von MachineCell {MachineID} hinzugefügt.");

            // Positioniere das Produkt links von der InputQueue-Position
            Vector3 neuePosition = inputQueueTransform.position - new Vector3(produktwidth * inputQueue.Count, 0, 0);
            produkt.transform.position = neuePosition;

            // Optional: Setzen Sie das Produkt als Kind des InputQueueTransforms
            produkt.transform.SetParent(inputQueueTransform, true);
            
        //}
        //else
        //{
        //    Debug.Log($"Maximale Kapazität erreicht. Produkt {produkt.ID} kann nicht hinzugefügt werden.");
        //}
    }

    private IEnumerator BewegeProduktZurWorkstation(Produkt produkt)
    {
        yield return new WaitForSeconds(1f);

        if (inputQueue.Count > 0 && workstation.childCount == 0)
        {
            inputQueue.Dequeue();
            produkt.transform.SetParent(workstation, false);
            produkt.transform.localPosition = Vector3.zero;
            produkt.SetMachineCell(this);
            produkt.StartBearbeitung();
            Debug.Log($"Produkt {produkt.ID} bewegt sich zur Workstation von MachineCell {MachineID}.");
        }
    }
}
