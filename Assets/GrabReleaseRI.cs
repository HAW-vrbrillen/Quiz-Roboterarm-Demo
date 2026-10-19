using UnityEngine;
using Oculus.Interaction;

public class GrabReleaseRI : MonoBehaviour
{
    public GameObject otherObject; // Das andere Objekt, das deaktiviert/aktiviert werden soll
    private OVRGrabbable grabbable;

    void Start()
    {
        grabbable = GetComponent<OVRGrabbable>();
    }

    void Update()
    {
        if (grabbable.isGrabbed && otherObject.activeSelf)
        {
            otherObject.SetActive(false);
        }
        else if (!grabbable.isGrabbed && !otherObject.activeSelf)
        {
            otherObject.SetActive(true);
        }
    }
}
