using System.Collections;
using UnityEngine;
using Whisper;

public class AudioTranscriptionManager : MonoBehaviour
{
    public AudioClip clipToTranscribe;
    [Multiline]
    public string Anleitung = "Sprachen: de, en, es, fr,\n"+ "zh (chinesisch), ru Russisch";

    private void Start()
    {
        //StartCoroutine(TranscribeAudioClip(clipToTranscribe));
    }

    public void transcribeAudioClipCall(AudioClip audioClip1)
    {
        StartCoroutine(TranscribeAudioClip(audioClip1));
    }

    private IEnumerator TranscribeAudioClip(AudioClip clip)
    {
        var whisperManager = FindObjectOfType<WhisperManager>();
        if (whisperManager == null)
        {
            Debug.LogError("WhisperManager not found in the scene!");
            yield break;
        }

        // Stellt sicher, dass das Whisper-Modell initialisiert ist
        if (!whisperManager.IsLoaded)
        {
            // Warten auf die Fertigstellung der InitModel Task
            var initTask = whisperManager.InitModel();
            yield return new WaitUntil(() => initTask.IsCompleted);
        }

        // Startet die Transkription und wartet auf das Ergebnis
        var transcriptionTask = whisperManager.GetTextAsync(clip);
        yield return new WaitUntil(() => transcriptionTask.IsCompleted);

        // Verarbeiten des Transkriptionsergebnisses
        var transcriptionResult = transcriptionTask.Result;
        if (transcriptionResult != null)
        {
            //Debug.Log("Transkribierter Text: " + transcriptionResult.Text);
            Debug.Log("Transkribierter Text: " + transcriptionResult.Result.ToString());
        }
        else
        {
            Debug.LogError("Transkription fehlgeschlagen");
        }
    }
}
