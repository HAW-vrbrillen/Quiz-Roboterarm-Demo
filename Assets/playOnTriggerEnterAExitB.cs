using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playOnTriggerEnterAExitB : MonoBehaviour
{
    public AudioClip clipA;
    public AudioClip clipB;
    AudioSource as1;
    // Start is called before the first frame update
    void Start()
    {
        as1 = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        as1.Stop();
        as1.PlayOneShot(clipA);
    }
    private void OnTriggerExit(Collider other)
    {
        as1.Stop();
        as1.PlayOneShot(clipB);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
