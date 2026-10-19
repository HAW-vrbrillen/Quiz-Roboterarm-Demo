using System.Collections;
using UnityEngine;
using TMPro;

public class BoatController : MonoBehaviour
{
    public GameObject confettiEffectPrefab; // Weisen Sie Ihr Konfetti-Partikelsystem zu
    public TMP_Text thankYouText; // Weisen Sie Ihr Text-Objekt zu, das "Danke für Ihren Beitrag zur Ozeanreinigung" sagt
    public TMP_Text aktNum;
    public ParticleSystem particelSystem;
    public AudioSource audioSource1;


    public float boatSpeed = 5f; // Geschwindigkeit, mit der das Boot weg fahren soll

    public int objectsInBoat = 0;
    
    private void Start()
    {
        //confettiEffectPrefab.SetActive(false);
        //thankYouText.enabled = false;
        particelSystem.Stop();

    }

    private void OnTriggerEnter(Collider other)
    {
        // Überprüfen, ob das kollidierte Objekt ein sammelbares Objekt ist
        if (other.CompareTag("Collectible"))
        {
            objectsInBoat++; // Erhöht die Anzahl der Objekte im Boot
            aktNum.text = "Aktuell: " + objectsInBoat;
            //Destroy(other.gameObject); // Optional: Objekt zerstören oder deaktivieren

            // Prüfen, ob die erforderliche Anzahl von Objekten erreicht ist
            if (objectsInBoat == 3)
            {
                // Konfetti-Effekt erzeugen
                particelSystem.Play();
                audioSource1.Play();


                // Danke-Schrift aktivieren
                thankYouText.enabled= true;

                // Boot wegfahren lassen
                //StartCoroutine(MoveBoatAway());
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        // Überprüfen, ob das kollidierte Objekt ein sammelbares Objekt ist
        if (other.CompareTag("Collectible"))
        {
            objectsInBoat--; // Erniedrigt die Anzahl der Objekte im Boot
            aktNum.text = "Aktuell: " + objectsInBoat;
            //Destroy(other.gameObject); // Optional: Objekt zerstören oder deaktivieren

            // Prüfen, ob die erforderliche Anzahl von Objekten erreicht ist
            if (objectsInBoat < 3)
            {
                // Danke-Schrift aktivieren
                thankYouText.enabled = false;
                audioSource1.Stop();
                particelSystem.Stop();

            }
        }
    }

    private IEnumerator MoveBoatAway()
    {
        Vector3 startPosition = transform.position;
        Vector3 endPosition = startPosition + new Vector3(0, 0, 50); // Passen Sie dies an Ihre Szene an

        float timeToMove = 5f; // Zeit in Sekunden, die das Boot zum Wegfahren benötigt
        float elapsedTime = 0;

        while (elapsedTime < timeToMove)
        {
            transform.position = Vector3.Lerp(startPosition, endPosition, (elapsedTime / timeToMove));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Deaktivieren Sie hier das Boot oder führen Sie weitere Aktionen durch
    }
}

