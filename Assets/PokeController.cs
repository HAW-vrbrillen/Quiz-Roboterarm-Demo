using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PokeController : MonoBehaviour
{
    private GameObject pokeSpherePrefab;
    public List<GameObject> targetObjects = new List<GameObject>();
    public List<KeyCode> activationKeys = new List<KeyCode>();
    private Dictionary<GameObject, Vector3> originalPositions = new Dictionary<GameObject, Vector3>();

    void Start()
    {
        // Lade das Prefab
        pokeSpherePrefab = Resources.Load<GameObject>("Rise-PokeSphere");
        if (pokeSpherePrefab == null)
        {
            Debug.LogError("Prefab konnte nicht gefunden werden. Stellen Sie sicher, dass es im richtigen Ordner liegt.");
            return;
        }

        // Instanziiere Prefabs für jedes Zielobjekt und speichere ihre ursprünglichen Positionen
        foreach (var target in targetObjects)
        {
            var instance = Instantiate(pokeSpherePrefab, target.transform.position - new Vector3(0, 0, 0.01f), Quaternion.identity, target.transform);
            originalPositions[target] = instance.transform.position;
        }
    }

    void Update()
    {
        for (int i = 0; i < activationKeys.Count; i++)
        {
            if (Input.GetKeyDown(activationKeys[i]) && i < targetObjects.Count)
            {
                StartCoroutine(MovePokeSphere(targetObjects[i], originalPositions[targetObjects[i]]));
            }
        }
    }

    IEnumerator MovePokeSphere(GameObject targetObject, Vector3 originalPosition)
    {
        float moveDistance = 0.5f; // Distanz, die das Objekt sich bewegen soll
        GameObject sphere = targetObject.transform.GetChild(0).gameObject; // Gehe davon aus, dass die Sphere das erste Kind ist

        // Bewege die Sphere nach vorne
        while (Vector3.Distance(sphere.transform.position, originalPosition + new Vector3(0, 0, moveDistance)) > 0.1f)
        {
            sphere.transform.position = Vector3.MoveTowards(sphere.transform.position, originalPosition + new Vector3(0, 0, moveDistance), 20f * Time.deltaTime);
            yield return null;
        }

        // Kurze Pause
        yield return new WaitForSeconds(0.5f);

        // Bewege die Sphere zurück zu ihrer ursprünglichen Position
        while (Vector3.Distance(sphere.transform.position, originalPosition) > 0.01f)
        {
            sphere.transform.position = Vector3.MoveTowards(sphere.transform.position, originalPosition, 20f * Time.deltaTime);
            yield return null;
        }
    }
}
