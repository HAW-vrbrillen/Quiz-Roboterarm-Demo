using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class rotateObj : MonoBehaviour
{
    public GameObject Rotor;
    public float speed;
    public bool startRotateNow;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void startRotate()
    {
        startRotateNow = !startRotateNow;
    }
    // Update is called once per frame
    void Update()
    {
        if (startRotateNow)
        {
            Rotor.transform.Rotate(0, 0, speed * Time.deltaTime);
        }
        
    }
}
