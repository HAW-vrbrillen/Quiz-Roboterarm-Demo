using UnityEngine;

public class SkalierungsManager : MonoBehaviour
{
    public GameObject[] skalierbareObjekte; // Setze dies im Inspector

    private void Start()
    {
        // Finde und stoppe alle Audioquellen in der Szene
        AudioSource[] audioQuellen = FindObjectsOfType<AudioSource>();
        foreach (var audioQuelle in audioQuellen)
        {
            audioQuelle.Stop();
        }

        // Skaliere alle Objekte runter, wenn sie nicht schon klein sind
        foreach (var obj in skalierbareObjekte)
        {
            if (obj.transform.localScale.x > 0.001f ||
                obj.transform.localScale.y > 0.001f ||
                obj.transform.localScale.z > 0.001f)
            {
                obj.transform.localScale = new Vector3(0.0001f, 0.0001f, 0.0001f);
            }
        }
    }

    public void ToggleSkalierung()
    {
        // Skaliere die Objekte hoch oder runter, basierend auf ihrem aktuellen Zustand
        foreach (var obj in skalierbareObjekte)
        {
            if (obj.transform.localScale.x < 1f) // Wenn kleiner als ursprüngliche Größe
            {
                obj.transform.localScale *= 1000f; // Skaliere hoch
            }
            else
            {
                obj.transform.localScale = new Vector3(0.0001f, 0.0001f, 0.0001f); // Skaliere runter
            }
        }
    }
}
