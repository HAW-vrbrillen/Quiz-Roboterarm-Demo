using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Joint : MonoBehaviour
{
    public Joint m_child;
    public string m_jointtype = "y";


    public Joint GetChild()
    {
        return m_child; // method to get the child
    }

    public void Rotate(float _angle)
    {
        if (m_jointtype == "y")
          transform.Rotate(Vector3.up * _angle);
        if (m_jointtype == "x")
            transform.Rotate(Vector3.left * _angle);
        if (m_jointtype == "z")
            transform.Rotate(Vector3.forward * _angle);
    }
}
