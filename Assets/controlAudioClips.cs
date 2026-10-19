using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class controlAudioClips : MonoBehaviour
{
    public AudioSource as1;
    public AudioClip clip1;
    public AudioClip clip2;

    public void playClip1()
    {
        as1.clip = clip1;
        as1.Play();
        
    }
    public void playClip2()
    {
        as1.clip = clip2;
        as1.Play();

    }
    private void Start()
    {
        playClip2();
    }
}
