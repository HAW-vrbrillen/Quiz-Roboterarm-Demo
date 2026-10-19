using UnityEngine;

public class SwitchPassThrough : MonoBehaviour
{
    public GameObject passthroughLayer; // Stellen Sie sicher, dass dies im Unity-Editor zugewiesen ist

    private Camera centerCamera; // Variable für die Referenzierung der Kamera

    // Start wird vor dem ersten Frame-Update aufgerufen
    void Start()
    {
        // Versuchen Sie, die CenterCamera über den OVRCameraRig zu finden
        OVRCameraRig cameraRig = FindObjectOfType<OVRCameraRig>();
        if (cameraRig != null)
        {
            centerCamera = cameraRig.centerEyeAnchor.GetComponent<Camera>();
        }

        if (centerCamera == null)
        {
            Debug.LogError("CenterCamera wurde nicht gefunden!");
            return;
        }

        // Überprüfen, ob Passthrough empfohlen wird und entsprechend handeln
        EnablePassthrough(OVRManager.IsPassthroughRecommended());
    }

    // Methode zum Ein-/Ausschalten des Passthrough
    public void EnablePassthrough(bool enable)
    {
        if (passthroughLayer != null)
        {
            passthroughLayer.SetActive(enable);
        }
        else
        {
            Debug.LogError("PassthroughLayer ist nicht zugewiesen!");
        }

        // Kamera-Hintergrund entsprechend einstellen
        if (centerCamera != null)
        {
            if (enable)
            {
                centerCamera.clearFlags = CameraClearFlags.SolidColor;
                centerCamera.backgroundColor = Color.clear;
            }
            else
            {
                centerCamera.clearFlags = CameraClearFlags.Skybox;
            }
        }
        else
        {
            Debug.LogError("CenterCamera wurde nicht gefunden!");
        }

        // Aktivieren oder deaktivieren Sie weitere VR-Elemente je nach Zustand des Passthrough
        // ...
    }
}
