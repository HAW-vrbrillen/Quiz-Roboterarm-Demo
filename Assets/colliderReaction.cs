using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class colliderReaction1 : MonoBehaviour
{
    public Collider col1;
    public AudioSource as1;
    public AudioClip asc1;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        as1.PlayOneShot(asc1);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
