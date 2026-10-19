using UnityEngine;

public class ArmIKController : MonoBehaviour
{
    public Transform target; // Das Zielobjekt, zu dem der Arm bewegt werden soll
    public Animator animator; // Der Animator des gesamten Körpers
    public bool ikActive = false;

    void OnAnimatorIK(int layerIndex)
    {
        if (animator)
        {
            // Wenn ein Ziel für die IK gesetzt ist
            if (target != null && ikActive)
            {
                // Setze das Gewicht für die rechte Hand, um den Einfluss der IK zu aktivieren
                animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
                animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);

                // Setze die Position und Rotation für die rechte Hand
                animator.SetIKPosition(AvatarIKGoal.RightHand, target.position);
                animator.SetIKRotation(AvatarIKGoal.RightHand, target.rotation);
            }
            else
            {
                // Deaktiviere das Gewicht, um den Einfluss der IK zu deaktivieren, wenn ikActive falsch ist
                animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
                animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0);
            }
        }
    }

    // Öffentliche Methode zum Ein- oder Ausschalten der IK
    public void SetIKActive(bool state)
    {
        ikActive = state;
    }
}
