using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RabbitBehavior : MonoBehaviour
{
    public GameObject carrotsParent; // Referenz auf das Elternobjekt der Karotten
    public float consumeDuration = 2.0f; // Dauer des Verzehrens
    public float respawnDelay = 5.0f; // Verzögerung bis zum Wiederauftauchen
    public Animator animator;
    public AudioSource audioSource;
    public AudioClip eatingSound;
    public AudioClip thankYouSound;

    private Dictionary<GameObject, Vector3> originalCarrotPositions = new Dictionary<GameObject, Vector3>();
    private Dictionary<GameObject, Vector3> originalCarrotScales = new Dictionary<GameObject, Vector3>(); // Für die ursprüngliche Skalierung
    private string parentNameSubstring; // Die ersten 6 Zeichen des Namens des Parent-Objekts

    private void Start()
    {
        // Speichert die ersten 6 Zeichen des Namens des Parent-Objekts
        parentNameSubstring = carrotsParent.name.Substring(0, Mathf.Min(6, carrotsParent.name.Length));

        // Speichert die Ursprungspositionen und Skalierungen aller Karotten, basierend auf den ersten 6 Zeichen des Parent-Namens
        foreach (Transform carrot in carrotsParent.transform)
        {
            if (carrot.gameObject.name.StartsWith(parentNameSubstring))
            {
                originalCarrotPositions.Add(carrot.gameObject, carrot.position);
                originalCarrotScales.Add(carrot.gameObject, carrot.localScale); // Skalierung speichern
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("trigger erfolgt " + other.gameObject.name);
        // Überprüft, ob der Name des GameObjects mit den ersten 6 Zeichen des Parent-Objekts übereinstimmt
        if (other.gameObject.name.StartsWith(parentNameSubstring))
        {
            StartCoroutine(ConsumeCarrot(other.gameObject));
        }
    }

    private IEnumerator ConsumeCarrot(GameObject carrot)
    {
        carrot.transform.parent = transform;

        // Auslösen der Ess-Animation und Abspielen des Essens-Sounds, falls definiert
        animator.SetTrigger("eat");
        if (eatingSound != null)
        {
            audioSource.PlayOneShot(eatingSound);
        }

        Vector3 originalScale = carrot.transform.localScale;
        float elapsedTime = 0;

        // Über die Zeit des Essens kleiner werden lassen

        while (elapsedTime < (eatingSound != null ? eatingSound.length : consumeDuration))
        {
            elapsedTime += Time.deltaTime;
            float scale = 1 - (elapsedTime / (eatingSound != null ? eatingSound.length : consumeDuration));
            carrot.transform.localScale = originalScale * scale;
            yield return null;
        }

        // Karotte vollständig unsichtbar machen
        carrot.SetActive(false);

        // Abspielen des Dankes-Sounds, falls definiert
        if (thankYouSound != null)
        {
            audioSource.PlayOneShot(thankYouSound);
            yield return new WaitForSeconds(thankYouSound.length + respawnDelay);
        }
        else
        {
            // Warten auf das Ende der Verzögerung, auch wenn kein ThankYouSound definiert ist
            yield return new WaitForSeconds(respawnDelay);
        }

        // Überprüfen, ob die Karotte im Dictionary ist, und sie dann zurücksetzen
        if (originalCarrotPositions.ContainsKey(carrot))
        {
            carrot.transform.position = originalCarrotPositions[carrot];
            carrot.transform.localScale = originalCarrotScales[carrot]; // Ursprüngliche Skalierung wiederherstellen
            carrot.SetActive(true);
        }
    }
}
