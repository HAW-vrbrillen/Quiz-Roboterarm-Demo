using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class reactonObject : MonoBehaviour
{
    
    public GameObject objectToReactTo;
    public AudioClip soundToPlay;
    public AudioSource audioSource;
    public ParticleSystem particleSystem;
    public TMP_Text text;

    // Start is called before the first frame update
    void Start()
    {
        particleSystem.Stop();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == objectToReactTo)
        {
            Debug.Log("Object entered trigger");
            audioSource.PlayOneShot(soundToPlay);
            particleSystem.Play();
            text.text = "Challenge 2 erfolgreich:" +
                "Die HAW Hamburg heiﬂt Dich willkommen!";
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == objectToReactTo)
        {
            Debug.Log("Object left trigger");
            particleSystem.Stop();
            audioSource.Stop();
            //text.text = "";
        }

    }

}
