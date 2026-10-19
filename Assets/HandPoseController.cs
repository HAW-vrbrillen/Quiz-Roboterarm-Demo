using UnityEngine;

[System.Serializable]
public class FingerTransform
{
    public Vector3 localPosition;
    public Quaternion localRotation;
    public Vector3 localScale;
}

public class HandPoseController : MonoBehaviour
{
    public bool IKisActive = false;
    private bool previousIKState; // Speichert den vorherigen Zustand von IKisActive
    public Transform[] fingers; // Füge die Fingertransformationen hier im Inspector hinzu
    private FingerTransform[] defaultHandTransforms; // Zum Speichern der T-Pose der Hand

    private int ikLayerIndex; // Der Index des IK-Override Layers
    public Animator animator; // Der Animator des Charakters

    void Awake()
    {
        // Finde den Index des IK-Override Layers
        ikLayerIndex = animator.GetLayerIndex("IKOverwrite");
        // Setze das Gewicht des IK Layers auf 0, damit der Base Layer kontrolliert
        animator.SetLayerWeight(ikLayerIndex, 0);

        // Initialisiere das Array für die T-Pose
        defaultHandTransforms = new FingerTransform[fingers.Length];

        // Speichere die T-Pose der Hand
        StoreHandPose(defaultHandTransforms);

        // Setze den vorherigen Zustand von IKisActive
        previousIKState = false;
    }

    public void StoreHandPose(FingerTransform[] handTransforms)
    {
        for (int i = 0; i < fingers.Length; i++)
        {
            handTransforms[i] = new FingerTransform
            {
                localPosition = fingers[i].localPosition,
                localRotation = fingers[i].localRotation,
                localScale = fingers[i].localScale
            };
        }
    }

    public void ApplyHandPose(FingerTransform[] handTransforms)
    {
        for (int i = 0; i < fingers.Length; i++)
        {
            fingers[i].localPosition = handTransforms[i].localPosition;
            fingers[i].localRotation = handTransforms[i].localRotation;
            fingers[i].localScale = handTransforms[i].localScale;
        }
    }

    // Diese Methode stellt die T-Pose wieder her
    public void ResetToTPose()
    {
        Debug.Log("20240310 reset totpose");
        ApplyHandPose(defaultHandTransforms);
    }

    void Update()
    {
        Debug.Log("im update");
        // Überprüfe, ob sich der Zustand von IKisActive geändert hat
        if (IKisActive)
        {
            // Aktualisiere den vorherigen IK-Zustand
            previousIKState = IKisActive;

            // Setze das Gewicht des IK Layers basierend auf dem Zustand von IKisActive
            //animator.SetLayerWeight(ikLayerIndex, IKisActive ? 1 : 0);

            // Wenn IK aktiv ist, wende die T-Pose an, um die IK-Steuerung zu simulieren
            if (IKisActive)
            {
                ResetToTPose();
            }
            // Wenn IK nicht aktiv ist, wird die Kontrolle automatisch an die Animation im Base Layer übergeben
        }
    }
}
