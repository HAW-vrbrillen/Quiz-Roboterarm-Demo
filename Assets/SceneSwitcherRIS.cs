using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    private List<string> sceneNames = new List<string>();
    private int currentIndex = 0;

    private void Start()
    {
        // Erstelle eine Liste der Szenennamen im Projekt
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string sceneName = System.IO.Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(i));
            sceneNames.Add(sceneName);
        }
    }

    public void SwitchToNextScene()
    {
        // Wechsle zur nächsten Szene in der Liste
        SceneManager.LoadScene(sceneNames[currentIndex]);

        // Inkrementiere den Index und setze ihn auf 0, wenn alle Szenen durchlaufen wurden
        currentIndex = (currentIndex + 1) % sceneNames.Count;
    }
}
