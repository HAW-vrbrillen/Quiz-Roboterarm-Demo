using UnityEngine;
using UnityEditor;
using Oculus.Interaction;
using Oculus.Interaction.HandGrab;

public class WireGrabReferences : Editor
{
    // Neue 3-Logo-Puzzle-Stücke
    static readonly string[] cubeNames = { "LP_Hamburg", "LP_HAW_Hamburg", "LP_Rathaus_Passage" };
    static readonly string[] targetNames = { "Target_LP_Hamburg", "Target_LP_HAW_Hamburg", "Target_LP_Rathaus_Passage" };

    [MenuItem("Tools/Wire Logo Puzzle Grab References")]
    static void Wire()
    {
        foreach (string name in cubeNames)
        {
            // Suche im gesamten Scene-Hierarchy
            GameObject cube = GameObject.Find(name);
            if (cube == null)
            {
                // Auch unter environment/Box/ versuchen
                cube = GameObject.Find("environment/Box/" + name);
            }
            if (cube == null) { Debug.LogWarning("Not found: " + name); continue; }

            Rigidbody rb = cube.GetComponent<Rigidbody>();
            Transform handGrab = cube.transform.Find("[BuildingBlock] HandGrab");
            if (handGrab == null) { Debug.LogWarning("No HandGrab child on: " + name); continue; }

            var grabbable = handGrab.GetComponent<Grabbable>();
            var handGrabInteractable = handGrab.GetComponent<HandGrabInteractable>();
            var grabInteractable = handGrab.GetComponent<GrabInteractable>();

            if (grabbable != null)
            {
                SerializedObject so = new SerializedObject(grabbable);
                so.FindProperty("_rigidbody").objectReferenceValue = rb;
                so.FindProperty("_targetTransform").objectReferenceValue = cube.transform;
                so.ApplyModifiedProperties();
            }

            if (handGrabInteractable != null)
            {
                SerializedObject so = new SerializedObject(handGrabInteractable);
                so.FindProperty("_rigidbody").objectReferenceValue = rb;
                so.FindProperty("_pointableElement").objectReferenceValue = grabbable;
                so.ApplyModifiedProperties();
            }

            if (grabInteractable != null)
            {
                SerializedObject so = new SerializedObject(grabInteractable);
                so.FindProperty("_rigidbody").objectReferenceValue = rb;
                so.FindProperty("_pointableElement").objectReferenceValue = grabbable;
                so.ApplyModifiedProperties();
            }

            EditorUtility.SetDirty(cube);
            Debug.Log("Wired grab references for: " + name);
        }

        // Wire LogoPuzzleSnap arrays
        WirePuzzleSnap();
    }

    static void WirePuzzleSnap()
    {
        // Suche LogoPuzzleSnap Komponente in der Szene
        LogoPuzzleSnap snap = Object.FindObjectOfType<LogoPuzzleSnap>();
        if (snap == null) { Debug.LogWarning("LogoPuzzleSnap nicht in der Szene gefunden!"); return; }

        SerializedObject so = new SerializedObject(snap);

        SerializedProperty piecesProp = so.FindProperty("puzzlePieces");
        piecesProp.arraySize = cubeNames.Length;
        for (int i = 0; i < cubeNames.Length; i++)
        {
            GameObject cube = GameObject.Find(cubeNames[i])
                ?? GameObject.Find("environment/Box/" + cubeNames[i]);
            if (cube != null)
                piecesProp.GetArrayElementAtIndex(i).objectReferenceValue = cube.transform;
        }

        SerializedProperty targetsProp = so.FindProperty("targetPositions");
        targetsProp.arraySize = targetNames.Length;
        for (int i = 0; i < targetNames.Length; i++)
        {
            // Suche Targets unter dem PuzzleTargets_Logos Container oder direkt
            GameObject target = GameObject.Find(targetNames[i]);
            if (target != null)
                targetsProp.GetArrayElementAtIndex(i).objectReferenceValue = target.transform;
        }

        so.ApplyModifiedProperties();
        EditorUtility.SetDirty(snap.gameObject);
        Debug.Log($"LogoPuzzleSnap arrays wired: {cubeNames.Length} pieces + {targetNames.Length} targets");
    }
}
