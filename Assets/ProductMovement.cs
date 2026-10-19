using UnityEngine;

public class ProductMovement : MonoBehaviour
{
    public Transform currentTarget;
    public float speed = 1f;
    private ProductController productController;

    void Start()
    {
        productController = GetComponent<ProductController>();
        MoveToNextMachine();
    }

    private void Update()
    {
        if (currentTarget != null)
        {
            MoveTowardsTarget();
        }
    }

    public void MoveTowardsTarget()
    {
        if (Vector3.Distance(transform.position, currentTarget.position) < 0.1f)
        {
            ArrivedAtMachine();
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, currentTarget.position, speed * Time.deltaTime);
        }
    }

    public void ArrivedAtMachine()
    {
        // Logik für das Warten und Bearbeiten an der Maschine
        MoveToNextMachine();
    }

    public void MoveToNextMachine()
    {
        string nextMachineName = productController.GetNextMachine();

        if (nextMachineName != null)
        {
            GameObject nextMachine = GameObject.Find(nextMachineName);
            if (nextMachine != null)
            {
                currentTarget = nextMachine.transform;
            }
            else
            {
                Debug.Log("keine maschine gefunden");
            }
        }
    }
}
