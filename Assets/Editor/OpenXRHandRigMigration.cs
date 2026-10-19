using System.Linq;
using Oculus.Interaction.Input;
using Oculus.Interaction.Locomotion;
using Oculus.Interaction.OVR.Editor.QuickActions;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

// Stellt Szenen, die mit Meta XR SDK v68/v69 (OVR-Hand-Skelett) gebaut wurden,
// auf das OpenXR-Hand-Skelett von SDK v201+ um:
//   1. OVRSkeleton/OVRMesh an den Hand-Tracking-Blöcken auf "OpenXR Hand" setzen
//   2. altes "[BuildingBlock] Interaction"-Rig löschen
//   3. neues Interaction-Rig über die Meta Quick Action einfügen
// Batch: Unity -batchmode -quit -executeMethod OpenXRHandRigMigration.MigrateBuildScenes
public static class OpenXRHandRigMigration
{
    private const string OldInteractionRigName = "[BuildingBlock] Interaction";

    [MenuItem("Tools/Girlsday/Hand-Rig auf OpenXR umstellen (alle Build-Szenen)")]
    public static void MigrateBuildScenes()
    {
        foreach (var scene in EditorBuildSettings.scenes)
        {
            MigrateScene(scene.path);
        }
    }

    public static void MigrateScene(string scenePath)
    {
        var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
        var cameraRig = Object.FindObjectsByType<OVRCameraRig>(FindObjectsInactive.Include, FindObjectsSortMode.None)
            .FirstOrDefault();
        if (cameraRig == null)
        {
            Debug.LogWarning($"[HandRigMigration] {scenePath}: kein OVRCameraRig gefunden, übersprungen.");
            return;
        }

        foreach (var skeleton in cameraRig.GetComponentsInChildren<OVRSkeleton>(true))
        {
            var so = new SerializedObject(skeleton);
            var type = so.FindProperty("_skeletonType");
            if (type.intValue == (int)OVRSkeleton.SkeletonType.HandLeft)
                type.intValue = (int)OVRSkeleton.SkeletonType.XRHandLeft;
            else if (type.intValue == (int)OVRSkeleton.SkeletonType.HandRight)
                type.intValue = (int)OVRSkeleton.SkeletonType.XRHandRight;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        foreach (var mesh in cameraRig.GetComponentsInChildren<OVRMesh>(true))
        {
            var so = new SerializedObject(mesh);
            var type = so.FindProperty("_meshType");
            if (type.intValue == (int)OVRMesh.MeshType.HandLeft)
                type.intValue = (int)OVRMesh.MeshType.XRHandLeft;
            else if (type.intValue == (int)OVRMesh.MeshType.HandRight)
                type.intValue = (int)OVRMesh.MeshType.XRHandRight;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        var oldRig = cameraRig.GetComponentsInChildren<Transform>(true)
            .FirstOrDefault(t => t.name == OldInteractionRigName);
        if (oldRig != null)
        {
            Object.DestroyImmediate(oldRig.gameObject);
        }

        if (Object.FindObjectsByType<OVRCameraRigRef>(FindObjectsInactive.Include, FindObjectsSortMode.None).Length == 0)
        {
            OVRQuickActionsAPI.AddOVRInteractionRig(generateAsEditableCopy: false);
        }

        RemoveDuplicateRigsAndLocomotion();

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        Debug.Log($"[HandRigMigration] {scenePath}: umgestellt.");
    }

    // Das Comprehensive Rig enthält einen Locomotor mit Schwerkraft. Ohne Boden-Collider
    // fällt der Spieler dann ins Leere (die Szene scheint nach oben wegzufliegen).
    [MenuItem("Tools/Girlsday/Fortbewegung abschalten + doppeltes Rig entfernen (alle Build-Szenen)")]
    public static void FixBuildScenes()
    {
        var activeScenePath = SceneManager.GetActiveScene().path;
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;

        foreach (var buildScene in EditorBuildSettings.scenes)
        {
            var scene = EditorSceneManager.OpenScene(buildScene.path, OpenSceneMode.Single);
            RemoveDuplicateRigsAndLocomotion();
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            Debug.Log($"[HandRigMigration] {buildScene.path}: Fortbewegung abgeschaltet.");
        }

        if (!string.IsNullOrEmpty(activeScenePath))
        {
            EditorSceneManager.OpenScene(activeScenePath, OpenSceneMode.Single);
        }
    }

    private static void RemoveDuplicateRigsAndLocomotion()
    {
        var rigs = Object.FindObjectsByType<OVRCameraRigRef>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var duplicate in rigs.Skip(1))
        {
            var root = PrefabUtility.GetOutermostPrefabInstanceRoot(duplicate.gameObject) ?? duplicate.gameObject;
            Debug.Log($"[HandRigMigration] Doppeltes Interaction-Rig entfernt: {root.name}");
            Object.DestroyImmediate(root);
        }

        foreach (var locomotor in Object.FindObjectsByType<FirstPersonLocomotor>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            var target = locomotor.transform;
            while (target.parent != null && target.name != "Locomotor")
            {
                target = target.parent;
            }
            if (target.name != "Locomotor") target = locomotor.transform;

            target.gameObject.SetActive(false);
            Debug.Log($"[HandRigMigration] Fortbewegung deaktiviert: {target.name}");
        }
    }
}
