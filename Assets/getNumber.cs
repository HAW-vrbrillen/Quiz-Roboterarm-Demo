using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Windows;

public class getNumber : MonoBehaviour
{
    public TMP_Text input1;
    public void setNumber()
    {
        if (name == "bck") {
            input1.text = input1.text.Substring(0, input1.text.Length - 1);
        }
        else
        {
            input1.text = input1.text + name;
        }
    }
}
