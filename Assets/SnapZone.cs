using Oculus.Interaction.HandGrab;
using TMPro;
using UnityEngine;

public class SnapZone : MonoBehaviour
{
    public Transform snapPoint; // Der Punkt, an dem Objekte einrasten sollen
    public TMP_Text info;

    void OnTriggerEnter(Collider other)
    {
        
        HandGrabInteractable interactable = other.GetComponent<HandGrabInteractable>();
        info.text = "trigger snapzone, interactable = " + interactable.name;
        //if (interactable != null && interactable.IsGrabbed)
        if (interactable != null)
            {
            //HandGrabInteractor grabInteractor = interactable.GrabbingInteractor;
            HandGrabInteractor interactor = interactable.GetComponent<HandGrabInteractor>(); //yrise 20231126
            ReleaseObject(interactor);
            SnapObject(interactable.gameObject);
        }
    }

    private void SnapObject(GameObject objectToSnap)
    {
        objectToSnap.transform.position = snapPoint.position;
        objectToSnap.transform.rotation = snapPoint.rotation;
    }

    private void ReleaseObject(HandGrabInteractor interactor)
    {
        // Setzen Sie hier Logik ein, um das Objekt loszulassen
        // Möglicherweise müssen Sie eine öffentliche Methode in HandGrabInteractor verwenden oder auf eine bestimmte Weise darauf zugreifen
        //interactor.ForceRelease(interactable);
        interactor.ForceRelease();
    }
}
