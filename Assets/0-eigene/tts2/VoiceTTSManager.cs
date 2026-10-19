using Oculus.Voice;
using UnityEngine;

public class VoiceTTSManager : MonoBehaviour
{
    private AppVoiceExperience voiceExperience;

    void Start()
    {
        voiceExperience = GetComponent<AppVoiceExperience>();
        if (voiceExperience == null)
        {
            Debug.LogError("Voice Experience Component fehlt!");
        }
    }

    public void Speak(string text)
    {
        if (voiceExperience != null)
        {
            //voiceExperience.ActivateWitRequest(text); // Hier wird der Text über das Wit-System ausgegeben.
            Debug.Log("TTS: " + text);
        }
        else
        {
            Debug.LogError("Voice Experience ist nicht verfügbar.");
        }
    }
}
