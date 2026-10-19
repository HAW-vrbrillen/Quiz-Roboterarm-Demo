using RIS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testTTS1 : MonoBehaviour
{
    private TTSSpeakerInput _input;
    public bool playFlag;
    // Start is called before the first frame update
    void Start()
    {
        _input = GameObject.Find("TTSSpeakerInput").GetComponent<TTSSpeakerInput>();
        if (playFlag)
        {
            _input.SpeakIt("Test");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
