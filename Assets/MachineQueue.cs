using System.Collections.Generic;
using UnityEngine;

public class MachineQueue : MonoBehaviour
{
    public Queue<GameObject> productQueue = new Queue<GameObject>();

    public void EnqueueProduct(GameObject product)
    {
        productQueue.Enqueue(product);
    }

    public GameObject DequeueProduct()
    {
        return productQueue.Count > 0 ? productQueue.Dequeue() : null;
    }
}
