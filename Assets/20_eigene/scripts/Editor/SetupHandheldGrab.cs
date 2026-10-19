using UnityEngine;
using UnityEditor;

/// <summary>
/// Editor-Script: Macht das DresselWelcomePanel (Handheld) greifbar
/// NUR an den Holzgriffen HandleLeft und HandleRight.
///
/// Struktur:
///   DresselWelcomePanel (Parent) — Rigidbody (kinematic)
///     ├─ HandleLeft
///     │    └─ [BuildingBlock] HandGrab (Grabbable + HandGrabInteractable)
///     └─ HandleRight
///          └─ [BuildingBlock] HandGrab (Grabbable + HandGrabInteractable)
///
/// Menü: GameObject > Handheld Grab einrichten
/// </summary>
public class SetupHandheldGrab : Editor
{
    static readonly string[] handleNames = { "HandleLeft", "HandleRight" };

    [MenuItem("GameObject/Handheld Grab einrichten", false, 11)]
    static void Setup()
    {
        GameObject selected = Selection.activeGameObject;
        if (selected == null)
        {
            EditorUtility.DisplayDialog("Kein Objekt ausgewählt",
                "Bitte das DresselWelcomePanel (Handheld) in der Hierarchy auswählen.", "OK");
            return;
        }

        // Handles suchen
        bool anyFound = false;
        foreach (string handleName in handleNames)
        {
            Transform handle = FindChildRecursive(selected.transform, handleName);
            if (handle != null) { anyFound = true; break; }
        }

        if (!anyFound)
        {
            EditorUtility.DisplayDialog("Handles nicht gefunden",
                "Kein 'HandleLeft' oder 'HandleRight' als Kind gefunden.\n" +
                "Bitte benenne die Holzgriff-Objekte entsprechend.", "OK");
            return;
        }

        Undo.RegisterCompleteObjectUndo(selected, "Setup Handheld Grab");

        // Rigidbody auf Parent (falls noch nicht vorhanden)
        Rigidbody rb = selected.GetComponent<Rigidbody>();
        if (rb == null)
            rb = Undo.AddComponent<Rigidbody>(selected);
        rb.isKinematic = true;
        rb.useGravity = false;

        int setupCount = 0;

        foreach (string handleName in handleNames)
        {
            Transform handle = FindChildRecursive(selected.transform, handleName);
            if (handle == null)
            {
                Debug.LogWarning($"'{handleName}' nicht gefunden — übersprungen.");
                continue;
            }

            // Prüfe ob schon ein HandGrab-Child existiert
            Transform existingGrab = handle.Find("[BuildingBlock] HandGrab");
            if (existingGrab != null)
            {
                Debug.Log($"'{handleName}' hat bereits ein HandGrab Child — übersprungen.");
                continue;
            }

            // Collider auf dem Handle (falls noch nicht vorhanden)
            Collider col = handle.GetComponent<Collider>();
            if (col == null)
            {
                BoxCollider box = Undo.AddComponent<BoxCollider>(handle.gameObject);
                box.isTrigger = true;
            }

            // [BuildingBlock] HandGrab Child unter dem Handle erstellen
            GameObject handGrabChild = new GameObject("[BuildingBlock] HandGrab");
            Undo.RegisterCreatedObjectUndo(handGrabChild, $"Create HandGrab on {handleName}");
            handGrabChild.transform.SetParent(handle, false);
            handGrabChild.transform.localPosition = Vector3.zero;
            handGrabChild.transform.localRotation = Quaternion.identity;
            handGrabChild.transform.localScale = Vector3.one;

            // Dieselben GUIDs wie bei den Logo-Würfeln
            AddComponentByGUID(handGrabChild, "5a1bc992571301d4a9602ac95ef4c71a"); // Grabbable
            AddComponentByGUID(handGrabChild, "e9a7676b01585ce43908639a27765dfc"); // HandGrabInteractable
            AddComponentByGUID(handGrabChild, "43f86b14a27b52f4f9298c33015b5c26"); // PointableElement

            Debug.Log($"HandGrab eingerichtet auf '{handleName}'!");
            setupCount++;
        }

        if (setupCount > 0)
        {
            Debug.Log($"Fertig! {setupCount} Handle(s) greifbar gemacht.");
            Debug.Log("WICHTIG: Im Inspector die Rigidbody-Referenz im HandGrabInteractable setzen!");
        }

        EditorUtility.SetDirty(selected);
    }

    static Transform FindChildRecursive(Transform parent, string name)
    {
        // Erst direkte Kinder
        Transform direct = parent.Find(name);
        if (direct != null) return direct;

        // Dann rekursiv
        foreach (Transform child in parent)
        {
            Transform found = FindChildRecursive(child, name);
            if (found != null) return found;
        }
        return null;
    }

    static void AddComponentByGUID(GameObject obj, string guid)
    {
        string path = AssetDatabase.GUIDToAssetPath(guid);
        if (string.IsNullOrEmpty(path))
        {
            Debug.LogWarning($"Script mit GUID {guid} nicht gefunden! Bitte manuell hinzufügen via BuildingBlock.");
            return;
        }

        MonoScript script = AssetDatabase.LoadAssetAtPath<MonoScript>(path);
        if (script == null)
        {
            Debug.LogWarning($"MonoScript konnte nicht geladen werden: {path}");
            return;
        }

        System.Type type = script.GetClass();
        if (type == null)
        {
            Debug.LogWarning($"Klasse nicht gefunden in Script: {path}");
            return;
        }

        obj.AddComponent(type);
    }
}
