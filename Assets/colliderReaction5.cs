using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class colliderReaction5 : MonoBehaviour
{
    public Collider col1;
    public AudioSource as1;
    public AudioClip asc1;
    public Animator animator;

    // Start is called befoanre the first frame update
    void Start()
    {
        animator =  GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        as1.Stop();
        as1.PlayOneShot(asc1);
        animator.SetTrigger("attack");
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
