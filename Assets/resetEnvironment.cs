using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class resetEnvironment : MonoBehaviour
{
    public Transform toBeReseted;

    private void Start()
    {
        toBeReseted = GameObject.Find("Terrain").transform;
    }

    public void resetTransformEnvironment()
    {
        toBeReseted.transform.localPosition = new Vector3(0, 0, 0);
    }
}
