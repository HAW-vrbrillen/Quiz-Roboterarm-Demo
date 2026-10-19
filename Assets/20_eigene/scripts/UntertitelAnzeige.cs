using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

/// <summary>
/// Grosses, gut lesbares Untertitel-/Anweisungsfeld, das automatisch erscheint,
/// solange eine bestimmte AudioSource spielt (z.B. der Erfolgs-Sprachclip nach den
/// Challenges), und danach wieder ausblendet.
///
/// Selbstbauendes WorldSpace-Canvas im Stil von WirtschaftsTafel/IKInfoTafel:
/// dunkler halbtransparenter Balken, grosse weisse TMP-Schrift, autom. Zeilenumbruch.
///
/// Bedienung:
///  1. Script an ein leeres GameObject haengen (Canvas wird automatisch erzeugt).
///  2. "audioQuelle" auf die AudioSource setzen, die den Sprachclip abspielt
///     (z.B. die AudioSource auf "AudioObj1").
///  3. "untertitelText" mit dem gesprochenen Text fuellen (Transkript des Clips).
///  4. Optional "kameraAnker" auf CenterEyeAnchor setzen, damit der Untertitel
///     kamerafixiert immer im Blickfeld bleibt (sonst wird Camera.main genutzt).
///
/// Alternativ kann die Anzeige per Skript gesteuert werden:
///   untertitel.ZeigeWaehrend("Text...", audioSource);   // bis Clip endet
///   untertitel.Zeige("Text...");  / untertitel.Verstecke();
/// </summary>
[RequireComponent(typeof(Canvas))]
public class UntertitelAnzeige : MonoBehaviour
{
    /// <summary>Ein zeitlich getaktetes Untertitel-Segment (Sekunden ab Clip-Start).</summary>
    [System.Serializable]
    public struct UntertitelSegment
    {
        [TextArea(1, 3)] public string text;
        [Tooltip("Startzeit in Sekunden ab Clip-Beginn.")] public float start;
        [Tooltip("Endzeit in Sekunden ab Clip-Beginn.")]   public float ende;
    }

    [Header("Audio-Quelle & Text")]
    [Tooltip("Die AudioSource, waehrend deren Wiedergabe der Untertitel gezeigt wird.")]
    public AudioSource audioQuelle;
    [Tooltip("Der anzuzeigende Untertitel-Text (Transkript des Clips).")]
    [TextArea(2, 6)]
    public string untertitelText = "";

    [Header("Platzierung (kamerafixiert)")]
    [Tooltip("CenterEyeAnchor o.ae. Wenn leer, wird Camera.main verwendet.")]
    public Transform kameraAnker;
    [Tooltip("Abstand vor der Kamera in Metern.")]
    public float abstand = 1.6f;
    [Tooltip("Hoehen-Versatz in Metern (negativ = unteres Blickfeld).")]
    public float hoehenVersatz = -0.45f;

    [Header("Timing")]
    [Tooltip("Haltezeit nach Ende der Wiedergabe, bevor ausgeblendet wird (gegen Flackern).")]
    public float haltezeit = 0.4f;
    [Tooltip("Dauer der Ein-/Ausblendung in Sekunden.")]
    public float fadeDauer = 0.2f;

    // ── Farben (konsistent zu den anderen Tafeln) ────────────────────────────
    static readonly Color kBGDark = new Color(0.05f, 0.07f, 0.11f, 0.92f);
    static readonly Color kText   = new Color(0.98f, 0.98f, 1.00f);

    const float W = 1100f;   // px  -> 1.10 m bei Scale 0.001
    const float H = 240f;    // px  -> 0.24 m

    // ── Laufzeit-Referenzen ──────────────────────────────────────────────────
    private CanvasGroup _group;
    private TMP_Text    _untertitelText;

    private bool  _sichtbar;
    private bool  _ausblendAngefordert;
    private float _ausblendTimer;

    // Beobachtete Wiedergabe-Quelle (Inspector oder per ZeigeWaehrend gesetzt).
    private AudioSource _aktiveQuelle;
    private bool  _hatGespielt;     // Quelle wurde mind. 1 Frame als spielend gesehen
    private float _quelleTimer;     // Zeit seit Quelle nicht (mehr) spielt
    private Coroutine _segmentCoroutine;

    // Toleranz, bis ein gerade gestarteter Clip die Wiedergabe wirklich meldet.
    const float kStartToleranz = 0.5f;

    // ════════════════════════════════════════════════════════════════════════
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

        _group = gameObject.GetComponent<CanvasGroup>();
        if (_group == null) _group = gameObject.AddComponent<CanvasGroup>();
        _group.interactable = false;
        _group.blocksRaycasts = false;
        _group.alpha = 0f;

        BuildUI(rt);
    }

    void Start()
    {
        // Kamera-Anker automatisch bestimmen, falls im Inspector leer.
        if (kameraAnker == null && Camera.main != null)
            kameraAnker = Camera.main.transform;

        if (kameraAnker != null)
        {
            transform.SetParent(kameraAnker, false);
            transform.localPosition = new Vector3(0f, hoehenVersatz, abstand);
            transform.localRotation = Quaternion.identity;
        }

        // Standard-Quelle im Inspector -> automatisch beobachten.
        if (audioQuelle != null)
            _aktiveQuelle = audioQuelle;

        // Text vorbereiten (bleibt unsichtbar bis Wiedergabe startet).
        SetzeText(untertitelText);
    }

    // ════════════════════════════════════════════════════════════════════════
    // ── Oeffentliche API (optional per Skript) ────────────────────────────────

    /// <summary>Zeigt den Text und blendet automatisch aus, sobald die AudioSource endet.</summary>
    public void ZeigeWaehrend(string text, AudioSource quelle)
    {
        _aktiveQuelle = quelle;
        _hatGespielt = false;
        _quelleTimer = 0f;
        Zeige(text);                      // sofort einblenden (kein Warten auf isPlaying)
    }

    /// <summary>Blendet den Untertitel mit Text ein (bleibt bis Verstecke()/Wiedergabe-Ende).</summary>
    public void Zeige(string text)
    {
        SetzeText(text);
        _sichtbar = true;
        _ausblendAngefordert = false;
        _ausblendTimer = 0f;
    }

    /// <summary>Blendet den Untertitel nach der Haltezeit aus.</summary>
    public void Verstecke()
    {
        _ausblendAngefordert = true;
        _ausblendTimer = 0f;
    }

    /// <summary>
    /// Zeigt eine zeitlich getaktete Folge von Segmenten (Sekunden ab jetzt).
    /// Aufrufen zeitgleich mit dem Start des zugehoerigen AudioClips.
    /// </summary>
    public void ZeigeSegmente(UntertitelSegment[] segmente)
    {
        if (segmente == null || segmente.Length == 0) return;
        _aktiveQuelle = null;              // AudioSource-Watcher deaktivieren
        _ausblendAngefordert = false;
        if (_segmentCoroutine != null) StopCoroutine(_segmentCoroutine);
        _segmentCoroutine = StartCoroutine(SegmenteAblauf(segmente));
    }

    IEnumerator SegmenteAblauf(UntertitelSegment[] segmente)
    {
        float ende = 0f;
        foreach (var s in segmente) if (s.ende > ende) ende = s.ende;

        float t = 0f;
        while (t <= ende + 0.15f)
        {
            string aktuell = null;
            for (int k = 0; k < segmente.Length; k++)
            {
                if (t >= segmente[k].start && t < segmente[k].ende)
                {
                    aktuell = segmente[k].text;
                    break;
                }
            }

            if (!string.IsNullOrEmpty(aktuell))
            {
                SetzeText(aktuell);
                _sichtbar = true;
            }
            else
            {
                _sichtbar = false;   // Luecke zwischen Segmenten -> ausblenden
            }

            yield return null;
            t += Time.deltaTime;
        }

        _sichtbar = false;
        _segmentCoroutine = null;
    }

    // ════════════════════════════════════════════════════════════════════════
    void Update()
    {
        // AudioSource beobachten: waehrend Wiedergabe sichtbar, danach ausblenden.
        if (_aktiveQuelle != null)
        {
            if (_aktiveQuelle.isPlaying)
            {
                // Quelle spielt -> sichtbar halten (deckt auch PlayOneShot ab).
                _hatGespielt = true;
                _quelleTimer = 0f;
                _sichtbar = true;
                _ausblendAngefordert = false;
                _ausblendTimer = 0f;
            }
            else
            {
                _quelleTimer += Time.deltaTime;
                // Ende erkannt, wenn die Quelle gespielt hat und nun fertig ist,
                // oder der Clip nach der Start-Toleranz nie angelaufen ist.
                bool zuEnde = _hatGespielt || _quelleTimer > kStartToleranz;
                if (zuEnde && _sichtbar && !_ausblendAngefordert)
                    Verstecke();
            }
        }

        // Nach Wiedergabe-Ende: Haltezeit abwarten, dann ausblenden.
        if (_ausblendAngefordert)
        {
            _ausblendTimer += Time.deltaTime;
            if (_ausblendTimer >= haltezeit)
            {
                _sichtbar = false;
                _ausblendAngefordert = false;
            }
        }

        // Sanftes Ein-/Ausblenden
        if (_group != null)
        {
            float ziel = _sichtbar ? 1f : 0f;
            float schritt = (fadeDauer > 0f) ? Time.deltaTime / fadeDauer : 1f;
            _group.alpha = Mathf.MoveTowards(_group.alpha, ziel, schritt);
        }
    }

    void SetzeText(string text)
    {
        if (_untertitelText != null)
            _untertitelText.text = text ?? "";
    }

    // ════════════════════════════════════════════════════════════════════════
    void BuildUI(RectTransform root)
    {
        // Hintergrund-Balken
        Panel(root, "BG", V(0, 0), V(W, H), kBGDark);

        // Untertitel-Text: grosse, kontrastreiche Schrift mit autom. Zeilenumbruch.
        _untertitelText = Txt(root, "Untertitel", "",
            V(0, 0), V(W - 60f, H - 40f), 48, FontStyles.Bold, kText);
        _untertitelText.enableWordWrapping = true;
        _untertitelText.alignment = TextAlignmentOptions.Center;
        _untertitelText.overflowMode = TextOverflowModes.Overflow;
        // Auto-Sizing: kurze Segmente gross, lange schrumpfen automatisch ins Feld.
        _untertitelText.enableAutoSizing = true;
        _untertitelText.fontSizeMax = 48f;
        _untertitelText.fontSizeMin = 26f;
    }

    // ════════════════════════════════════════════════════════════════════════
    // ── UI-Builder-Hilfsmethoden (Stil wie WirtschaftsTafel/IKInfoTafel) ─────

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

    static Vector2 V(float x, float y) => new Vector2(x, y);
}
