
#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

/// <summary>
/// Weist die 4 BoxCollider (3 Arm-Segmente + Wire) dem DresselWelcomePanel.glowZones Array zu.
/// </summary>
public static class AssignGlowZones
{
    [MenuItem("Tools/Assign Glow Zones")]
    public static void Assign()
    {
        // DresselPanel finden
        var panelGO = GameObject.Find("DresselPanel");
        if (panelGO == null)
        {
            Debug.LogError("AssignGlowZones: 'DresselPanel' nicht gefunden.");
            return;
        }

        var panel = panelGO.GetComponent<DresselWelcomePanel>();
        if (panel == null)
        {
            Debug.LogError("AssignGlowZones: DresselWelcomePanel-Komponente fehlt.");
            return;
        }

        // BoxLid-Objekte und Wire finden
        var names = new string[] {
            "L_Axis2Top_BoxLid",   // unterer langer Arm
            "U_Axis3Top_BoxLid",   // oberer langer Arm
            "Wire"                  // schwarzer Steuergriff
        };

        var colliders = new System.Collections.Generic.List<BoxCollider>();

        foreach (var name in names)
        {
            var go = GameObject.Find(name);
            if (go == null)
            {
                Debug.LogWarning($"AssignGlowZones: '{name}' nicht gefunden, übersprungen.");
                continue;
            }
            var bc = go.GetComponent<BoxCollider>();
            if (bc == null)
            {
                Debug.LogWarning($"AssignGlowZones: Kein BoxCollider auf '{name}', übersprungen.");
                continue;
            }
            colliders.Add(bc);
            Debug.Log($"AssignGlowZones: '{name}' BoxCollider hinzugefügt (center={bc.center}, size={bc.size})");
        }

        // Zuweisen
        var so = new SerializedObject(panel);
        var prop = so.FindProperty("glowZones");
        prop.arraySize = colliders.Count;
        for (int i = 0; i < colliders.Count; i++)
        {
            prop.GetArrayElementAtIndex(i).objectReferenceValue = colliders[i];
        }
        so.ApplyModifiedProperties();
        EditorUtility.SetDirty(panelGO);

        Debug.Log($"AssignGlowZones: {colliders.Count} BoxCollider zugewiesen.");
    }
}
#endif
