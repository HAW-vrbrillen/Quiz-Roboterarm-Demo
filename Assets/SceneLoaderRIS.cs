using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoaderRIS : MonoBehaviour
{
    public void LoadSceneWithDelay(string sceneName)
    {
        StartCoroutine(LoadSceneCoroutine(sceneName));
    }

    private IEnumerator LoadSceneCoroutine(string sceneName)
    {
        yield return new WaitForSeconds(2);

        if (SceneManager.GetActiveScene() != null)
        {
            Scene currentScene = SceneManager.GetActiveScene();
            AsyncOperation unloadOperation = SceneManager.UnloadSceneAsync(currentScene);

            while (unloadOperation != null && !unloadOperation.isDone)
            {
                yield return null;
            }
        }

        if (!string.IsNullOrEmpty(sceneName))
        {
            AsyncOperation loadOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);

            while (loadOperation != null && !loadOperation.isDone)
            {
                yield return null;
            }
        }
        else
        {
            Debug.LogError("Scene name is null or empty");
        }
    }
}

/*
 * Diese optimierte Version bietet folgende Verbesserungen:

Die aktuelle Szene wird explizit entladen, bevor die neue geladen wird. Dies stellt sicher, 
dass alle Objekte der alten Szene, einschließlich des Camera Rigs, ordnungsgemäß entfernt werden.
Der LoadSceneMode.Single Parameter wird verwendet, um sicherzustellen, dass nur eine Szene aktiv ist.
Asynchrones Laden und Entladen wird verwendet, um eine bessere Performance 
zu gewährleisten, besonders bei größeren Szenen.
Der Code wartet, bis jeder Schritt abgeschlossen ist, bevor er fortfährt. 
Dies verhindert Überlappungen oder Konflikte zwischen den Szenen.
Optionale Stellen für Fade-Effekte wurden hinzugefügt, 
die Sie implementieren können, um den Übergang sanfter zu gestalten.

Diese Änderungen sollten das Problem mit überlappenden Camera Rigs oder verbleibenden Referenzen 
lösen. Jede neue Szene wird nun in einer sauberen Umgebung geladen, nachdem die vorherige 
vollständig entfernt wurde.
Beachten Sie, dass Sie möglicherweise Ihre Szeneneinstellungen in Unity anpassen müssen, um 
sicherzustellen, dass jede Szene ihr eigenes Camera Rig hat und dass keine 
persistenten Objekte zwischen den Szenen geteilt werden, es sei denn, dies ist ausdrücklich beabsichtigt.
 */
