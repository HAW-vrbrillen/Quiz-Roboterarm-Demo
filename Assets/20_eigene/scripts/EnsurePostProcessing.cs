
using UnityEngine;
using UnityEngine.Rendering.Universal;

/// <summary>
/// Stellt sicher, dass Post-Processing (Bloom) auf der VR-Kamera aktiv ist.
/// OVR SDK kann Kamera-Settings zur Laufzeit überschreiben —
/// dieses Script setzt renderPostProcessing + HDR in LateUpdate.
/// Auf das CenterEyeAnchor legen.
/// </summary>
[RequireComponent(typeof(Camera))]
public class EnsurePostProcessing : MonoBehaviour
{
    Camera cam;
    UniversalAdditionalCameraData camData;

    void Start()
    {
        cam = GetComponent<Camera>();
        camData = GetComponent<UniversalAdditionalCameraData>();
        Apply();
    }

    void LateUpdate()
    {
        Apply();
    }

    void Apply()
    {
        // Kein cam.allowHDR = true — HDR stört Passthrough auf Meta Quest!
        // Bloom funktioniert auch in LDR.
        if (camData != null)
            camData.renderPostProcessing = true;
    }
}
