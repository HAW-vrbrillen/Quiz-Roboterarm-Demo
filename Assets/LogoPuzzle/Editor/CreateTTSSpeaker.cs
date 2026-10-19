using UnityEngine;
using UnityEditor;
using Meta.WitAi.TTS.Utilities;
using RIS;

/// <summary>
/// Editor-Script: Erzeugt ein TTS-Speaker GameObject mit allen nötigen Komponenten
/// und verdrahtet es automatisch mit dem LogoPuzzleSnap in der Szene.
///
/// Menü: Tools > TTS-Speaker fuer LogoPuzzle erstellen
/// </summary>
public class CreateTTSSpeaker : Editor
{
    [MenuItem("Tools/TTS-Speaker fuer LogoPuzzle erstellen", false, 20)]
    static void Create()
    {
        // Prüfe ob schon ein TTSSpeakerInput in der Szene existiert
        TTSSpeakerInput existingInput = Object.FindObjectOfType<TTSSpeakerInput>();
        if (existingInput != null)
        {
            WireToLogoPuzzle(existingInput);
            EditorUtility.DisplayDialog("TTS-Speaker gefunden",
                $"Es existiert bereits ein TTSSpeakerInput auf '{existingInput.gameObject.name}'.\n" +
                "Dieser wurde mit dem LogoPuzzleSnap verbunden.",
                "OK");
            return;
        }

        // --- Neues GameObject erstellen ---
        GameObject ttsSpeakerGO = new GameObject("TTS-Speaker");
        Undo.RegisterCreatedObjectUndo(ttsSpeakerGO, "Create TTS-Speaker");

        // 1. TTSSpeaker (Meta Voice SDK)
        TTSSpeaker speaker = ttsSpeakerGO.AddComponent<TTSSpeaker>();

        // 2. TextPoolRIS
        TextPoolRIS textPool = ttsSpeakerGO.AddComponent<TextPoolRIS>();

        // 3. TTSSpeakerInput
        TTSSpeakerInput speakerInput = ttsSpeakerGO.AddComponent<TTSSpeakerInput>();

        // TTSSpeakerInput._speaker per SerializedObject setzen (privates Feld)
        SerializedObject so = new SerializedObject(speakerInput);
        SerializedProperty speakerProp = so.FindProperty("_speaker");
        if (speakerProp != null)
        {
            speakerProp.objectReferenceValue = speaker;
            so.ApplyModifiedProperties();
        }

        // TextPoolRIS Referenz setzen
        speakerInput.textPoolRIS = textPool;

        // 4. AudioSource für TTS-Wiedergabe
        if (ttsSpeakerGO.GetComponent<AudioSource>() == null)
            ttsSpeakerGO.AddComponent<AudioSource>();

        // --- Mit LogoPuzzleSnap verbinden ---
        WireToLogoPuzzle(speakerInput);

        // WitConfiguration suchen und zuweisen
        WireWitConfiguration(speaker);

        Selection.activeGameObject = ttsSpeakerGO;
        EditorUtility.SetDirty(ttsSpeakerGO);

        Debug.Log("TTS-Speaker erstellt und mit LogoPuzzleSnap verbunden!");
        Debug.Log("WICHTIG: Prüfe ob eine WitConfiguration im TTSSpeaker zugewiesen ist (Window > Wit.ai > Settings).");
    }

    static void WireToLogoPuzzle(TTSSpeakerInput speakerInput)
    {
        LogoPuzzleSnap puzzle = Object.FindObjectOfType<LogoPuzzleSnap>();
        if (puzzle != null)
        {
            SerializedObject puzzleSO = new SerializedObject(puzzle);
            SerializedProperty ttsProp = puzzleSO.FindProperty("ttsSpeaker");
            if (ttsProp != null)
            {
                ttsProp.objectReferenceValue = speakerInput;
                puzzleSO.ApplyModifiedProperties();
                EditorUtility.SetDirty(puzzle);
                Debug.Log($"LogoPuzzleSnap.ttsSpeaker -> '{speakerInput.gameObject.name}' verbunden.");
            }
        }
        else
        {
            Debug.LogWarning("Kein LogoPuzzleSnap in der Szene gefunden! Bitte ttsSpeaker manuell zuweisen.");
        }
    }

    static void WireWitConfiguration(TTSSpeaker speaker)
    {
        string[] guids = AssetDatabase.FindAssets("t:WitConfiguration");
        if (guids.Length > 0)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
            var config = AssetDatabase.LoadAssetAtPath<ScriptableObject>(path);
            if (config != null)
            {
                SerializedObject so = new SerializedObject(speaker);
                SerializedProperty iter = so.GetIterator();
                bool found = false;
                while (iter.NextVisible(true))
                {
                    if (iter.propertyType == SerializedPropertyType.ObjectReference &&
                        iter.type.Contains("WitConfiguration"))
                    {
                        iter.objectReferenceValue = config;
                        found = true;
                        break;
                    }
                }
                if (found)
                {
                    so.ApplyModifiedProperties();
                    Debug.Log($"WitConfiguration '{path}' dem TTSSpeaker zugewiesen.");
                }
            }
        }
        else
        {
            Debug.LogWarning("Keine WitConfiguration gefunden! Bitte über Window > Wit.ai > Settings erstellen.");
        }
    }
}
