using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class microRecorderRIS : MonoBehaviour
{
    [Multiline]
    public string Anleitung = "V:Start,B:Stop, N:PlayRec, M:Transcribe";
    private AudioSource audioSource;
    private bool isRecording = false;
    private AudioClip recordedClip;
    public int recordingTime = 10; // Aufnahmezeit in Sekunden
    public bool directPlay = false;
    public AudioTranscriptionManager transcriptionManager;
    

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        Anleitung = "V:Start,B:Stop, N:PlayRec, M:Transcribe";
        Debug.Log("Micro: V:Start,B:Stop, N:PlayRec, M:Transcribe");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            StartRecording();
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            StopRecording();
        }

        if (Input.GetKeyDown(KeyCode.N))
        {
            PlayRecording();
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            transcriptionManager.transcribeAudioClipCall(recordedClip);
        }
    }
    [ContextMenu("start Recording")]
    private void StartRecording()
    {
        if (isRecording)
        {
            Debug.LogWarning("Bereits am Aufnehmen");
            return;
        }

        if (Microphone.devices.Length <= 0)
        {
            Debug.LogWarning("Kein Mikrofon gefunden");
            return;
        }
        Debug.Log("is recording");
        isRecording = true;
        recordedClip = Microphone.Start(null, true, recordingTime, 44100);
        StartCoroutine(StopRecordingAfterTime(recordingTime));
    }

    private IEnumerator StopRecordingAfterTime(int seconds)
    {
        yield return new WaitForSeconds(seconds);
        StopRecording();
        
    }

    private void StopRecording()
    {
        if (!isRecording)
        {
            Debug.LogWarning("Nicht am Aufnehmen");
            return;
        }
        Microphone.End(null);
        isRecording = false;
        // Zuweisen des aufgenommenen Clips, nachdem die Aufnahme gestoppt wurde
        if (audioSource.clip == null)
        {
            audioSource.clip = recordedClip;
        }
        Debug.Log("stopped recording");
        transcriptionManager.transcribeAudioClipCall(recordedClip);
        PlayRecording();
    }
    [ContextMenu("play recording")]
    private void PlayRecording()
    {
        if (audioSource.clip == null)
        {
            Debug.LogWarning("Keine Aufnahme zum Abspielen");
            return;
        }

        audioSource.PlayOneShot(recordedClip);
    }
}
