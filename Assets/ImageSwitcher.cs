using UnityEngine;
using TMPro;

public class ImageSwitcher : MonoBehaviour
{
    public TMP_Text textdistance;
    public GameObject sharpImage; // Das GameObject mit dem scharfen Bild
    public GameObject blurryImage; // Das GameObject mit dem unscharfen Bild
    public Transform viewer; // Der Transform des Betrachters (z.B. Kamera)
    public float switchDistance = 0.3f; // Entfernung in Metern, bei der das Bild gewechselt wird

    void Update()
    {
        // Berechne die Entfernung zwischen dem Betrachter und dem Bild
        float distance = Vector3.Distance(viewer.position, blurryImage.transform.position);
        textdistance.text = distance.ToString();
        // Wenn die Entfernung kleiner als die Schwellendistanz ist, zeige das scharfe Bild, ansonsten das unscharfe Bild
        if (distance < switchDistance)
        {
            sharpImage.SetActive(true);
            blurryImage.SetActive(false);
        }
        else
        {
            sharpImage.SetActive(false);
            blurryImage.SetActive(true);
        }
    }
}
