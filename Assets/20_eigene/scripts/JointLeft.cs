using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JointLeft : MonoBehaviour
{
    public Joint m_child;


    public Joint GetChild()
    {
        return m_child; // method to get the child
    }

    public void Rotate(float _angle)
    {
        transform.Rotate(Vector3.left * _angle);
    }
}
