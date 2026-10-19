using UnityEngine;

public class HandshakeTrigger : MonoBehaviour
{
    public Transform object1;
    public Transform object2;
    public Animator animator; // Stelle sicher, dass der Animator des Objekts, das die Animation abspielen soll, hier zugewiesen ist
    private bool isHandshakeTriggered = false;
    public managerSdginfo managerSdginfo1;

    // Der Name des Animationszustands, den du überwachen möchtest
    private string handshakeAnimationStateName = "Handshake";

    // Die Layer-Nummer des Animators, normalerweise 0 für die Basis-Layer
    private int layerIndex = 0;

    void Update()
    {
        // Überprüfe die Distanz nur, wenn der Handshake noch nicht ausgelöst wurde
        if (!isHandshakeTriggered && Vector3.Distance(object1.position, object2.position) < 0.04f)
        {
            animator.SetTrigger("shakeHands"); // Setze den Trigger im Animator
            managerSdginfo1.startImmersionLargeWater();
            isHandshakeTriggered = true; // Verhindere, dass der Trigger mehrfach ausgelöst wird
        }

        // Überprüfe, ob der Handshake-Animationszustand am Ende ist
        if (isHandshakeTriggered && IsAnimationStateAlmostFinished(handshakeAnimationStateName, layerIndex, 0.95f))
        {
            // Hier kannst du Aktionen ausführen, die nach dem Ende der Animation erfolgen sollen
            // Zum Beispiel das Deaktivieren der IK-Steuerung
            var ikController = animator.GetComponent<ArmIKController>();
            if (ikController != null)
            {
                ikController.SetIKActive(false);
            }
            isHandshakeTriggered = false; // Setze zurück, falls du den Handshake erneut auslösen möchtest
        }
    }

    private bool IsAnimationStateAlmostFinished(string stateName, int layerIndex, float threshold)
    {
        var stateInfo = animator.GetCurrentAnimatorStateInfo(layerIndex);
        // Überprüfe, ob wir uns im richtigen Animationszustand befinden und ob die normierte Zeit nahe 1.0 ist
        return stateInfo.IsName(stateName) && stateInfo.normalizedTime >= threshold;
    }
}
