
#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using Oculus.Interaction;
using Oculus.Interaction.HandGrab;

/// <summary>
/// Weist dem DresselPanel HandGrab nur den Parent-BoxCollider als Grab-Collider zu,
/// damit man nicht überall am Panel greifen kann.
/// </summary>
public static class FixGrabColliders
{
    [MenuItem("Tools/Fix DresselPanel Grab Colliders")]
    public static void Fix()
    {
        var panel = GameObject.Find("DresselPanel");
        if (panel == null)
        {
            Debug.LogError("FixGrabColliders: 'DresselPanel' nicht gefunden.");
            return;
        }

        var boxCollider = panel.GetComponent<BoxCollider>();
        if (boxCollider == null)
        {
            Debug.LogError("FixGrabColliders: Kein BoxCollider auf DresselPanel.");
            return;
        }

        var handGrab = panel.transform.Find("[BuildingBlock] HandGrab");
        if (handGrab == null)
        {
            Debug.LogError("FixGrabColliders: '[BuildingBlock] HandGrab' nicht gefunden.");
            return;
        }

        // GrabInteractable — Colliders per SerializedObject setzen
        var grabInteractable = handGrab.GetComponent<GrabInteractable>();
        if (grabInteractable != null)
        {
            var so = new SerializedObject(grabInteractable);
            var colProp = so.FindProperty("_colliders");
            if (colProp == null)
                colProp = so.FindProperty("Colliders");
            if (colProp == null)
                colProp = so.FindProperty("_pointableColliders");

            if (colProp != null && colProp.isArray)
            {
                colProp.ClearArray();
                colProp.InsertArrayElementAtIndex(0);
                colProp.GetArrayElementAtIndex(0).objectReferenceValue = boxCollider;
                so.ApplyModifiedProperties();
                Debug.Log($"GrabInteractable: BoxCollider '{boxCollider.name}' als einziger Collider zugewiesen.");
            }
            else
            {
                // Alle serialisierten Properties auflisten zum Debuggen
                var iter = so.GetIterator();
                iter.Next(true);
                string props = "";
                while (iter.Next(false))
                    props += iter.name + ", ";
                Debug.LogWarning($"GrabInteractable: Collider-Property nicht gefunden. Properties: {props}");
            }
        }

        // HandGrabInteractable — Colliders per SerializedObject setzen
        var handGrabInteractable = handGrab.GetComponent<HandGrabInteractable>();
        if (handGrabInteractable != null)
        {
            var so = new SerializedObject(handGrabInteractable);
            var colProp = so.FindProperty("_colliders");
            if (colProp == null)
                colProp = so.FindProperty("Colliders");
            if (colProp == null)
                colProp = so.FindProperty("_pointableColliders");

            if (colProp != null && colProp.isArray)
            {
                colProp.ClearArray();
                colProp.InsertArrayElementAtIndex(0);
                colProp.GetArrayElementAtIndex(0).objectReferenceValue = boxCollider;
                so.ApplyModifiedProperties();
                Debug.Log($"HandGrabInteractable: BoxCollider '{boxCollider.name}' als einziger Collider zugewiesen.");
            }
            else
            {
                var iter = so.GetIterator();
                iter.Next(true);
                string props = "";
                while (iter.Next(false))
                    props += iter.name + ", ";
                Debug.LogWarning($"HandGrabInteractable: Collider-Property nicht gefunden. Properties: {props}");
            }
        }

        EditorUtility.SetDirty(panel);
        Debug.Log("FixGrabColliders: Fertig.");
    }
}
#endif
