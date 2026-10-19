using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Erklärt Inverse Kinematik live während der Roboter sich bewegt.
/// Einfach dieses Script an ein leeres GameObject hängen —
/// Canvas + alle UI-Elemente werden automatisch erstellt.
/// Danach URIKButton und EndEffektor/Ziel im Inspector zuweisen.
/// </summary>
[RequireComponent(typeof(Canvas))]
public class IKInfoTafel : MonoBehaviour
{
    [Header("Verbindung zum IK-System")]
    public URIKButton urikButton;
    public Transform  endEffektor;   // joint6 (Werkzeugflansch)
    public Transform  ikZiel;        // IK-Target Transform

    // ── Live-Textfelder (in BuildUI erzeugt) ────────────────────────────────
    private TMP_Text statusText;
    private TMP_Text abstandText;
    private TMP_Text iterationText;
    private TMP_Text phaseText;

    // ── Schritt-Highlight-Panels ────────────────────────────────────────────
    private Image schritt1Panel;
    private Image schritt2Panel;
    private Image schritt3Panel;

    // ── Farben ──────────────────────────────────────────────────────────────
    static readonly Color kAktiv    = new Color(0.15f, 0.90f, 0.35f);
    static readonly Color kInaktiv  = new Color(0.55f, 0.55f, 0.55f);
    static readonly Color kPhase1   = new Color(1.00f, 0.78f, 0.10f);
    static readonly Color kPhase2   = new Color(0.20f, 0.70f, 1.00f);
    static readonly Color kBGDark   = new Color(0.08f, 0.10f, 0.14f, 0.95f);
    static readonly Color kBGMid    = new Color(0.12f, 0.15f, 0.20f, 0.95f);
    static readonly Color kAccent   = new Color(0.20f, 0.55f, 0.90f);
    static readonly Color kSchritt  = new Color(0.20f, 0.22f, 0.28f, 0.85f);

    const float W = 600f;
    const float H = 780f;

    float blinkTimer;
    bool  blinkState;

    // ════════════════════════════════════════════════════════════════════════
    void Start()
    {
        // Auto-Referenzen setzen falls im Inspector leer
        if (urikButton == null)
            urikButton = FindObjectOfType<URIKButton>();

        if (urikButton != null)
        {
            if (endEffektor == null)
                endEffektor = urikButton.joint6_T_Axis6;
            if (ikZiel == null && urikButton.ikTarget != null)
                ikZiel = urikButton.ikTarget.transform;
        }

        if (urikButton == null)
            Debug.LogWarning("IKInfoTafel: Kein URIKButton in der Szene gefunden!");
    }

    void Awake()
    {
        // Canvas auf WorldSpace setzen
        Canvas c = GetComponent<Canvas>();
        c.renderMode = RenderMode.WorldSpace;

        RectTransform rt = GetComponent<RectTransform>();
        rt.sizeDelta   = new Vector2(W, H);
        transform.localScale = Vector3.one * 0.001f;   // 600px → 0.60 m

        CanvasScaler  cs = GetComponent<CanvasScaler>();  if (cs) cs.enabled = false;
        GraphicRaycaster gr = GetComponent<GraphicRaycaster>(); if (gr) gr.enabled = false;

        BuildUI(rt);
    }

    // ════════════════════════════════════════════════════════════════════════
    void BuildUI(RectTransform root)
    {
        Panel(root, "BG", V2(0,0), V2(W,H), kBGDark);

        // Kopfleiste
        var header = Panel(root, "Header", V2(0, H/2f-55f), V2(W,110f), kAccent);
        TMP(header, "Titel", "🤖  Inverse Kinematik (IK)",
            V2(0,0), V2(W-20f,80f), 32, FontStyles.Bold, Color.white);

        float y = H/2f - 155f;

        // ── Abschnitt: Was ist IK? ───────────────────────────────────────────
        Section(root, "WAS IST IK?", ref y);
        TMP(root, "Erkl",
            "Der Roboter kennt nur sein <b>Ziel</b>.\n" +
            "Er berechnet automatisch, wie sich\n" +
            "jedes Gelenk drehen muss —\n" +
            "rückwärts vom Ziel zur Basis.",
            V2(0, y-52f), V2(W-40f,115f), 22, FontStyles.Normal,
            new Color(0.88f,0.88f,0.95f));
        y -= 145f;

        // ── Abschnitt: Wie funktioniert es? ─────────────────────────────────
        Section(root, "WIE FUNKTIONIERT ES?", ref y);
        schritt1Panel = SchrittZeile(root, "① Abstand zum Ziel messen",        ref y);
        schritt2Panel = SchrittZeile(root, "② Jeden Gelenkwinkel anpassen",    ref y);
        schritt3Panel = SchrittZeile(root, "③ Wiederholen bis nah genug",      ref y);
        y -= 10f;

        // ── Abschnitt: Live-Status ───────────────────────────────────────────
        Section(root, "LIVE-STATUS", ref y);
        var sb = Panel(root, "StatusBlock", V2(0, y-65f), V2(W-20f,130f), kBGMid);

        Row(sb, "Status",       V2(0, 38f), out statusText);
        Row(sb, "Phase",        V2(0, -5f), out phaseText);
        Row(sb, "Abstand",      V2(0,-48f), out abstandText);
        y -= 150f;

        // ── Iterationen ──────────────────────────────────────────────────────
        var ib = Panel(root, "IterBlock", V2(0, y-30f), V2(W-20f,58f), kBGMid);
        TMP(ib, "IterLabel", "Iterationen/Frame:", V2(-100f,0), V2(220f,44f),
            20, FontStyles.Normal, new Color(0.65f,0.65f,0.75f));
        iterationText = TMP(ib, "IterWert", "—", V2(140f,0), V2(130f,44f),
            22, FontStyles.Bold, kInaktiv);
        iterationText.alignment = TextAlignmentOptions.Left;

        // ── Fußzeile ─────────────────────────────────────────────────────────
        var foot = Panel(root, "Footer", V2(0, -H/2f+28f), V2(W,55f),
            new Color(0.05f,0.08f,0.12f,0.95f));
        TMP(foot, "FooterTxt", "Girls'Day  ·  HAW Hamburg  ·  ppci",
            V2(0,0), V2(W-20f,40f), 18, FontStyles.Italic,
            new Color(0.5f,0.5f,0.6f));
    }

    // ════════════════════════════════════════════════════════════════════════
    void Update()
    {
        if (urikButton == null) return;
        bool aktiv  = urikButton.IsRunning;
        bool phase2 = urikButton.IsFollowMode;

        blinkTimer += Time.deltaTime;
        if (blinkTimer >= 0.5f) { blinkTimer = 0f; blinkState = !blinkState; }

        // Status
        if (statusText != null)
        {
            if      (!aktiv)  { statusText.text = "○ Warte auf Start"; statusText.color = kInaktiv; }
            else if (!phase2) { statusText.text = blinkState?"◉ BERECHNET…":"● BERECHNET…"; statusText.color = kPhase1; }
            else              { statusText.text = "◎ FOLGT ZIEL";        statusText.color = kAktiv; }
        }

        // Phase
        if (phaseText != null)
        {
            if      (!aktiv)  { phaseText.text = "Inaktiv";             phaseText.color = kInaktiv; }
            else if (!phase2) { phaseText.text = "Phase 1: Anfahren";   phaseText.color = kPhase1; }
            else              { phaseText.text = "Phase 2: Verfolgen";  phaseText.color = kPhase2; }
        }

        // Abstand
        if (abstandText != null)
        {
            if (!aktiv || endEffektor == null || ikZiel == null)
            {
                abstandText.text = "— cm"; abstandText.color = kInaktiv;
            }
            else
            {
                float d = Vector3.Distance(endEffektor.position, ikZiel.position);
                abstandText.text  = $"{d * 100f:F1} cm";
                abstandText.color = d < 0.05f ? kAktiv : kPhase1;
            }
        }

        // Iterationen
        if (iterationText != null)
        {
            if (!aktiv) { iterationText.text = "—"; iterationText.color = kInaktiv; }
            else
            {
                iterationText.text = $"{urikButton.stepsPerFrame}x / Frame";
                iterationText.color = kAktiv;
            }
        }

        // Schritt-Highlights
        Color ausCol   = kSchritt;
        Color s1Col    = new Color(0.15f, 0.40f, 0.15f, 0.85f);
        Color s2Col    = new Color(0.45f, 0.35f, 0.05f, 0.85f);
        Color s3Col    = new Color(0.10f, 0.25f, 0.50f, 0.85f);

        if (schritt1Panel) schritt1Panel.color = aktiv              ? s1Col : ausCol;
        if (schritt2Panel) schritt2Panel.color = aktiv && !phase2   ? s2Col : ausCol;
        if (schritt3Panel) schritt3Panel.color = aktiv &&  phase2   ? s3Col : ausCol;
    }

    // ════════════════════════════════════════════════════════════════════════
    // ── UI-Builder-Hilfsmethoden ─────────────────────────────────────────────

    RectTransform Panel(RectTransform parent, string name,
        Vector2 pos, Vector2 size, Color col)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var img = go.AddComponent<Image>();
        img.color = col; img.raycastTarget = false;
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = rt.pivot = V2(0.5f, 0.5f);
        rt.anchoredPosition = pos; rt.sizeDelta = size;
        return rt;
    }

    TMP_Text TMP(RectTransform parent, string name, string text,
        Vector2 pos, Vector2 size,
        float fs, FontStyles style, Color col)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var t = go.AddComponent<TextMeshProUGUI>();
        t.text = text; t.fontSize = fs; t.fontStyle = style;
        t.color = col; t.alignment = TextAlignmentOptions.Center;
        t.raycastTarget = false; t.overflowMode = TextOverflowModes.Overflow;
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = rt.pivot = V2(0.5f, 0.5f);
        rt.anchoredPosition = pos; rt.sizeDelta = size;
        return t;
    }

    void Section(RectTransform parent, string label, ref float y)
    {
        Panel(parent, label+"_ln", V2(0, y+4f), V2(W-30f, 2f),
              new Color(0.35f, 0.45f, 0.65f));
        TMP(parent, label+"_hd", label,
            V2(0, y-14f), V2(W-30f, 34f),
            19, FontStyles.Bold|FontStyles.SmallCaps,
            new Color(0.55f, 0.70f, 0.95f));
        y -= 48f;
    }

    Image SchrittZeile(RectTransform parent, string text, ref float y)
    {
        var rt = Panel(parent, "S_"+text.Substring(0,3),
                       V2(0, y), V2(W-30f, 52f), kSchritt);
        TMP(rt, "T", text, V2(8f,0), V2(W-60f,44f),
            21, FontStyles.Normal, new Color(0.88f,0.88f,0.95f));
        y -= 60f;
        return rt.GetComponent<Image>();
    }

    void Row(RectTransform parent, string label, Vector2 pos, out TMP_Text valueOut)
    {
        TMP(parent, label+"Lbl", label+":",
            pos + V2(-130f,0), V2(150f,36f),
            20, FontStyles.Normal, new Color(0.65f,0.65f,0.75f));
        valueOut = TMP(parent, label+"Val", "—",
            pos + V2(90f,0), V2(260f,36f),
            20, FontStyles.Bold, kInaktiv);
        valueOut.alignment = TextAlignmentOptions.Left;
    }

    static Vector2 V2(float x, float y) => new Vector2(x, y);
}
