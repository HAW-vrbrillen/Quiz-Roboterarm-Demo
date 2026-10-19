using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class toggleLipSyncTalk : MonoBehaviour
{
    public AudioSource as1;
    public AudioClip ac1;



    public void toggleLipSyncSound1()
    {

        if (as1.isPlaying)
        {
            as1.Stop();
        }
        else
        {
            as1.clip = ac1;
            as1.Play();
        }

    }
}
