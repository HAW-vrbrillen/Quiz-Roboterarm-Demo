using UnityEngine;

public class SnapActionHandlerMy : MonoBehaviour
{
    public Material wood;
    public Material sand;
    
    // Diese Methode wird aufgerufen, wenn ein anderes Collider-Objekt eintritt.
    private void OnTriggerEnter(Collider other)
    {
        // Überprüfen Sie, ob das Objekt ein potenzieller Interactor ist
        if (other.CompareTag("Snappable")) // Ersetzen Sie "Interactor" mit dem tatsächlichen Tag Ihres Interactors
        {
            // Führen Sie Ihre Snap-Logik hier aus
            GetComponent<Renderer>().material = sand;
            SnapObject(other.gameObject);
        }
    }

    private void SnapObject(GameObject interactor)
    {
        
        Renderer renderer = GetComponent<MeshRenderer>();
        if (renderer != null)
        {
            // Ändern Sie hier die Farbe nach Ihren Wünschen
            renderer.material.color = Color.red; // Beispiel: Ändern zu Rot
        }
        Animator animator = GetComponent<Animator>();
        if (animator != null)
        {
            // Starten Sie hier die gewünschte Animation
            animator.SetTrigger("StartAnimation"); // Ersetzen Sie 'StartAnimation' mit dem Namen Ihres Animationstriggers
        }

        // Hier könnten Sie die Position und Ausrichtung anpassen, Farben ändern, Animationen starten, usw.
    }
}
