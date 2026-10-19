using UnityEngine;

public class TeleportationWand : MonoBehaviour
{
    public GameObject ovrrig; // Referenz auf das OVR-Rig
    public LayerMask teleportationLayerMask; // LayerMask für mögliche Teleportationsziele
    public Transform rayOrigin; // Ursprung des Strahls (Spitze des Stabs)
    public GameObject teleportationRayPrefab; // Prefab für den visuellen Strahl
    public float maxRayDistance = 100f; // Maximale Länge des Strahls

    private GameObject teleportationRayInstance; // Instanz des visuellen Strahls
    private Vector3 targetTeleportationLocation; // Zielposition für die Teleportation
    private bool isRayActive = false; // Status, ob der Strahl aktiv ist

    // Funktion zum Aktivieren des Strahls
    public void ActivateRay()
    {
        if (!isRayActive)
        {
            teleportationRayInstance = Instantiate(teleportationRayPrefab, rayOrigin.position, Quaternion.identity, rayOrigin);
            isRayActive = true;
        }
    }

    // Funktion zum Deaktivieren des Strahls und Durchführen der Teleportation
    public void PerformTeleportation()
    {
        if (isRayActive && teleportationRayInstance != null)
        {
            ovrrig.transform.position = targetTeleportationLocation;
            Destroy(teleportationRayInstance);
            isRayActive = false;
        }
    }

    void Update()
    {
        // Aktualisiere den Strahl, wenn er aktiv ist
        if (isRayActive)
        {
            UpdateRay();
        }
    }

    void UpdateRay()
    {
        RaycastHit hit;
        if (Physics.Raycast(rayOrigin.position, rayOrigin.forward, out hit, maxRayDistance, teleportationLayerMask))
        {
            // Treffpunkt gefunden, aktualisiere die visuelle Darstellung des Strahls
            targetTeleportationLocation = hit.point;
            UpdateRayVisualization(rayOrigin.position, hit.point);
        }
        else
        {
            // Kein Treffpunkt, deaktiviere den Strahl
            if (teleportationRayInstance != null)
            {
                Destroy(teleportationRayInstance);
                isRayActive = false;
            }
        }
    }

    void UpdateRayVisualization(Vector3 start, Vector3 end)
    {
        // Passt die Position, Ausrichtung und Länge des visuellen Strahls an
        teleportationRayInstance.transform.position = start + (end - start) / 2f;
        teleportationRayInstance.transform.LookAt(end);
        teleportationRayInstance.transform.localScale = new Vector3(1, 1, (end - start).magnitude);
    }
}
