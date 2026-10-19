using UnityEngine;
using System.Collections.Generic;
using Oculus.Interaction; // Stellen Sie sicher, dass Sie den richtigen Namespace verwenden

public class FingerCountRecognition : MonoBehaviour
{
    public OVRSkeleton leftHandSkeleton;
    public OVRSkeleton rightHandSkeleton;

    private void Update()
    {
        int leftHandExtendedFingers = CountExtendedFingers(leftHandSkeleton);
        int rightHandExtendedFingers = CountExtendedFingers(rightHandSkeleton);

        Debug.Log($"Linke Hand: {leftHandExtendedFingers} ausgestreckte Finger.");
        Debug.Log($"Rechte Hand: {rightHandExtendedFingers} ausgestreckte Finger.");
    }

    private int CountExtendedFingers(OVRSkeleton handSkeleton)
    {
        if (handSkeleton == null || handSkeleton.Bones.Count == 0) return 0;

        int extendedFingers = 0;

        // Identifiziere die Knochen-IDs für die Endglieder jedes Fingers
        var tipBoneIds = new List<OVRPlugin.BoneId>
        {
            OVRPlugin.BoneId.Hand_Thumb3,
            OVRPlugin.BoneId.Hand_Index3,
            OVRPlugin.BoneId.Hand_Middle3,
            OVRPlugin.BoneId.Hand_Ring3,
            OVRPlugin.BoneId.Hand_Pinky3
        };

        foreach (var bone in handSkeleton.Bones)
        {
            if (tipBoneIds.Contains((OVRPlugin.BoneId)bone.Id))
            {
                // Berechne den Abstand zwischen dem Spitzenknochen und dem Handgelenk, um zu bestimmen, ob der Finger ausgestreckt ist
                float distance = Vector3.Distance(bone.Transform.position, handSkeleton.Bones[0].Transform.position); // Annahme: Bone[0] ist das Handgelenk
                
                // Werte müssen möglicherweise angepasst werden, um optimale Ergebnisse zu erzielen
                if (distance > 0.1f) // Beispielwert
                {
                    extendedFingers++;
                }
            }
        }

        return extendedFingers;
    }
}
