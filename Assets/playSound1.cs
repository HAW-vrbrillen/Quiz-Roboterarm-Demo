using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playSound1 : MonoBehaviour
{
    private AudioSource as1;
    // Start is called before the first frame update
    void Start()
    {
        as1 = GetComponent<AudioSource>();
    }

    public void playthisAudio()
    {
        if (as1.isPlaying == false)
        {
            as1.Play();
        }
        else { as1.Stop(); }
        
        
    }
    private void OnTriggerEnter(Collider other)
    {
        as1.Play();
    }
    private void OnTriggerExit(Collider other)
    {
        as1.Stop();   
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
