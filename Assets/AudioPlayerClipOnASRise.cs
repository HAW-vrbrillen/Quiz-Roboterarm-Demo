using UnityEngine;

public class AudioPlayerClipOnASRise : MonoBehaviour
{
    // Referenz zur AudioSource-Komponente
    private AudioSource audioSource;

    // Hier kannst du den AudioClip im Inspector festlegen
    [Header ("Funktion: PlayAudioClipOnAudioSource")]
    public AudioClip audioClip;

    void Start()
    {
        // Versuche, eine AudioSource-Komponente zu finden, die bereits am GameObject hängt
        audioSource = GetComponent<AudioSource>();

        // Wenn keine AudioSource-Komponente vorhanden ist, füge eine hinzu
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    // Diese Funktion spielt den AudioClip ab, der der Variablen audioClip zugewiesen wurde
    public void PlayAudioClipOnAudioSource()
    {
        // Überprüfe, ob ein AudioClip zugewiesen wurde, um Fehler zu vermeiden
        if (audioClip != null)
        {
            // Setze den AudioClip auf die AudioSource und spiele ihn ab
            audioSource.clip = audioClip;
            audioSource.Play();
        }
        else
        {
            Debug.LogWarning("AudioClip ist nicht zugewiesen!");
        }
    }
}
