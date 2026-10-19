using System.Collections;
using UnityEngine;

public class CarrotBehavior : MonoBehaviour
{
    private Vector3 originalPosition;
    private Vector3 originalScale;
    private Color originalColor;
    private Renderer carrotRenderer;
    public float consumeDuration = 2.0f; // Dauer des Verzehrens
    public float respawnDelay = 4.0f; // Verzögerung bis zum Wiederauftauchen
    public Animator animatorRabbit;

    private void Start()
    {
        // Ursprüngliche Werte speichern
        originalPosition = transform.position;
        originalScale = transform.localScale;
        carrotRenderer = GetComponent<Renderer>();
        if (carrotRenderer != null)
        {
            originalColor = carrotRenderer.material.color;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Trigger nur für das GameObject mit dem richtigen Namen auslösen
        if (other.gameObject.name.Contains("rabbit"))
        {
            StartCoroutine(ConsumeAndRespawnCarrot());
        }
    }

    private IEnumerator ConsumeAndRespawnCarrot()
    {
        // Karotte verschwinden lassen
        yield return StartCoroutine(ChangeCarrotState(Vector3.zero, 0f, consumeDuration));

        // Karotte für eine gewisse Zeit deaktivieren
        gameObject.SetActive(false);
        yield return new WaitForSeconds(respawnDelay);

        // Karotte zurück an den Ursprungsort setzen und wieder aktivieren
        transform.position = originalPosition;
        gameObject.SetActive(true);

        // Karotte wieder erscheinen lassen
        yield return StartCoroutine(ChangeCarrotState(originalScale, 1f, consumeDuration));
    }

    private IEnumerator ChangeCarrotState(Vector3 targetScale, float targetAlpha, float duration)
    {
        float time = 0;
        animatorRabbit.SetTrigger("eat");
        while (time < duration)
        {
            time += Time.deltaTime;
            float fraction = time / duration;

            // Skalierung interpolieren
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, fraction);

            // Transparenz interpolieren, wenn ein Renderer vorhanden ist
            if (carrotRenderer != null)
            {
                Color color = carrotRenderer.material.color;
                color.a = Mathf.Lerp(color.a, targetAlpha, fraction);
                carrotRenderer.material.color = color;
            }

            yield return null;
        }
    }
}
