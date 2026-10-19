using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ColorChangerRIS : MonoBehaviour
{
    public Slider slider; // Der Slider
    public Renderer rend; // Das Objekt, dessen Farbe geändert wird
    public TMP_Text slidertext;

    void Start()
    {
        // Initialisieren Sie den Slider mit Werten zwischen 0 und 1
        slider.minValue = 0;
        slider.maxValue = 1;
        slider.value = 0;

        // Setzen Sie die Anfangsfarbe des Objekts auf Blau
        rend.material.color = Color.blue;
    }

    void Update()
    {
        // Ändern Sie die Farbe des Objekts basierend auf dem Wert des Sliders
        rend.material.color = Color.Lerp(Color.blue, Color.red, slider.value);

        // Zeigen Sie den aktuellen Wert des Sliders als Zahl an
        Debug.Log("Slider-Wert: " + slider.value);
        slidertext.text="Unsere Temperatur ist: " + slider.value*15 + " Grad Celsius";
    }
}
