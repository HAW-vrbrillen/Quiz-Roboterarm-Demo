using UnityEngine;

public class BewegeObjekt : MonoBehaviour
{
    public float geschwindigkeit = 5.0f; // Geschwindigkeit, mit der sich das Objekt bewegt

    void Update()
    {
        // Eingaben lesen
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // Bewegung basierend auf Eingaben berechnen
        Vector3 bewegung = new Vector3(horizontal, 0, vertical) * geschwindigkeit * Time.deltaTime;

        // Objekt bewegen
        transform.Translate(bewegung);
    }
}
