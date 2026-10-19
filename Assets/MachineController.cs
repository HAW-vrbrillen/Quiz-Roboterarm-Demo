using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MachineController : MonoBehaviour
{
    public float processingTime = 5f; // Bearbeitungszeit pro Produkt
    private Queue<GameObject> productQueue = new Queue<GameObject>();

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Product"))
        {
            productQueue.Enqueue(other.gameObject);
            other.GetComponent<ProductMovement>().currentTarget = null; // Stoppt die Bewegung
            if (productQueue.Count == 1) // Wenn das Produkt das erste in der Warteschlange ist
            {
                StartCoroutine(ProcessProduct());
            }
        }
    }

    IEnumerator ProcessProduct()
    {
        yield return new WaitForSeconds(processingTime); // Warte für die Bearbeitungszeit
        GameObject processedProduct = productQueue.Dequeue();
        processedProduct.GetComponent<ProductMovement>().MoveToNextMachine(); // Sendet Produkt zur nächsten Maschine

        if (productQueue.Count > 0) // Wenn es weitere Produkte in der Warteschlange gibt
        {
            StartCoroutine(ProcessProduct()); // Startet die Bearbeitung des nächsten Produkts
        }
    }
}
