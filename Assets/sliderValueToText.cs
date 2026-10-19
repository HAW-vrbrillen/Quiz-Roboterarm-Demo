using UnityEngine;
using UnityEngine.UI;

public class sliderValueToText : MonoBehaviour
{
    private Text label;
    private Slider slider;

    // Start is called before the first frame update
    void Start()
    {
        label = GetComponentInChildren<Text>();
        slider = GetComponent<Slider>();

        // Stellen Sie sicher, dass der Label-Text beim Start aktualisiert wird.
        setTextValue(slider.value);

        // Fügen Sie den Event-Listener hinzu, falls er nicht über den Editor hinzugefügt wurde.
        slider.onValueChanged.AddListener((value) => setTextValue(value));
    }

    // Diese Methode sollte jetzt einen Parameter akzeptieren, um direkt mit dem Event Listener zu arbeiten.
    public void setTextValue(float value)
    {
        label.text = value.ToString("0.##"); // Formatieren Sie die Zahl nach Bedarf.
    }
}
