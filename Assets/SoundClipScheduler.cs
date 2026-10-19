using UnityEngine;
using System.Collections;

public class SoundClipScheduler : MonoBehaviour
{
    public AudioSource audioSource1;
    public AudioClip clip1;
    public AudioSource audioSource2;
    public AudioClip clip2;


    private void Start()
    {
        audioSource1.clip = clip1;
        audioSource2.clip = clip2;

    }

    public void PlayAirbusSound()
    {
        // Stelle sicher, dass die AudioSource-Komponente und der Clip zugewiesen sind
        if (!audioSource1 || !clip1 || !audioSource2 || !clip2)
        {
            Debug.LogError("AudioSource-Komponente oder Clips fehlen!");
            return;
        }

        // Starte den ersten Clip bei 15 Sekunden
        PlayClipForDuration(audioSource1, clip1, 20f, 5f); // 10 Sekunden lang ab der 15. Sekunde

        // Berechne den Zeitpunkt, um den zweiten Clip zu starten
        // Beachte: Die Startzeit plus die Dauer des ersten Clips
        double startTimeForClip2 = AudioSettings.dspTime + 15f ; //+ 10f; // Startet in 25 Sekunden

        // Plane das Abspielen des zweiten Clips direkt nach dem ersten
        audioSource2.clip = clip2;
        audioSource2.PlayScheduled(5f); //startTimeForClip2);
    }

    void PlayClipForDuration(AudioSource source, AudioClip clip, float startAt, float duration)
    {
        source.clip = clip;
        source.time = startAt; // Setze den Startpunkt des Clips
        source.Play();
        StartCoroutine(StopClipAfterDuration(source, duration));
    }

    IEnumerator StopClipAfterDuration(AudioSource source, float duration)
    {
        yield return new WaitForSeconds(duration);
        source.Stop(); // Stoppe den Clip nach der spezifizierten Dauer
    }
}
