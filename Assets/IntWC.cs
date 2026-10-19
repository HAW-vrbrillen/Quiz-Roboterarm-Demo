using UnityEngine;
using System.Collections.Generic;

public class IntWC : MonoBehaviour
{
    public int workcellID; // Öffentliche Variable für die ID
    public Queue<IntProdukt> inputQueue;
    private IntProdukt currentProduct;

    void Start()
    {
        inputQueue = new Queue<IntProdukt>();
    }

    public void ReceiveProduct(IntProdukt produkt, float bearbeitungszeit)
    {
        inputQueue.Enqueue(produkt);
        // Optional: Starten der Bearbeitung oder Warten auf den Prozess
    }

    void Update()
    {
        // Implementieren Sie hier die Logik für die Bearbeitung von Produkten
    }

    // Weitere Methoden...
}
