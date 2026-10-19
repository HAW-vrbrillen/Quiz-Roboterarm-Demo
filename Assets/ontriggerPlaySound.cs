using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
/// <summary>
/// Tipp: Trigger: beide Objekte müssen Collider haben, eines muss den Collider auf isTrigger On gesetzt haben
/// und eines der Objekte muss ein rigidbody haben (kann kinematic sein, wenn sie ihn nur für den Trigger nutzen)
/// </summary>
public class ontriggerPlaySound : MonoBehaviour
{
    public AudioSource as1;
    // Start is called before the first frame update
    void Start()
    {
        as1.Stop();
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
