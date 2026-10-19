using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using Oculus.Interaction;

/// <summary>
/// Step-Controller für das Dressel Welcome-Panel.
/// Alle visuellen Objekte sind Scene-GameObjects (3D Cubes + TMP).
/// Findet Kinder per Name: "InhaltText", "ButtonLabelText", "PokeButton".
/// Nutzt PokeInteractable Events zum Weiterschalten.
///
/// Ablauf:
/// Step 0: Begrüßung
/// Step 1: Challenge 1 — Logo Puzzle (wartet auf Puzzle + Audio-Ansage)
/// Step 2: "Nun Challenge 2 starten..." Button → Roboter erscheint, Ausgangsposition, Glow
/// Step 3: Roboter erkunden ("Bitte bewegen Sie...") → Button "Challenge starten!"
/// </summary>
public class DresselWelcomePanel : MonoBehaviour
{
    [Header("Challenge")]
    public ChallengeButton challengeButton;

    [Header("Logo Puzzle")]
    public LogoPuzzleSnap logoPuzzleSnap;

    [Header("Roboter")]
    [Tooltip("Roboter-Objekte die bei Challenge 2 eingeblendet werden")]
    public GameObject[] roboterObjekte;

    [Tooltip("BoxCollider der greifbaren Zonen (Arm-Segmente + Wire)")]
    public BoxCollider[] glowZones;

    [Tooltip("Glow-Farbe (Grün, halbtransparent)")]
    public Color glowColor = new Color(0f, 1f, 0.3f, 0.35f);

    [Tooltip("Puls-Geschwindigkeit (Zyklen pro Sekunde)")]
    public float glowPulseSpeed = 1.5f;

    [Header("Roboter-Reset")]
    [Tooltip("URIKButton für Arm-Reset in Ausgangsposition")]
    public URIKButton urikButton;

    // ── Intern ──────────────────────────────────────────────────────
    private int currentStep;
    private TMP_Text inhaltText;
    private TMP_Text buttonLabelText;
    private PokeInteractable pokeInteractable;
    private bool waitingForPuzzle;
    private bool challengeRunning;

    static readonly string[] stepTexte = new string[]
    {
        // Step 0: Begrüßung
        "Herzlich willkommen!\n\n" +
        "Heute warten zwei spannende\n" +
        "Challenges auf Sie:\n\n" +
        "1. Greifen üben\n" +
        "2. Roboter verstehen & steuern\n\n" +
        "Los geht's!",

        // Step 1: Challenge 1 — Greifen üben (Logo Puzzle)
        "Challenge 1: Greifen üben!\n\n" +
        "Gucken Sie bitte einmal in die\n" +
        "viktorianische Box.\n\n" +
        "Sie sehen drei Würfel zum\n" +
        "Thema Krise im Kongo.\n\n" +
        "Greifen Sie jeden Würfel mit\n" +
        "der Hand und setzen Sie ihn\n" +
        "in die durchsichtige Zielbox\n" +
        "mit der passenden Beschreibung.\n\n" +
        "Tipp: Daumen und Zeigefinger\n" +
        "zusammenführen zum Greifen.",

        // Step 2: Challenge 2 Intro — Freiheitsgrade & IK
        "Sehr gut! Weiter zu Challenge 2:\n\n" +
        "Ein Industrieroboter hat\n6 Freiheitsgrade — wie ein Arm.\n\n" +
        "Erleben Sie, was Automatisierung\n" +
        "mit inverser Kinematik erreichen\n" +
        "kann — und verstehen Sie den\n" +
        "wirtschaftlichen Vorteil!",

        // Step 3: Roboter erkunden + Challenge starten
        "Bitte bewegen Sie einmal den\n" +
        "Roboter am Stab, am unteren\n" +
        "und am oberen Arm.\n\n" +
        "Die leuchtenden Teile können\n" +
        "Sie greifen und bewegen.\n\n" +
        "Wenn Sie bereit sind, starten\n" +
        "Sie die Challenge!",

        // Step 4: Challenge läuft
        "Roboter vs. Mensch!\n\n" +
        "Greifen Sie den Roboterarm\n" +
        "und führen Sie ihn zum Ziel.\n\n" +
        "Danach zeigt der Algorithmus,\n" +
        "wie schnell er das gleiche schafft."
    };

    static readonly string[] buttonLabels = new string[]
    {
        "Zur Challenge 1",
        "Challenge 1 starten",
        "Weiter zu Challenge 2",
        "Wettbewerb starten!",
        ""
    };

    void Start()
    {
        // Referenzen per Name in Kindern finden
        var inhaltGO = transform.Find("InhaltText");
        if (inhaltGO != null) inhaltText = inhaltGO.GetComponent<TMP_Text>();

        var btnLabelGO = transform.Find("ButtonLabelText");
        if (btnLabelGO != null) buttonLabelText = btnLabelGO.GetComponent<TMP_Text>();

        // PokeInteractable im Kind suchen
        foreach (var poke in GetComponentsInChildren<PokeInteractable>())
        {
            pokeInteractable = poke;
            break;
        }

        if (challengeButton == null)
            challengeButton = FindObjectOfType<ChallengeButton>();

        if (logoPuzzleSnap == null)
            logoPuzzleSnap = FindObjectOfType<LogoPuzzleSnap>();

        if (urikButton == null)
            urikButton = FindObjectOfType<URIKButton>();

        if (pokeInteractable != null)
            pokeInteractable.WhenSelectingInteractorAdded.Action += OnPoked;

        // Roboter initial verstecken
        SetRoboterVisible(false);

        currentStep = 0;
        UpdateContent();
    }

    void OnDestroy()
    {
        if (pokeInteractable != null)
            pokeInteractable.WhenSelectingInteractorAdded.Action -= OnPoked;
    }

    void Update()
    {
        // Auto-Advance: Puzzle fertig → weiter zu Step 2 (auch ohne Button-Druck)
        if (currentStep <= 1 && logoPuzzleSnap != null && logoPuzzleSnap.AudioAnnouncementDone)
        {
            waitingForPuzzle = false;
            currentStep = 2;
            UpdateContent();
        }
    }

    void OnPoked(PokeInteractor interactor)
    {
        NextStep();
    }

    void NextStep()
    {
        // Step 0 (Begrüßung): Weiter zu Step 1 (Logo Puzzle)
        if (currentStep == 0)
        {
            currentStep = 1;
            UpdateContent();
            return;
        }

        // Step 1 (Logo Puzzle): Warte auf Puzzle-Lösung + Audio
        if (currentStep == 1)
        {
            if (logoPuzzleSnap != null && !logoPuzzleSnap.AudioAnnouncementDone)
            {
                waitingForPuzzle = true;
                if (inhaltText != null)
                    inhaltText.text = "Platzieren Sie alle Würfel\nin die passenden Boxen.\n\n" +
                                      "Das Panel schaltet automatisch\nweiter, sobald alle Würfel\nrichtig sitzen.";
                if (buttonLabelText != null)
                    buttonLabelText.text = "\u23f3  Warte auf Würfel...";
                return;
            }
            // Puzzle + Audio schon fertig → direkt weiter
            currentStep = 2;
            UpdateContent();
            return;
        }

        // Step 2 ("Nun Challenge 2 starten..."): Roboter einblenden + Ausgangsposition + Glow
        if (currentStep == 2)
        {
            currentStep = 3;
            ShowRoboterAndPrepare();
            UpdateContent();
            return;
        }

        // Step 3 ("Challenge starten!"): Challenge über Panel-Coroutine starten
        if (currentStep == 3)
        {
            if (challengeRunning) return;
            currentStep = 4;
            SetGlow(false);
            UpdateContent();

            // URIKButton-GameObject aktivieren (Renderer/Collider bereits deaktiviert)
            // damit StartChallengeIK() → StartCoroutine(RunIK()) funktioniert
            if (urikButton != null)
                urikButton.gameObject.SetActive(true);

            StartCoroutine(ChallengeFlow());
            return;
        }
    }

    /// <summary>Roboter einblenden, in Ausgangsposition fahren, greifbare Teile glühen lassen.</summary>
    void ShowRoboterAndPrepare()
    {
        // Roboter sichtbar machen
        SetRoboterVisible(true);

        // Logo-Würfel aus Challenge 1 ausblenden, damit sie nicht über dem Knopf hängen
        SetLogoWuerfelVisible(false);

        // Arm in gerade Ausgangsposition zurückfahren
        if (urikButton != null)
        {
            // IK stoppen falls aktiv
            if (urikButton.IsRunning)
                urikButton.StopIK();
            StartCoroutine(urikButton.ResetToInitial(80f));
        }

        // Greifbare Teile leicht glühen lassen
        SetGlow(true);

        Debug.Log("DresselWelcomePanel: Roboter eingeblendet, Ausgangsposition, Glow aktiv.");
    }

    // ════════════════════════════════════════════════════════════════
    // Challenge-Flow — läuft komplett über das Hand-Panel
    // ════════════════════════════════════════════════════════════════
    IEnumerator ChallengeFlow()
    {
        challengeRunning = true;

        // Referenzen vom (inaktiven) ChallengeButton holen
        ParticleSystem[] zielPartikel = challengeButton != null ? challengeButton.zielPartikel : null;
        GameObject wirtschaftsTafelGO = challengeButton != null ? challengeButton.wirtschaftsTafel : null;

        // IK stoppen falls aktiv
        if (urikButton != null && urikButton.IsRunning)
            urikButton.StopIK();

        // ── Roboter in Ausgangsposition ───────────────────────────────
        if (inhaltText != null)
            inhaltText.text = "Roboter fährt in\nAusgangsposition...";

        if (urikButton != null)
            yield return StartCoroutine(urikButton.ResetToInitial(80f));

        // ── Phase 1: Manuell zum Ziel ─────────────────────────────────
        // Mit Werkzeugspitze: Die Spitze muss den roten Knopf (urikButton) berühren.
        // Ohne: wie bisher Achse 6 auf 10 cm an das IK-Ziel (großzügig für VR-Steuerung).
        bool mitSpitze = urikButton != null && urikButton.werkzeugSpitze != null;
        const float manualThreshold = 0.10f;

        if (inhaltText != null)
            inhaltText.text = mitSpitze
                ? "Los geht's!\n\nFühren Sie die Spitze des\nRoboters auf den roten Knopf.\n\nDie Zeit läuft..."
                : "Los geht's!\n\nBewegen Sie den Roboterarm\nzum HAW-Logo.\n\nDie Zeit läuft...";

        Debug.Log($"DresselWelcomePanel: Phase 1 gestartet (mitSpitze={mitSpitze}, manualThreshold={manualThreshold}m)");

        float startTime = Time.time;
        bool zielErreicht = false;

        while (!zielErreicht)
        {
            // Partikel-Check (falls extern ausgelöst)
            if (zielPartikel != null)
                foreach (var ps in zielPartikel)
                    if (ps != null && ps.isPlaying) { zielErreicht = true; break; }

            // Berührungs- bzw. Distanz-Check
            if (!zielErreicht && urikButton != null &&
                urikButton.joint6_T_Axis6 != null && urikButton.ikTarget != null)
            {
                bool erreicht;
                float dist;
                if (mitSpitze)
                {
                    erreicht = SpitzeBeruehrtKnopf(out dist);
                }
                else
                {
                    dist = Vector3.Distance(
                        urikButton.joint6_T_Axis6.position,
                        urikButton.ikTarget.transform.position);
                    erreicht = dist < manualThreshold;
                }
                if (erreicht)
                {
                    Debug.Log($"DresselWelcomePanel: Ziel manuell erreicht! Distanz={dist:F3}m");
                    zielErreicht = true;
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
                Debug.Log("DresselWelcomePanel: Challenge Timeout.");
                if (inhaltText != null)
                    inhaltText.text = "Zeitüberschreitung.\nVersuchen Sie es erneut.";
                challengeRunning = false;
                yield break;
            }
            yield return null;
        }

        float manuelleZeit = Time.time - startTime;
        Debug.Log($"DresselWelcomePanel: Manuell geschafft in {manuelleZeit:F1}s");

        if (inhaltText != null)
            inhaltText.text = $"Geschafft in {manuelleZeit:F1}s!\n\nRoboter wird zurückgesetzt...";

        yield return new WaitForSeconds(2f);

        // ── Phase 2: Reset ────────────────────────────────────────────
        Debug.Log("DresselWelcomePanel: Phase 2 — Roboter zurücksetzen...");
        if (inhaltText != null)
            inhaltText.text = "Roboter wird zurückgesetzt...";

        if (urikButton != null)
            yield return StartCoroutine(urikButton.ResetToInitial(80f));

        Debug.Log("DresselWelcomePanel: Phase 2 — Reset abgeschlossen.");
        yield return new WaitForSeconds(0.5f);

        // ── Phase 3: IK (schneller) ───────────────────────────────────
        Debug.Log("DresselWelcomePanel: Phase 3 — IK-Challenge startet...");
        if (inhaltText != null)
            inhaltText.text = "Jetzt der Roboter!\n\nInverse Kinematik\nberechnet den Weg...";

        float ikStart = Time.time;
        if (urikButton != null)
            urikButton.StartChallengeIK();

        while (urikButton != null && urikButton.IsRunning)
            yield return null;

        float ikZeit = Time.time - ikStart;
        Debug.Log($"DresselWelcomePanel: Phase 3 — IK geschafft in {ikZeit:F1}s");

        yield return new WaitForSeconds(1f);

        // ── Phase 4: Ergebnis + Vollkostenrechnung auf Hand-Panel ─────
        float faktor = ikZeit > 0 ? manuelleZeit / ikZeit : 0f;

        // Vollkostenrechnung (DE 2024/25)
        const float invest      = 35000f;  // EUR UR5e + Greifer + Integration
        const float afaJahr     = 7000f;   // 35.000/5 Jahre
        const float betriebJahr = 3000f;   // Strom+Wartung+Verschleiß+Vers.
        const float kalkZins    = 700f;    // 4% auf Ø-Kapitalbindung
        const float robotJahr   = afaJahr + betriebJahr + kalkZins; // 10.700
        const float menschH     = 38f;     // Vollkosten/h Facharbeiter
        const float menschJahr  = menschH * 1760f;                  // 66.880
        float einsparung = menschJahr - robotJahr;                   // 56.180
        float amortMon   = (invest / (menschJahr - betriebJahr - kalkZins)) * 12f;

        if (inhaltText != null)
            inhaltText.text =
                $"<b>Zeitmessung</b>\n" +
                $"Sie: {manuelleZeit:F1}s  |  Roboter: {ikZeit:F1}s\n" +
                $"\u2192 <b>{faktor:F1}x schneller</b>\n\n" +
                $"<b>Vollkostenrechnung</b>\n" +
                $"UR5e Invest.: {invest:N0}\u20ac\n" +
                $"AfA 5J: {afaJahr:N0}\u20ac/J | Betrieb: {betriebJahr:N0}\u20ac/J\n" +
                $"Kalk. Zins 4%: {kalkZins:N0}\u20ac/J\n" +
                $"<color=#26D94E>Roboter gesamt: {robotJahr:N0}\u20ac/J</color>\n\n" +
                $"Facharbeiter: {menschH:F0}\u20ac/h \u00d7 1.760h\n" +
                $"<color=#E64033>Mensch gesamt: {menschJahr:N0}\u20ac/J</color>\n\n" +
                $"<b><color=#26D94E>Einsparung: {einsparung:N0}\u20ac/J</color></b>\n" +
                $"<b><color=#4D99FF>Amortisation: ~{amortMon:F0} Monate</color></b>";

        challengeRunning = false;
    }

    void SetRoboterVisible(bool visible)
    {
        if (roboterObjekte == null) return;
        foreach (var obj in roboterObjekte)
        {
            if (obj != null) obj.SetActive(visible);
        }
    }

    /// <summary>
    /// True, wenn die Werkzeugspitze auf der Oberseite des roten Knopfes (urikButton) liegt:
    /// über der (runden) Knopffläche mit 1 cm Rand und höchstens 2 cm darüber.
    /// Maße kommen aus dem Mesh des Knopfes (Unity-Zylinder: Oberseite lokal bei y = 1).
    /// </summary>
    bool SpitzeBeruehrtKnopf(out float abstandZurOberseite)
    {
        abstandZurOberseite = float.MaxValue;
        Transform knopf = urikButton.transform;
        var mf = knopf.GetComponent<MeshFilter>();
        if (mf == null || mf.sharedMesh == null) return false;

        Bounds b = mf.sharedMesh.bounds;
        Vector3 skala = knopf.lossyScale;
        Vector3 lokal = knopf.InverseTransformPoint(urikButton.werkzeugSpitze.position) - b.center;

        const float rand = 0.01f;        // m seitlich über die Kante hinaus
        const float maxDarueber = 0.02f; // m über der Oberseite
        const float maxDarin = 0.05f;    // m unter der Oberseite (Spitze taucht ein)

        float rx = b.extents.x * skala.x + rand;
        float rz = b.extents.z * skala.z + rand;
        float x = lokal.x * skala.x, z = lokal.z * skala.z;
        bool ueberFlaeche = (x * x) / (rx * rx) + (z * z) / (rz * rz) <= 1f;
        abstandZurOberseite = (lokal.y - b.extents.y) * skala.y;
        if (ueberFlaeche && abstandZurOberseite <= maxDarueber && abstandZurOberseite >= -maxDarin)
            return true;

        // Die Werkzeugspitze ist ein unsichtbarer Punkt am Flansch. Von Hand berührt man den
        // Knopf meist mit dem Rand des Flanschs – deshalb zählt auch, wenn ein sichtbares Teil
        // am Ende des Arms (Achse 6 samt Anbauteilen) den Knopf berührt.
        return EndeBeruehrtKnopf();
    }

    const float kBeruehrToleranz = 0.02f; // m

    bool EndeBeruehrtKnopf()
    {
        var knopfRenderer = urikButton.GetComponent<Renderer>();
        if (knopfRenderer == null || urikButton.joint6_T_Axis6 == null) return false;

        Bounds knopf = knopfRenderer.bounds;
        knopf.Expand(kBeruehrToleranz * 2f);
        foreach (var r in urikButton.joint6_T_Axis6.GetComponentsInChildren<Renderer>())
        {
            if (r.enabled && knopf.Intersects(r.bounds))
                return true;
        }
        return false;
    }

    void SetLogoWuerfelVisible(bool visible)
    {
        if (logoPuzzleSnap == null || logoPuzzleSnap.puzzlePieces == null) return;
        foreach (var piece in logoPuzzleSnap.puzzlePieces)
        {
            if (piece != null) piece.gameObject.SetActive(visible);
        }
    }

    // ── Glow-Overlay-System: transparente pulsierende Cubes über den BoxCollidern ──
    // WICHTIG: Overlays sind NICHT Kinder der Grab-Objekte, damit GrabInteractable
    // deren Collider nicht cached → kein MissingReferenceException beim Aufräumen.
    private List<GameObject> glowOverlays = new List<GameObject>();
    private List<BoxCollider> glowOverlaySources = new List<BoxCollider>();
    private Coroutine glowPulseCoroutine;

    void SetGlow(bool on)
    {
        if (on)
            StartGlow();
        else
            StopGlow();
    }

    void StartGlow()
    {
        StopGlow(); // aufräumen falls schon aktiv

        if (glowZones == null) return;

        foreach (var col in glowZones)
        {
            if (col == null) continue;

            // Cube-Primitive als Overlay erstellen
            var overlay = GameObject.CreatePrimitive(PrimitiveType.Cube);
            overlay.name = "GlowOverlay_" + col.name;

            // Collider komplett entfernen — Overlay ist KEIN Kind eines Grab-Objekts,
            // also kann DestroyImmediate hier sicher verwendet werden
            var overlayCol = overlay.GetComponent<Collider>();
            if (overlayCol != null) DestroyImmediate(overlayCol);

            // NICHT als Kind der BoxCollider-Objekte einhängen!
            // Stattdessen Position per Coroutine nachführen.
            // So entdeckt GrabInteractable den Overlay-Collider nie.
            UpdateOverlayTransform(overlay.transform, col);

            // Transparentes grünes Material (URP Lit)
            var mr = overlay.GetComponent<MeshRenderer>();
            var mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            mat.SetFloat("_Surface", 1); // Transparent
            mat.SetFloat("_Blend", 0);
            mat.SetFloat("_SrcBlend", (float)UnityEngine.Rendering.BlendMode.SrcAlpha);
            mat.SetFloat("_DstBlend", (float)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            mat.SetFloat("_ZWrite", 0);
            mat.SetFloat("_AlphaClip", 0);
            mat.renderQueue = 3000;
            mat.SetOverrideTag("RenderType", "Transparent");
            mat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
            mat.EnableKeyword("_EMISSION");
            mat.SetColor("_BaseColor", glowColor);
            mat.SetColor("_EmissionColor", new Color(glowColor.r, glowColor.g, glowColor.b) * 2f);
            mr.material = mat;
            mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            mr.receiveShadows = false;

            glowOverlays.Add(overlay);
            glowOverlaySources.Add(col);
        }

        // Puls-Coroutine starten
        if (glowOverlays.Count > 0)
            glowPulseCoroutine = StartCoroutine(PulseGlow());
    }

    void StopGlow()
    {
        if (glowPulseCoroutine != null)
        {
            StopCoroutine(glowPulseCoroutine);
            glowPulseCoroutine = null;
        }
        foreach (var overlay in glowOverlays)
        {
            if (overlay != null) Destroy(overlay);
        }
        glowOverlays.Clear();
        glowOverlaySources.Clear();
    }

    /// <summary>Overlay-Cube auf Welt-Position/Rotation/Skalierung des BoxColliders setzen.</summary>
    void UpdateOverlayTransform(Transform overlay, BoxCollider col)
    {
        Transform src = col.transform;
        overlay.position = src.TransformPoint(col.center);
        overlay.rotation = src.rotation;
        // BoxCollider.size ist lokal → mit lossyScale skalieren
        Vector3 ls = src.lossyScale;
        overlay.localScale = new Vector3(
            col.size.x * ls.x,
            col.size.y * ls.y,
            col.size.z * ls.z);
    }

    IEnumerator PulseGlow()
    {
        while (true)
        {
            // Sinuswelle 0→1→0 für sanftes Pulsieren
            float t = (Mathf.Sin(Time.time * glowPulseSpeed * 2f * Mathf.PI) + 1f) * 0.5f;
            float alpha = Mathf.Lerp(0.1f, glowColor.a, t);
            Color c = glowColor;
            c.a = alpha;

            for (int i = 0; i < glowOverlays.Count; i++)
            {
                var overlay = glowOverlays[i];
                if (overlay == null) continue;

                // Position nachführen (Arm bewegt sich)
                if (i < glowOverlaySources.Count && glowOverlaySources[i] != null)
                    UpdateOverlayTransform(overlay.transform, glowOverlaySources[i]);

                var mr = overlay.GetComponent<MeshRenderer>();
                if (mr != null && mr.material != null)
                    mr.material.SetColor("_BaseColor", c);
            }
            yield return null;
        }
    }

    void UpdateContent()
    {
        if (currentStep < stepTexte.Length)
        {
            if (inhaltText != null)
                inhaltText.text = stepTexte[currentStep];
        }
        if (currentStep < buttonLabels.Length)
        {
            if (buttonLabelText != null)
            {
                buttonLabelText.text = "\u25b6  " + buttonLabels[currentStep];
                buttonLabelText.color = new Color(0.08f, 0.12f, 0.35f); // Dunkelblau
            }
        }
    }

    public void GoToStep(int step)
    {
        currentStep = Mathf.Clamp(step, 0, stepTexte.Length - 1);
        waitingForPuzzle = false;
        UpdateContent();
    }

    public void ResetPanel()
    {
        currentStep = 0;
        waitingForPuzzle = false;
        SetRoboterVisible(false);
        SetLogoWuerfelVisible(true);
        SetGlow(false);
        UpdateContent();
    }
}
