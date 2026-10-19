using UnityEngine;

public class InteraktionController : MonoBehaviour
{
    // Ziehen Sie hier Ihr AudioClip im Inspector hin
    public AudioClip moechtenSieErleauterungClip;

    // Ziehen Sie hier Ihr PokeButton GameObject im Inspector hin
   // public GameObject pokeButton;

    private AudioSource audioSource;

    void Start()
    {
        // AudioSource-Komponente hinzufuegen, wenn nicht vorhanden
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = moechtenSieErleauterungClip;

        // Stellen Sie sicher, dass der PokeButton zu Beginn deaktiviert ist
        //pokeButton.SetActive(false);
    }

    // Wird aufgerufen, wenn ein Objekt den Trigger betritt
    void OnTriggerEnter(Collider other)
    {
        // ueberpruefen, ob das eintretende Objekt das richtige ist
        if (other.CompareTag("Player")) // Stellen Sie sicher, dass das Tag korrekt gesetzt ist
        {
            audioSource.Play();
            //pokeButton.SetActive(true);
        }
    }

    // Wird aufgerufen, wenn ein Objekt den Trigger verl�sst
    void OnTriggerExit(Collider other)
    {
        // �berpr�fen, ob das austretende Objekt das richtige ist
        if (other.CompareTag("Objekt1")) // Stellen Sie sicher, dass das Tag korrekt gesetzt ist
        {
            audioSource.Stop();
            //pokeButton.SetActive(false);
        }
    }
}
