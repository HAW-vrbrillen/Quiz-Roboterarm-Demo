using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NoticeTargetArrived : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject target1;
    public TMP_Text do1text;
    public AudioSource as1;
    public AudioClip aclip1;
    public AudioClip aclip2;

    void Start()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == target1.name)
        {
            do1text.text = do1text.text.Replace("OFFEN", "Erledigt");
            if (as1.isPlaying == false)
            {
                as1.PlayOneShot(aclip1);
            }
            

        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == target1.name)
        {
            do1text.text = do1text.text.Replace("Erledigt", "OFFEN");
            if (as1.isPlaying == false)
            {
                as1.PlayOneShot(aclip2);
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
