using System.Collections.Generic;
using UnityEngine;

public class PositionResetter : MonoBehaviour
{
    public Transform parentObject;
    public CubeFormationChecker cubeFormationChecker;

    // Eine Hilfsklasse, um Position und Rotation zu speichern
    private class TransformData
    {
        public Vector3 Position;
        public Quaternion Rotation;

        public TransformData(Vector3 position, Quaternion rotation)
        {
            Position = position;
            Rotation = rotation;
        }
    }

    private List<KeyValuePair<GameObject, TransformData>> originalTransforms = new List<KeyValuePair<GameObject, TransformData>>();

    void Start()
    {
        if (parentObject == null)
        {
            Debug.LogError("Parent object not assigned in PositionResetter.");
            return;
        }

        foreach (Transform child in parentObject)
        {
            // Speichert sowohl die Position als auch die Rotation
            originalTransforms.Add(new KeyValuePair<GameObject, TransformData>(
                child.gameObject, new TransformData(child.position, child.rotation)));
        }
    }

   public void resetTextResults()
    {
        cubeFormationChecker.cubeThrowResult.text = "Bitte beginnen.";
        cubeFormationChecker.cubeBuildingResult.text = "Bitte beginnen.";
        cubeFormationChecker.cubeShiftResult.text = "Bitte beginnen.";
    }

    public void ResetPositions()
    {
        cubeFormationChecker.AufgabeErledigt = false;
        

        foreach (var kvp in originalTransforms)
        {
            Rigidbody rb = kvp.Key.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.position = kvp.Value.Position;
                rb.rotation = kvp.Value.Rotation;
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
            else
            {
                kvp.Key.transform.position = kvp.Value.Position;
                kvp.Key.transform.rotation = kvp.Value.Rotation;
            }
        }
    }

}
