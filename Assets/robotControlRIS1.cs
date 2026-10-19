using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class robotControlRIS1 : MonoBehaviour
{
    [Header("Control Flags")]
    public bool move1 = false;

    [Header("Robot Axes - Assign in Inspector")]
    public Transform axis1;
    public Transform axis2;
    public Transform axis3;
    public Transform axis4;
    public Transform axis5;
    public Transform axis6;

    [Header("Movement Settings")]
    public float rotationSpeed = 30f;
    public float axis1TargetAngle = 90f;
    public float axis2TargetAngle = 45f;
    public float axis3TargetAngle = -30f;
    public float axis4TargetAngle = 60f;
    public float axis5TargetAngle = -45f;
    public float axis6TargetAngle = 90f;

    // Initial rotations stored at start
    private Quaternion axis1Initial;
    private Quaternion axis2Initial;
    private Quaternion axis3Initial;
    private Quaternion axis4Initial;
    private Quaternion axis5Initial;
    private Quaternion axis6Initial;

    private bool isMoving = false;

    void Start()
    {
        StoreInitialRotations();
    }

    void StoreInitialRotations()
    {
        if (axis1 != null) axis1Initial = axis1.localRotation;
        if (axis2 != null) axis2Initial = axis2.localRotation;
        if (axis3 != null) axis3Initial = axis3.localRotation;
        if (axis4 != null) axis4Initial = axis4.localRotation;
        if (axis5 != null) axis5Initial = axis5.localRotation;
        if (axis6 != null) axis6Initial = axis6.localRotation;
    }

    void Update()
    {
        if (move1 && !isMoving)
        {
            move1 = false;
            StartCoroutine(ExecuteMove1Sequence());
        }
    }

    IEnumerator ExecuteMove1Sequence()
    {
        isMoving = true;

        // Move each axis sequentially to target position
        // S-Achse (Sockel) - dreht um Y
        if (axis1 != null)
            yield return StartCoroutine(RotateAxisToAngle(axis1, axis1Initial, Vector3.up, axis1TargetAngle));

        // L-Achse (Lower arm) - dreht um Z
        if (axis2 != null)
            yield return StartCoroutine(RotateAxisToAngle(axis2, axis2Initial, Vector3.forward, axis2TargetAngle));

        // U-Achse (Upper arm) - dreht um Z
        if (axis3 != null)
            yield return StartCoroutine(RotateAxisToAngle(axis3, axis3Initial, Vector3.forward, axis3TargetAngle));

        // R-Achse (Rotation) - dreht um Z
        if (axis4 != null)
            yield return StartCoroutine(RotateAxisToAngle(axis4, axis4Initial, Vector3.forward, axis4TargetAngle));

        // B-Achse (Bend) - dreht um Y
        if (axis5 != null)
            yield return StartCoroutine(RotateAxisToAngle(axis5, axis5Initial, Vector3.up, axis5TargetAngle));

        // T-Achse (Tool) - dreht um Z
        if (axis6 != null)
            yield return StartCoroutine(RotateAxisToAngle(axis6, axis6Initial, Vector3.forward, axis6TargetAngle));

        // Short pause at target position
        yield return new WaitForSeconds(0.5f);

        // Return to initial state (in reverse order)
        if (axis6 != null)
            yield return StartCoroutine(ReturnToInitial(axis6, axis6Initial));

        if (axis5 != null)
            yield return StartCoroutine(ReturnToInitial(axis5, axis5Initial));

        if (axis4 != null)
            yield return StartCoroutine(ReturnToInitial(axis4, axis4Initial));

        if (axis3 != null)
            yield return StartCoroutine(ReturnToInitial(axis3, axis3Initial));

        if (axis2 != null)
            yield return StartCoroutine(ReturnToInitial(axis2, axis2Initial));

        if (axis1 != null)
            yield return StartCoroutine(ReturnToInitial(axis1, axis1Initial));

        isMoving = false;
    }

    IEnumerator RotateAxisToAngle(Transform axis, Quaternion initialRotation, Vector3 rotationAxis, float targetAngle)
    {
        Quaternion targetRotation = initialRotation * Quaternion.AngleAxis(targetAngle, rotationAxis);

        while (Quaternion.Angle(axis.localRotation, targetRotation) > 0.1f)
        {
            axis.localRotation = Quaternion.RotateTowards(
                axis.localRotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
            yield return null;
        }

        axis.localRotation = targetRotation;
    }

    IEnumerator ReturnToInitial(Transform axis, Quaternion initialRotation)
    {
        while (Quaternion.Angle(axis.localRotation, initialRotation) > 0.1f)
        {
            axis.localRotation = Quaternion.RotateTowards(
                axis.localRotation,
                initialRotation,
                rotationSpeed * Time.deltaTime
            );
            yield return null;
        }

        axis.localRotation = initialRotation;
    }
}
