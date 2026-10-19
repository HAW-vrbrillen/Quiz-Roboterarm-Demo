using UnityEngine;

public class FollowParentRISE : MonoBehaviour
{
    public Transform parentTransform; // Das Transform des Parent-Objekts

    // Optional: Sollen Position, Rotation und Skalierung übertragen werden?
    public bool followPosition = true;
    public bool followRotation = true;
    public bool followScale = false;

    // Position Deltas
    public float positionyDelta;
    public float positionzDelta;
    public float positionxDelta;

    // NEU: Rotation Deltas
    public float rotationxDelta;
    public float rotationyDelta;
    public float rotationzDelta;

    void Update()
    {
        if (parentTransform != null)
        {
            if (followPosition)
            {
                transform.position = parentTransform.position + new Vector3(positionxDelta, positionyDelta, positionzDelta);
            }

            if (followRotation)
            {
                // Kombiniert die Elternrotation mit den Delta-Werten der Rotation
                Quaternion deltaRotation = Quaternion.Euler(rotationxDelta, rotationyDelta, rotationzDelta);
                transform.rotation = parentTransform.rotation * deltaRotation;
            }

            if (followScale)
            {
                transform.localScale = parentTransform.localScale;
            }
        }
    }
}
