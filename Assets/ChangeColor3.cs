using UnityEngine;

public class ChangeColor : MonoBehaviour
{
    public Color color1;
    public Color color2;

    public void ChangeToColor() // Diese Methode wird durch das Animation Event aufgerufen
    {
        if (GetComponent<Renderer>().material.color == color1)
        {
            GetComponent<Renderer>().material.color = color2; // Ändert die Farbe zu Rot
        }
        else
            GetComponent<Renderer>().material.color = color1;

    }
}
