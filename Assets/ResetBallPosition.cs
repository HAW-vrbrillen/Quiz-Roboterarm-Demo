using System.Collections;
using UnityEngine;

public class ResetBallPosition : MonoBehaviour
{
    public Transform player;  // Der Transform des Spielers
    public float resetDistance = 1.2f;  // Entfernungsschwelle f�r das Zur�cksetzen
    private Vector3 initialPosition;  // Ursprungsort der Kugel
    public AudioSource as1;
    public AudioClip aclip1;
    private bool isResetting = false; // Flag, um sicherzustellen, dass die Coroutine nicht mehrfach gestartet wird

    void Start()
    {
        // Speichere den Startpunkt der Kugel
        initialPosition = transform.position;
    }

    void Update()
    {
        // Berechne die Entfernung zwischen der Kugel und dem Spieler
        float distance = Vector3.Distance(transform.position, player.position);

        // Wenn die Entfernung gr��er als 1,2 Meter ist und keine R�cksetzung l�uft, starte die Coroutine
        if (distance > resetDistance && !isResetting)
        {
            StartCoroutine(ResetPositionWithDelay());
        }
    }

    private IEnumerator ResetPositionWithDelay()
    {
        isResetting = true; // R�cksetzvorgang gestartet

        // Warte 2 Sekunden bevor die Position zur�ckgesetzt wird
        yield return new WaitForSeconds(2f);

        transform.position = initialPosition;

        // Optional: Setze auch die Geschwindigkeit der Kugel zur�ck, falls sie physikalisch bewegt wird
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // Optional: Spiele einen Sound ab
        // as1.PlayOneShot(aclip1);

        // Kommentar: Position der Kugel zur�ckgesetzt, da sie zu weit vom Spieler entfernt war. 202408131310
        isResetting = false; // R�cksetzvorgang beendet
    }
}
