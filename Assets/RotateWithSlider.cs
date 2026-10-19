using TMPro;
using UnityEngine;
using UnityEngine.UI; // Wichtig für den Zugriff auf UI-Elemente wie den Slider

public class RotateWithSlider : MonoBehaviour
{
    public Slider rotationSlider; // Referenz zum Slider
    public GameObject objectToRotate; // Das Objekt, das rotiert werden soll
    public TMP_Text text;

    

    public void OnSliderValueChanged()
    {
        // Setze die neue Rotation basierend auf dem Slider-Wert
        float zRotation = rotationSlider.value;
        text.text = "Aktueller Wert: " + zRotation;
        objectToRotate.transform.rotation = Quaternion.Euler(0, 0, zRotation);
    }
}
