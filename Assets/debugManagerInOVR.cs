using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class debugManagerInOVR : MonoBehaviour
{
    [Header("Adds Text to DebugInfo Field on Panel use emptyString to reset")]
    public TMP_Text debugInfo;
    
    // Start is called before the first frame update
    void Start()
    {
        debugInfo = GameObject.Find("DebugInfo").GetComponent<TMP_Text>();
    }

    public void addDebugInfoText(string newDebugInfo)
    {
        debugInfo.text = debugInfo.text + newDebugInfo;
        if(newDebugInfo == "")
        {
            debugInfo.text = "";
        }
    }
}
