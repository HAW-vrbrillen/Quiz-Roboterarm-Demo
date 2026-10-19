using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class animalEatBehavior1 : MonoBehaviour
{
    public GameObject foodParent; // Referenz auf das Elternobjekt der Karotten
    public float consumeDuration = 2.0f; // Dauer des Verzehrens
    public float respawnDelay = 5.0f; // Verzögerung bis zum Wiederauftauchen
    public Animator animator;
    public AudioSource audioSource;
    public AudioClip eatingSound;
    public AudioClip thankYouSound;

    private Dictionary<GameObject, Vector3> originalFoodPositions = new Dictionary<GameObject, Vector3>();
    private Dictionary<GameObject, Vector3> originalFoodScales = new Dictionary<GameObject, Vector3>(); // Für die ursprüngliche Skalierung
    private Dictionary<GameObject, Quaternion> originalFoodRotations = new Dictionary<GameObject, Quaternion>(); // Für die ursprüngliche Rotation
    private string parentNameSubstring; // Die ersten 6 Zeichen des Namens des Parent-Objekts

    private void Start()
    {
        parentNameSubstring = foodParent.name.Substring(0, Mathf.Min(6, foodParent.name.Length));

        foreach (Transform food in foodParent.transform)
        {
            if (food.gameObject.name.StartsWith(parentNameSubstring))
            {
                originalFoodPositions.Add(food.gameObject, food.position);
                originalFoodScales.Add(food.gameObject, food.localScale); // Skalierung speichern
                originalFoodRotations.Add(food.gameObject, food.rotation); // Rotation speichern
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("rabbit trigger : " + other.gameObject.name);
        if (other.gameObject.name.StartsWith(parentNameSubstring))
        {
            Debug.Log("trigger coroutine wird gleich aufgerufen mit " + parentNameSubstring);
            StartCoroutine(Consumefood(other.gameObject));
        }
    }

    private IEnumerator Consumefood(GameObject food)
    {
        food.transform.parent = transform;

        // Drehen des Nahrungsmittels um 180 Grad um die Y-Achse direkt nach dem Parenting
        food.transform.localRotation = Quaternion.Euler(food.transform.localRotation.eulerAngles.x, 
            food.transform.localRotation.eulerAngles.y + 180, food.transform.localRotation.eulerAngles.z);

        animator.SetTrigger("eat");
        if (eatingSound != null)
        {
            audioSource.PlayOneShot(eatingSound);
        }

        Vector3 originalScale = food.transform.localScale;
        float elapsedTime = 0;

        while (elapsedTime < (eatingSound != null ? eatingSound.length : consumeDuration))
        {
            elapsedTime += Time.deltaTime;
            float scale = 1 - (elapsedTime / (eatingSound != null ? eatingSound.length : consumeDuration));
            food.transform.localScale = originalScale * scale;

            yield return null;
        }

        food.transform.parent = null;
        food.SetActive(false);

        if (thankYouSound != null)
        {
            audioSource.PlayOneShot(thankYouSound);
            yield return new WaitForSeconds(thankYouSound.length + respawnDelay);
        }
        else
        {
            yield return new WaitForSeconds(respawnDelay);
        }

        if (originalFoodPositions.ContainsKey(food))
        {
            food.transform.position = originalFoodPositions[food];
            food.transform.localScale = originalFoodScales[food]; // Ursprüngliche Skalierung wiederherstellen
            food.transform.rotation = originalFoodRotations[food]; // Ursprüngliche Rotation wiederherstellen
            food.SetActive(true);
        }
    }
}
