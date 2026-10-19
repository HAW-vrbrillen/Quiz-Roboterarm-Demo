using UnityEngine;

public class PositionAdjuster : MonoBehaviour
{
    [Header("Funktion: PositionTargetObject")]
    public Transform playerTransform; // Der Spieler, der sich zum Ziel bewgen soll. ( zu dem sich das Zielobjekt bewegen soll
    public Transform targetObject; // Das Zielobjekt, das links vom Spieler positioniert werden soll
    private Transform A330300interior;

    void Start()
    {
        // Initialisierung kann hier erfolgen, falls nötig
        A330300interior = GameObject.Find("A330-300Interior").transform;
    }
    public void PositionTargetObject()
    {
        StopAllAudio();
        Vector3 targetPositionNoChangeinY = new Vector3(targetObject.position.x, 
            playerTransform.position.y, targetObject.position.z);
        //A330300interior.position = targetPositionNoChangeinY;
        playerTransform.position = targetPositionNoChangeinY;

        // Übernehme die Rotation des Zielobjekts auf den Spieler
        playerTransform.rotation = targetObject.rotation;
        



    }

    void StopAllAudio()
    {
        // Find all AudioSource components in the scene
        AudioSource[] allAudioSources = FindObjectsOfType<AudioSource>();

        // Iterate through each AudioSource and stop it
        foreach (AudioSource audioSource in allAudioSources)
        {
            audioSource.Stop();
        }

        Debug.Log("All audio sources stopped.");
    }



    /* OLD version nur kabine bewegen.. NEU OVR bewegen
    public void PositionTargetObject()
    {
        if (playerTransform == null || targetObject == null)
        {
            Debug.LogError("PlayerTransform oder TargetObject nicht zugewiesen.");
            return;
        }

        // Berechne die neue Position des Zielobjekts, so dass nur die Z-Achse verändert wird.
        // Wir behalten die X- und Y-Position des Zielobjekts bei und passen nur die Z-Position an.
        Vector3 targetPositionWithZChangeOnly = new Vector3(targetObject.position.x, targetObject.position.y, playerTransform.position.z);

        // Berechne die notwendige Verschiebung des Top-Parents, um das Zielobjekt an die gewünschte Position zu bringen
        // Dazu verwenden wir die Differenz zwischen der gewünschten Position des Zielobjekts und seiner aktuellen Position
        Vector3 currentTargetObjectPositionInWorld = targetObject.position;
        Vector3 requiredShiftForTopParent = targetPositionWithZChangeOnly - currentTargetObjectPositionInWorld;

        // Wende die berechnete Verschiebung auf den Top-Parent an
        A330300interior.position += requiredShiftForTopParent;
    }
    */
}
