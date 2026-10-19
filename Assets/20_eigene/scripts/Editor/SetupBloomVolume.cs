
#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEditor;

/// <summary>
/// Einmal-Editor-Tool: Erstellt ein VolumeProfile mit Bloom und weist es dem
/// "Post Processing Volume" GameObject zu. Danach kann das Script gelöscht werden.
/// </summary>
public static class SetupBloomVolume
{
    [MenuItem("Tools/Setup Bloom Volume")]
    public static void Setup()
    {
        // Volume GameObject finden
        var volumeGO = GameObject.Find("Post Processing Volume");
        if (volumeGO == null)
        {
            Debug.LogError("SetupBloomVolume: 'Post Processing Volume' nicht gefunden.");
            return;
        }

        var volume = volumeGO.GetComponent<Volume>();
        if (volume == null)
        {
            Debug.LogError("SetupBloomVolume: Volume-Komponente fehlt.");
            return;
        }

        // VolumeProfile als Asset erstellen
        string folder = "Assets/20_eigene/Settings";
        if (!AssetDatabase.IsValidFolder(folder))
            AssetDatabase.CreateFolder("Assets/20_eigene", "Settings");

        var profile = ScriptableObject.CreateInstance<VolumeProfile>();
        string profilePath = folder + "/BloomProfile.asset";
        AssetDatabase.CreateAsset(profile, profilePath);

        // Bloom Override hinzufügen
        var bloom = profile.Add<Bloom>(true);
        bloom.threshold.value = 0.9f;
        bloom.threshold.overrideState = true;
        bloom.intensity.value = 0.5f;
        bloom.intensity.overrideState = true;
        bloom.scatter.value = 0.7f;
        bloom.scatter.overrideState = true;

        EditorUtility.SetDirty(profile);
        AssetDatabase.SaveAssets();

        // Dem Volume zuweisen
        volume.isGlobal = true;
        volume.profile = profile;
        volume.priority = 1;

        EditorUtility.SetDirty(volumeGO);
        Debug.Log($"SetupBloomVolume: Bloom-Profile erstellt ({profilePath}) und zugewiesen. Threshold=0.9, Intensity=0.5, Scatter=0.7");
    }
}
#endif
