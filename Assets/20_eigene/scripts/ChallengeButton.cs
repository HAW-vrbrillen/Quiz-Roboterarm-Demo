using System.Collections;
using UnityEngine;

/// <summary>
/// Orange Challenge-Button:
/// 1) Manuell den Roboter zum Ziel bewegen (Zeit messen)
/// 2) Roboter zurücksetzen
/// 3) IK fährt schneller zum Ziel
/// 4) Wirtschaftlichkeitstafel einblenden
/// </summary>
public class ChallengeButton : MonoBehaviour
{
    [Header("Referenzen")]
    public URIKButton    urikButton;
    public GameObject    wirtschaftsTafel;
    public ParticleSystem[] zielPartikel;   // dieselben Partikel wie beim IK

    [Header("Button Proximity")]
    public float pokeRadius = 0.04f;

    // ── Intern ──────────────────────────────────────────────────────────

    private bool   buttonCooldown;
    private bool   challengeActive;
    private float  manuelleZeit;
    private float  ikZeit;

    private Transform leftFingertip;
    private Transform rightFingertip;
    private Renderer  buttonRenderer;

    static readonly Color kNormal = new Color(1.0f, 0.55f, 0.05f);  // Orange
    static readonly Color kFlash  = new Color(0.10f, 0.85f, 0.30f);

    void Start()
    {
        buttonRenderer = GetComponent<Renderer>();
        if (buttonRenderer != null)
            buttonRenderer.material.color = kNormal;

        if (urikButton == null)
            urikButton = FindObjectOfType<URIKButton>();

        foreach (var t in FindObjectsOfType<Transform>())
        {
            if (t.name != "HandIndexFingertip") continue;
            string path = GetPath(t);
            if (path.Contains("Left"))       leftFingertip  = t;
            else if (path.Contains("Right")) rightFingertip = t;
        }
    }

    string GetPath(Transform t)
    {
        string p = t.name;
        while (t.parent != null) { t = t.parent; p = t.name + "/" + p; }
        return p;
    }

    void Update()
    {
        if (buttonCooldown || challengeActive) return;

        bool poked = false;
        if (leftFingertip  != null && Vector3.Distance(transform.position, leftFingertip.position)  < pokeRadius) poked = true;
        if (!poked && rightFingertip != null && Vector3.Distance(transform.position, rightFingertip.position) < pokeRadius) poked = true;

        if (poked)
        {
            StartCoroutine(ChallengeFlow());
            StartCoroutine(Cooldown());
        }
    }

    /// <summary>Wird vom DresselWelcomePanel per SendMessage aufgerufen.</summary>
    public void StartChallenge()
    {
        if (!challengeActive)
            StartCoroutine(ChallengeFlow());
    }

    // ════════════════════════════════════════════════════════════════════
    IEnumerator ChallengeFlow()
    {
        challengeActive = true;
        if (buttonRenderer != null)
            buttonRenderer.material.color = kFlash;

        // Sicherstellen, dass IK aus ist
        if (urikButton != null && urikButton.IsRunning)
            urikButton.StopIK();

        // Roboter einblenden (Fallback, falls Panel es nicht schon getan hat)
        if (urikButton != null && urikButton.roboterObjekte != null)
            foreach (var obj in urikButton.roboterObjekte)
                if (obj != null) obj.SetActive(true);

        // Arm in Ausgangsposition zurücksetzen bevor Challenge startet
        if (urikButton != null)
            yield return StartCoroutine(urikButton.ResetToInitial(80f));

        Debug.Log("ChallengeButton: Phase 1 — Manuell zum Ziel bewegen!");

        // ── Phase 1: Manuell ────────────────────────────────────────────
        float startTime = Time.time;
        bool partikelAusgeloest = false;

        // Warte bis Partikel spielt (= Ziel erreicht per Hand)
        while (!partikelAusgeloest)
        {
            if (zielPartikel != null)
            {
                foreach (var ps in zielPartikel)
                {
                    if (ps != null && ps.isPlaying)
                    {
                        partikelAusgeloest = true;
                        break;
                    }
                }
            }
            // Auch direkte Distanzprüfung als Fallback
            if (!partikelAusgeloest && urikButton != null &&
                urikButton.joint6_T_Axis6 != null && urikButton.ikTarget != null)
            {
                float dist = Vector3.Distance(
                    urikButton.joint6_T_Axis6.position,
                    urikButton.ikTarget.transform.position);
                if (dist < urikButton.threshold)
                {
                    partikelAusgeloest = true;
                    // Partikel manuell auslösen
                    if (zielPartikel != null)
                        foreach (var ps in zielPartikel)
                        {
                            if (ps == null) continue;
                            ps.gameObject.SetActive(true);
                            ps.Play();
                        }
                }
            }

            // Timeout 120s
            if (Time.time - startTime > 120f)
            {
                Debug.Log("ChallengeButton: Timeout — Challenge abgebrochen.");
                challengeActive = false;
                if (buttonRenderer != null) buttonRenderer.material.color = kNormal;
                yield break;
            }
            yield return null;
        }

        manuelleZeit = Time.time - startTime;
        Debug.Log($"ChallengeButton: Manuell geschafft in {manuelleZeit:F1}s");

        yield return new WaitForSeconds(2f);

        // ── Phase 2: Reset ──────────────────────────────────────────────
        Debug.Log("ChallengeButton: Roboter zurücksetzen...");
        if (urikButton != null)
            yield return StartCoroutine(urikButton.ResetToInitial(80f));

        yield return new WaitForSeconds(0.5f);

        // ── Phase 3: IK (schneller) ─────────────────────────────────────
        Debug.Log("ChallengeButton: IK-Challenge startet...");
        float ikStart = Time.time;
        if (urikButton != null)
            urikButton.StartChallengeIK();

        // Warte bis IK fertig (nicht mehr running)
        while (urikButton != null && urikButton.IsRunning)
            yield return null;

        ikZeit = Time.time - ikStart;
        Debug.Log($"ChallengeButton: IK geschafft in {ikZeit:F1}s");

        yield return new WaitForSeconds(1f);

        // ── Phase 4: Wirtschaftstafel ───────────────────────────────────
        if (wirtschaftsTafel != null)
        {
            wirtschaftsTafel.SetActive(true);
            var tafel = wirtschaftsTafel.GetComponent<WirtschaftsTafel>();
            if (tafel != null)
                tafel.SetZeiten(manuelleZeit, ikZeit);
        }

        if (buttonRenderer != null)
            buttonRenderer.material.color = kNormal;
        challengeActive = false;
    }

    IEnumerator Cooldown()
    {
        buttonCooldown = true;
        yield return new WaitForSeconds(1.5f);
        buttonCooldown = false;
    }
}
