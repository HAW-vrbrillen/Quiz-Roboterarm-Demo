using System.Collections;
using System.Collections.Generic;
//using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

public class playAudioSourceOfObj : MonoBehaviour
{
    public AudioSource as1;
    bool onlyonce = true;


    // Start is called before the first frame update
    void Start()
    {
        as1.Stop();
        onlyonce = true;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (as1.isPlaying == false)
        {
            if(onlyonce){
                as1.Play();
                onlyonce = false;
            }
            
        }

    }


    // Update is called once per frame
    void Update()
    {

    }
}
