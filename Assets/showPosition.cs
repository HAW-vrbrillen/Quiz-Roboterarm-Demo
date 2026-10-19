using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class showPosition : MonoBehaviour
{
    // Start is called before the first frame update
    public TMP_Text text1;
    public GameObject go1;
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        text1.text =  go1.transform.position.ToString();
    }
}
