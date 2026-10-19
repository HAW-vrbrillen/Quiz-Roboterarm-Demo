using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class climateControl1 : MonoBehaviour
{
    public GameObject water1;
    public Toggle tog1;
    public float speed;
    // Start is called before the first frame update
    void Start()
    {
        tog1.isOn = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (tog1.isOn)
        {
            water1.transform.Translate(0,speed*Time.deltaTime,0);
        }
        
    }
}
