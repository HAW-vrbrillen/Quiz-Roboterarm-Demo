using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playOnTriggerOneShot : MonoBehaviour
{
    [Header("Achtung Nicht bei Lipsync clip muss vorher zugeordnet sein")]
    public AudioClip ac1;
    public AudioSource as1;
    bool alreadyPlayed;
    private void Start()
    {
        alreadyPlayed = false;
    }
    public void playClipOneShot()
    {
        as1.PlayOneShot(ac1);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!alreadyPlayed)
        {
            as1.PlayOneShot(ac1);
            alreadyPlayed=true;
        }
    }
    
}
