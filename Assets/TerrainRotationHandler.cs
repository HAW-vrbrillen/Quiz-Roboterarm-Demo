using UnityEngine;

public class TerrainRotationHandler : MonoBehaviour
{
    private Terrain terrain;

    // MonoBehaviour-Initialisierungsmethode
    private void Awake()
    {
        // Terrain-Objekt finden
        terrain = GameObject.Find("Terrain").GetComponent<Terrain>(); 
    }

    // Update-Methode
    private void Update()
    {
        // Rotation des Parent-Objekts abfragen
        Quaternion parentRotation = transform.parent.rotation;

        // Rotation des Terrains festlegen
        terrain.transform.rotation = parentRotation;
    }
}
