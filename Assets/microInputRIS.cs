using UnityEngine;
using UnityEngine.UI; // Notwendig für den Zugriff auf UI-Elemente
using System.Collections.Generic; // Für die Verwendung von Listen
using TMPro;

[RequireComponent(typeof(AudioSource))]
public class MicrophoneInput : MonoBehaviour
{
    public TMP_Dropdown micDropdown; // Referenz auf das Dropdown-Menü im UI
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        InitializeMicDropdown();
    }

    void InitializeMicDropdown()
    {
        if (micDropdown != null)
        {
            List<string> micOptions = new List<string>(Microphone.devices);
            micDropdown.ClearOptions();
            micDropdown.AddOptions(micOptions);

            // Optional: Standardmäßig das erste Mikrofon auswählen, falls verfügbar
            if (micOptions.Count > 0)
            {
                micDropdown.value = 2;
                StartMicrophone(micOptions[0]);
            }
        }
        else
        {
            Debug.LogError("MicDropdown ist nicht im Inspector zugewiesen!");
        }
    }

    public void OnMicDropdownValueChanged(int index)
    {
        // Diese Methode sollte im OnValueChanged-Event des Dropdowns aufgerufen werden
        string selectedMic = Microphone.devices[index];
        StartMicrophone(selectedMic);
    }

    void StartMicrophone(string micName)
    {
        audioSource.clip = Microphone.Start(micName, true, 10, 44100);
        while (!(Microphone.GetPosition(micName) > 0)) { }
        audioSource.Play();
    }
}
