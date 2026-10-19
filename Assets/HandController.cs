using UnityEngine;

public class HandController : MonoBehaviour
{
    public Transform[] fingerJoints; // Assign the finger joint Transforms here in the Inspector

    void Update()
    {
        OpenHand();
    }

    void OpenHand()
    {
        foreach (var joint in fingerJoints)
        {
            // Hier musst du die richtige Rotation für das Öffnen der Hand einstellen.
            // Dies ist ein Beispiel und muss wahrscheinlich angepasst werden.
            joint.localRotation = Quaternion.Euler(0f, 0f, 0f);
        }
    }
}
