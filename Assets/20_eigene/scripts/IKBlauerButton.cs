using System.Collections;
using UnityEngine;

/// <summary>
/// Blauer "Bitte IK"-Button: Startet IK und blendet die Erklärungs-Tafel ein.
/// Wird automatisch neben dem roten IKButton_Red platziert.
/// </summary>
public class IKBlauerButton : MonoBehaviour
{
    [Header("Referenzen")]
    public URIKButton urikButton;
    public GameObject ikInfoTafel;

    [Header("Button Proximity")]
    public float pokeRadius = 0.04f;

    private bool buttonCooldown = false;
    private Transform leftFingertip;
    private Transform rightFingertip;
    private Renderer buttonRenderer;

    static readonly Color kNormal = new Color(0.10f, 0.30f, 0.90f);
    static readonly Color kFlash  = new Color(0.10f, 0.85f, 0.30f);

    void Start()
    {
        buttonRenderer = GetComponent<Renderer>();
        if (buttonRenderer != null)
            buttonRenderer.material.color = kNormal;

        // Auto-find
        if (urikButton == null)
            urikButton = FindObjectOfType<URIKButton>();

        // Fingertip-Transforms suchen
        foreach (var t in FindObjectsOfType<Transform>())
        {
            if (t.name != "HandIndexFingertip") continue;
            string path = GetPath(t);
            if (path.Contains("Left"))  leftFingertip  = t;
            else if (path.Contains("Right")) rightFingertip = t;
        }
    }

    string GetPath(Transform t)
    {
        string path = t.name;
        while (t.parent != null) { t = t.parent; path = t.name + "/" + path; }
        return path;
    }

    void Update()
    {
        if (buttonCooldown) return;

        bool poked = false;
        if (leftFingertip  != null && Vector3.Distance(transform.position, leftFingertip.position)  < pokeRadius) poked = true;
        if (!poked && rightFingertip != null && Vector3.Distance(transform.position, rightFingertip.position) < pokeRadius) poked = true;

        if (poked)
        {
            OnPoked();
            StartCoroutine(Cooldown());
        }
    }

    void OnPoked()
    {
        // Erklärungstafel einblenden
        if (ikInfoTafel != null)
            ikInfoTafel.SetActive(true);

        // IK starten
        if (urikButton != null)
            urikButton.StartIK();

        if (buttonRenderer != null)
            StartCoroutine(FlashGreen());

        Debug.Log("IKBlauerButton: IK gestartet + Tafel eingeblendet.");
    }

    IEnumerator FlashGreen()
    {
        buttonRenderer.material.color = kFlash;
        yield return new WaitForSeconds(0.4f);
        buttonRenderer.material.color = kNormal;
    }

    IEnumerator Cooldown()
    {
        buttonCooldown = true;
        yield return new WaitForSeconds(1.0f);
        buttonCooldown = false;
    }
}
