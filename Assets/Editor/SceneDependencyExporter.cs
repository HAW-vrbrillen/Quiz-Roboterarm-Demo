using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class SceneDependencyExporter : EditorWindow
{
    private List<string> assetPaths;

    [MenuItem("Tools/Scene Dependency Exporter")]
    public static void ShowWindow()
    {
        GetWindow<SceneDependencyExporter>("Scene Dependency Exporter");
    }

    private void OnGUI()
    {
        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Find Scene Dependencies"))
        {
            FindDependencies();
        }

        if (GUILayout.Button("Export as Package"))
        {
            ExportPackage();
        }

        GUILayout.EndHorizontal();

        GUILayout.Space(10); // Adds some spacing between the buttons and the list

        if (assetPaths != null && assetPaths.Count > 0)
        {
            GUILayout.Label("Found " + assetPaths.Count + " dependencies:");

            foreach (string path in assetPaths)
            {
                GUILayout.Label(path);
            }
        }
    }

    private void FindDependencies()
    {
        // Get the active scene
        var activeScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        if (activeScene == null)
        {
            Debug.LogError("No active scene found!");
            return;
        }

        // Find all dependencies of the scene
        string[] scenePaths = new string[] { activeScene.path };
        string[] dependencies = AssetDatabase.GetDependencies(scenePaths, true);

        assetPaths = new List<string>(dependencies);
    }

    private void ExportPackage()
    {
        if (assetPaths == null || assetPaths.Count == 0)
        {
            Debug.LogError("No assets found to export!");
            return;
        }

        string exportPath = EditorUtility.SaveFilePanel("Save Package", "", "SceneDependencies.unitypackage", "unitypackage");

        if (!string.IsNullOrEmpty(exportPath))
        {
            AssetDatabase.ExportPackage(assetPaths.ToArray(), exportPath, ExportPackageOptions.Interactive);
            Debug.Log("Package exported to: " + exportPath);
        }
    }
}
