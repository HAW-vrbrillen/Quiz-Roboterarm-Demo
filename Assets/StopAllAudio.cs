using UnityEngine;

public class StopAllAudio : MonoBehaviour
{
    // Diese Methode können Sie aufrufen, um alle Audioquellen zu stoppen
    public void StopAllAudioSources()
    {
        // Findet alle AudioSource Komponenten, die sich momentan in der Szene befinden
        AudioSource[] audioSources = FindObjectsOfType<AudioSource>();

        foreach (AudioSource audioSource in audioSources)
        {
            audioSource.Stop();  // Stoppt die AudioSource
        }
    }
}
