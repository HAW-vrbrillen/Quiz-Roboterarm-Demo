using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playAddedClipOnAudioSourceOnTrigger : MonoBehaviour
{
    public AudioClip clip;
    public AudioSource audioSource;

    private void Start()
    {
        audioSource.clip = clip;
    }
    public void playAddedClipOnAS()
    {
        audioSource.Stop();
        audioSource.Play();
    }
    private void OnTriggerEnter(Collider other)
    {
        playAddedClipOnAS();
    }
    private void OnTriggerExit(Collider other)
    {
        audioSource.Stop();
    }
}
