using UnityEngine;
using TMPro;
using System.Collections; // Für den Zugriff auf TextMeshPro


public class RingFlightController : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip successClip;
    public TMP_Text successTextVari;
    private int ringsFlownThrough = 0;
    private bool[] ringPassed = new bool[4]; // Annahme von vier Ringen
    public int playDuration = 5;

    private IEnumerator PlayAudioForDurationWithFadeOut(AudioClip clip, float duration)
    {
        audioSource.clip = clip;
        audioSource.Play();

        float currentTime = 0;
        float startVolume = audioSource.volume;

        // Schrittweise Verringerung der Lautstärke über die Dauer 'duration'
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, 0, currentTime / duration);
            yield return null;
        }

        audioSource.Stop();
        audioSource.volume = startVolume; // Setze die Lautstärke zurück auf den Anfangswert
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ring"))
        {
            int ringID = other.gameObject.GetComponent<RingRISE>().ringID; // Zugriff auf die Ring-ID

            if (!ringPassed[ringID])
            {
                ringsFlownThrough++;
                ringPassed[ringID] = true;

                // Starte Coroutine, um die Audiodatei für 5 Sekunden abzuspielen und auszublenden
                StartCoroutine(PlayAudioForDurationWithFadeOut(successClip, 5f));

                // Aktualisiere successTextVari basierend auf ringsFlownThrough
                UpdatesuccessTextVari();
            }
        }
    }

    private void Start()
    {
        // Initialisiere ringPassed Array mit false
        iniRingFlight();
    }

    public void iniRingFlight()
    {
        ringsFlownThrough = 0;
        for (int i = 0; i < ringPassed.Length; i++)
        {
            ringPassed[i] = false;
        }
        UpdatesuccessTextVari() ;
    }

    private void UpdatesuccessTextVari()
    {
        switch (ringsFlownThrough)
        {
            case 0:
                successTextVari.text = "Leider noch kein Ring geschafft - Sie schaffen es!";
                break;
            case 1:
                successTextVari.text = "1ter Ring geschafft - Guter Start!";
                break;
            case 2:
                successTextVari.text = "2ter Ring geschafft - Fantastisch, weiter so!";
                break;
            case 3:
                successTextVari.text = "3ter Ring geschafft - Fast da, super Arbeit!";
                break;
            case 4:
                successTextVari.text = "4ter Ring geschafft - Perfekt, alle Ringe gemeistert!";
                break;
        }
    }
}

