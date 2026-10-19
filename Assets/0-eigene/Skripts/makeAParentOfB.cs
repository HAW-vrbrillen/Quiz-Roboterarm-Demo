using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class makeBChildofA : MonoBehaviour
{
    public GameObject Aparent;
    public GameObject Bchild;

    public void setAParentOfB()
    {
        Bchild.transform.parent = Aparent.transform;
    }
    public void setNullParentOfB()
    {
        Bchild.transform.parent = null;
    }
}
