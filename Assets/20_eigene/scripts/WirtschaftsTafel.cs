using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Wirtschaftlichkeitsrechnung: Roboter vs. Mensch (Vollkostenrechnung).
/// Selbstbauendes WorldSpace Canvas — einfach an leeres GO hängen.
/// Nach Challenge: SetZeiten(manuell, ik) aufrufen.
///
/// Kalkulationsgrundlage (Deutschland 2024/25):
/// - UR5e Cobot inkl. Greifer + Integration: 35.000 EUR
/// - Lineare AfA über 5 Jahre
/// - Kalkulatorischer Zinssatz 4% auf Durchschnittskapitalbindung
/// - Facharbeiter Vollkostenstundensatz inkl. AG-Anteil SV, Urlaubs-/Krankheitstage
/// </summary>
[RequireComponent(typeof(Canvas))]
public class WirtschaftsTafel : MonoBehaviour
{
    // ── Investition & AfA ────────────────────────────────────────────
    const float kInvestition         = 35000f;  // EUR (UR5e + Greifer + Integration)
    const float kNutzungsdauer       = 5f;      // Jahre (lineare AfA, steuerlich üblich)
    const float kKalkZinssatz        = 0.04f;   // 4% p.a. (Opportunitätskosten Kapitalbindung)

    // ── Roboter-Betriebskosten (jährlich) ────────────────────────────
    const float kStromJahr           = 540f;    // EUR (~300W Ø × 6.000h × 0,30 EUR/kWh)
    const float kWartungJahr         = 1500f;   // EUR (jährlicher Service lt. UR-Empfehlung)
    const float kVerschleissJahr     = 500f;    // EUR (Kabel, Schläuche, Greifer-Verschleiß)
    const float kVersicherungJahr    = 460f;    // EUR (Maschinenbruch/Haftpflicht)
    const float kBetriebskostenJahr  = kStromJahr + kWartungJahr + kVerschleissJahr + kVersicherungJahr; // 3.000 EUR

    // ── Mensch ───────────────────────────────────────────────────────
    const float kVollkostenStunde    = 38f;     // EUR/h (Bruttolohn + AG-SV 20% + Nebenkosten 30%)
    const float kArbeitsstundenJahr  = 1760f;   // h/Jahr (220 Arbeitstage × 8h, abzgl. Urlaub/Feiertage)

    // ── Roboter-Einsatz ──────────────────────────────────────────────
    const float kRoboterStundenJahr  = 4000f;   // h/Jahr (250 Tage × 16h, 2-Schicht-Betrieb)

    // ── Abgeleitete Konstanten ───────────────────────────────────────
    static readonly float kAfAJahr           = kInvestition / kNutzungsdauer;                    // 7.000 EUR/Jahr
    static readonly float kKalkZinsenJahr    = (kInvestition / 2f) * kKalkZinssatz;              // 700 EUR/Jahr (Ø-Kapitalbindung)
    static readonly float kRoboterVollkosten = kAfAJahr + kBetriebskostenJahr + kKalkZinsenJahr; // 10.700 EUR/Jahr
    static readonly float kRoboterProStunde  = kRoboterVollkosten / kRoboterStundenJahr;         // ~2,68 EUR/h
    static readonly float kMenschJahr        = kVollkostenStunde * kArbeitsstundenJahr;           // 66.880 EUR/Jahr
    static readonly float kEinsparungJahr    = kMenschJahr - kRoboterVollkosten;                  // 56.180 EUR/Jahr
    // Statische Amortisation: Investition / (Einsparung ohne AfA, da AfA = Investition/t)
    // → Investition / (Personalkosten - Betriebskosten - kalk. Zinsen)
    static readonly float kAmortisationMonate = (kInvestition / (kMenschJahr - kBetriebskostenJahr - kKalkZinsenJahr)) * 12f;

    // ── Farben ───────────────────────────────────────────────────────
    static readonly Color kBGDark  = new Color(0.06f, 0.08f, 0.12f, 0.95f);
    static readonly Color kBGMid   = new Color(0.10f, 0.13f, 0.18f, 0.95f);
    static readonly Color kAccent  = new Color(0.15f, 0.25f, 0.55f);        // HAW-Blau statt Orange
    static readonly Color kGruen   = new Color(0.15f, 0.85f, 0.30f);
    static readonly Color kRot     = new Color(0.90f, 0.25f, 0.20f);
    static readonly Color kBlau    = new Color(0.30f, 0.60f, 1.00f);
    static readonly Color kLabel   = new Color(0.60f, 0.60f, 0.72f);

    const float W = 720f;
    const float H = 1280f;  // etwas höher für mehr Zeilen

    // ── Live-Werte ──────────────────────────────────────────────────
    private TMP_Text zeitManuellText;
    private TMP_Text zeitIKText;
    private TMP_Text faktorText;
    private TMP_Text kostenMenschText;
    private TMP_Text kostenRoboterText;
    private TMP_Text einsparungText;
    private TMP_Text amortisationText;

    // ════════════════════════════════════════════════════════════════
    void Awake()
    {
        Canvas c = GetComponent<Canvas>();
        c.renderMode = RenderMode.WorldSpace;

        RectTransform rt = GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(W, H);
        transform.localScale = Vector3.one * 0.001f;

        CanvasScaler cs = GetComponent<CanvasScaler>();
        if (cs) cs.enabled = false;
        GraphicRaycaster gr = GetComponent<GraphicRaycaster>();
        if (gr) gr.enabled = false;

        BuildUI(rt);
    }

    // ════════════════════════════════════════════════════════════════
    void BuildUI(RectTransform root)
    {
        Panel(root, "BG", V(0, 0), V(W, H), kBGDark);

        // ── Kopfleiste ────────────────────────────────────────────
        var header = Panel(root, "Header", V(0, H / 2f - 45f), V(W, 90f), kAccent);
        Txt(header, "Titel", "Vollkostenrechnung: Roboter vs. Mensch",
            V(0, 0), V(W - 20f, 70f), 26, FontStyles.Bold, Color.white);

        float y = H / 2f - 130f;

        // ══════════════════════════════════════════════════════════
        // ZEITMESSUNG (CHALLENGE)
        // ══════════════════════════════════════════════════════════
        Section(root, "ZEITMESSUNG (CHALLENGE)", ref y);

        var zb = Panel(root, "ZeitBlock", V(0, y - 55f), V(W - 20f, 110f), kBGMid);
        ZeileLabel(zb, "Ihre Zeit (manuell):", V(0, 22f), out zeitManuellText, kRot);
        ZeileLabel(zb, "Roboter-Zeit (IK):", V(0, -22f), out zeitIKText, kGruen);
        y -= 130f;

        var fb = Panel(root, "FaktorBlock", V(0, y - 18f), V(W - 20f, 46f), kBGMid);
        Txt(fb, "FLbl", "Faktor schneller:", V(-120f, 0), V(250f, 38f), 19, FontStyles.Normal, kLabel);
        faktorText = Txt(fb, "FVal", "\u2014", V(130f, 0), V(200f, 38f), 22, FontStyles.Bold, kGruen);
        faktorText.alignment = TextAlignmentOptions.Left;
        y -= 72f;

        // ══════════════════════════════════════════════════════════
        // INVESTITION UR5e COBOT
        // ══════════════════════════════════════════════════════════
        Section(root, "INVESTITION UR5e COBOT", ref y);

        var ib = Panel(root, "InvestBlock", V(0, y - 70f), V(W - 20f, 145f), kBGMid);
        float iy = 48f;
        FixZeile(ib, "Anschaffung (UR5e + Greifer + Integration):", $"{kInvestition:N0} \u20ac", ref iy, kLabel, kBlau);
        FixZeile(ib, $"Lineare AfA ({kNutzungsdauer:F0} Jahre):", $"{kAfAJahr:N0} \u20ac/Jahr", ref iy, kLabel, kLabel);
        FixZeile(ib, $"Kalk. Zinsen ({kKalkZinssatz * 100f:F0}% auf \u00d8-Kapitalbindung):", $"{kKalkZinsenJahr:N0} \u20ac/Jahr", ref iy, kLabel, kLabel);
        FixZeile(ib, "Betriebskosten (Strom, Wartung, Verschlei\u00df, Vers.):", $"{kBetriebskostenJahr:N0} \u20ac/Jahr", ref iy, kLabel, kLabel);
        y -= 165f;

        // ══════════════════════════════════════════════════════════
        // VOLLKOSTENVERGLEICH PRO JAHR
        // ══════════════════════════════════════════════════════════
        Section(root, "VOLLKOSTENVERGLEICH PRO JAHR", ref y);

        var kb = Panel(root, "KostenBlock", V(0, y - 90f), V(W - 20f, 185f), kBGMid);
        float ky = 68f;

        Txt(kb, "H1", "Facharbeiter", V(-170f, ky), V(200f, 28f), 17, FontStyles.Bold, kRot);
        Txt(kb, "H2", "UR5e Cobot", V(130f, ky), V(200f, 28f), 17, FontStyles.Bold, kGruen);
        ky -= 32f;

        KostenZeile(kb, "Vollkosten/h:", $"{kVollkostenStunde:F0} \u20ac", ref ky, kRot, out _,
            $"{kRoboterProStunde:F2} \u20ac", kGruen, out _);
        KostenZeile(kb, "Einsatzstunden/Jahr:", $"{kArbeitsstundenJahr:F0} h", ref ky, kLabel, out _,
            $"{kRoboterStundenJahr:F0} h", kLabel, out _);
        KostenZeile(kb, "Jahreskosten:", "", ref ky, kRot, out kostenMenschText,
            "", kGruen, out kostenRoboterText);
        y -= 205f;

        // ══════════════════════════════════════════════════════════
        // ERGEBNIS
        // ══════════════════════════════════════════════════════════
        Section(root, "ERGEBNIS", ref y);

        var eb = Panel(root, "ErgebBlock", V(0, y - 55f), V(W - 20f, 110f),
            new Color(0.08f, 0.18f, 0.08f, 0.95f));
        ZeileLabel(eb, "Einsparung/Jahr:", V(0, 22f), out einsparungText, kGruen);
        ZeileLabel(eb, "Stat. Amortisation:", V(0, -22f), out amortisationText, kBlau);
        y -= 130f;

        // ── Hinweis ───────────────────────────────────────────────
        Txt(root, "Hinweis",
            "Vereinfachte Vollkostenrechnung. Annahme: 1:1-Ersatz einer\n" +
            "repetitiven T\u00e4tigkeit. Vollkostenstundensatz inkl. AG-Anteil\n" +
            "Sozialversicherung, Urlaubs- und Krankheitstage.",
            V(0, y - 32f), V(W - 30f, 70f), 14, FontStyles.Italic, kLabel);

        // ── Fußzeile ──────────────────────────────────────────────
        var foot = Panel(root, "Footer", V(0, -H / 2f + 28f), V(W, 50f),
            new Color(0.04f, 0.06f, 0.10f, 0.95f));
        Txt(foot, "FootTxt", "Girls\u2019Day  \u00b7  HAW Hamburg  \u00b7  ppci",
            V(0, 0), V(W - 20f, 38f), 17, FontStyles.Italic, new Color(0.5f, 0.5f, 0.6f));

        // Initialwerte
        SetZeiten(0f, 0f);
    }

    // ════════════════════════════════════════════════════════════════
    /// <summary>Wird nach der Challenge aufgerufen mit den gemessenen Zeiten.</summary>
    public void SetZeiten(float manuell, float ik)
    {
        if (zeitManuellText != null)
            zeitManuellText.text = manuell > 0 ? $"{manuell:F1} s" : "\u2014";
        if (zeitIKText != null)
            zeitIKText.text = ik > 0 ? $"{ik:F1} s" : "\u2014";

        if (faktorText != null)
        {
            if (manuell > 0 && ik > 0)
                faktorText.text = $"{manuell / ik:F1}x schneller";
            else
                faktorText.text = "\u2014";
        }

        // Jahreskosten
        if (kostenMenschText != null)
            kostenMenschText.text = $"{kMenschJahr:N0} \u20ac";
        if (kostenRoboterText != null)
            kostenRoboterText.text = $"{kRoboterVollkosten:N0} \u20ac";

        // Ergebnis
        if (einsparungText != null)
        {
            einsparungText.text = $"{kEinsparungJahr:N0} \u20ac/Jahr";
            einsparungText.color = kGruen;
        }
        if (amortisationText != null)
        {
            amortisationText.text = $"ca. {kAmortisationMonate:F0} Monate";
            amortisationText.color = kBlau;
        }
    }

    // ════════════════════════════════════════════════════════════════
    // ── UI-Builder ────────────────────────────────────────────────

    RectTransform Panel(RectTransform parent, string name, Vector2 pos, Vector2 size, Color col)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var img = go.AddComponent<Image>();
        img.color = col; img.raycastTarget = false;
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = rt.pivot = V(0.5f, 0.5f);
        rt.anchoredPosition = pos; rt.sizeDelta = size;
        return rt;
    }

    TMP_Text Txt(RectTransform parent, string name, string text,
        Vector2 pos, Vector2 size, float fs, FontStyles style, Color col)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var t = go.AddComponent<TextMeshProUGUI>();
        t.text = text; t.fontSize = fs; t.fontStyle = style;
        t.color = col; t.alignment = TextAlignmentOptions.Center;
        t.raycastTarget = false; t.overflowMode = TextOverflowModes.Overflow;
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = rt.pivot = V(0.5f, 0.5f);
        rt.anchoredPosition = pos; rt.sizeDelta = size;
        return t;
    }

    void Section(RectTransform parent, string label, ref float y)
    {
        Panel(parent, label + "_ln", V(0, y + 4f), V(W - 30f, 2f), new Color(0.35f, 0.45f, 0.65f));
        Txt(parent, label + "_hd", label, V(0, y - 14f), V(W - 30f, 32f),
            17, FontStyles.Bold | FontStyles.SmallCaps, new Color(0.55f, 0.70f, 0.95f));
        y -= 44f;
    }

    void ZeileLabel(RectTransform parent, string label, Vector2 pos, out TMP_Text val, Color valCol)
    {
        Txt(parent, label + "L", label, pos + V(-140f, 0), V(250f, 34f), 19, FontStyles.Normal, kLabel);
        val = Txt(parent, label + "V", "\u2014", pos + V(130f, 0), V(250f, 34f), 21, FontStyles.Bold, valCol);
        val.alignment = TextAlignmentOptions.Left;
    }

    void FixZeile(RectTransform parent, string label, string value, ref float iy, Color labelCol, Color valCol)
    {
        Txt(parent, "FL" + label, label, V(-40f, iy), V(W - 60f, 26f), 15, FontStyles.Normal, labelCol)
            .alignment = TextAlignmentOptions.Left;
        var v = Txt(parent, "FV" + label, value, V(40f, iy), V(W - 60f, 26f), 16, FontStyles.Bold, valCol);
        v.alignment = TextAlignmentOptions.Right;
        iy -= 30f;
    }

    void KostenZeile(RectTransform parent, string label,
        string leftVal, ref float ky, Color leftCol, out TMP_Text leftOut,
        string rightVal, Color rightCol, out TMP_Text rightOut)
    {
        Txt(parent, "KL" + label, label, V(-300f, ky), V(160f, 26f), 16, FontStyles.Normal, kLabel);
        leftOut = Txt(parent, "LV" + label, leftVal, V(-120f, ky), V(150f, 26f), 17, FontStyles.Bold, leftCol);
        leftOut.alignment = TextAlignmentOptions.Right;
        rightOut = Txt(parent, "RV" + label, rightVal, V(170f, ky), V(150f, 26f), 17, FontStyles.Bold, rightCol);
        rightOut.alignment = TextAlignmentOptions.Right;
        ky -= 32f;
    }

    static Vector2 V(float x, float y) => new Vector2(x, y);
}
