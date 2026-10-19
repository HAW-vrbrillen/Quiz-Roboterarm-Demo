using TMPro;
using UnityEngine;

public class ObjectAligner : MonoBehaviour
{
    public Transform targetObject; // Das Objekt, an dessen Position sich das aktuelle Objekt ausrichten soll
    public AudioSource soundToPlay; // Der AudioSource-Komponente, die den Sound abspielt

    private float rotationTolerance = 5.0f; // Toleranz für die Rotation in Grad
    private float positionTolerance = 0.01f; // Toleranz für die Position in Metern
    public TMP_Text info1;

    void Update()
    {
        // Prüfen, ob die Rotation innerhalb der Toleranz liegt
        info1.text = $" position : {transform.localPosition.ToString()} \n Grad Distanz: {Mathf.Abs(transform.eulerAngles.z - 180)} \nPos Distanz: {Vector3.Distance(transform.position, targetObject.position)}";
        Debug.Log(info1.text);
        if (Mathf.Abs(transform.eulerAngles.z - 180) <= rotationTolerance)
        {
            // Prüfen, ob die Position innerhalb der Toleranz liegt
            if (Vector3.Distance(transform.position, targetObject.position) <= positionTolerance)
            {
                // Sound abspielen, wenn er noch nicht abgespielt wird
                if (!soundToPlay.isPlaying)
                {
                    soundToPlay.Play();
                }
            }
        }
    }
}
