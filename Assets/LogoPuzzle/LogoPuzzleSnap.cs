using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Oculus.Interaction;
using Oculus.Interaction.HandGrab;

/// <summary>
/// Erkennt ob die Logo-Puzzle-Teile richtig zusammengesetzt sind.
/// Hänge dieses Script an ein leeres GameObject in der Szene.
///
/// Setup:
/// 1. Weise die 3 Puzzle-Cubes dem puzzlePieces Array zu
/// 2. Weise die 3 Ziel-Transforms dem targetPositions Array zu
/// 3. Bei Play: Wenn ein Teil nah genug am Ziel ist, rastet es ein
/// </summary>
public class LogoPuzzleSnap : MonoBehaviour
{
    [Header("Puzzle-Teile (die greifbaren Cubes)")]
    public Transform[] puzzlePieces = new Transform[3];

    [Header("Ziel-Positionen (Glas-Cubes an den Zielen)")]
    public Transform[] targetPositions = new Transform[3];

    [Header("Einstellungen")]
    [Tooltip("Abstand in Metern, ab dem ein Teil einrastet")]
    public float snapDistance = 0.15f;

    [Tooltip("Soll das Teil nach dem Einrasten fixiert werden?")]
    public bool lockAfterSnap = true;

    [Header("Feedback")]
    [Tooltip("Sound beim Einrasten (optional)")]
    public AudioClip snapSound;

    [Tooltip("Farbe wenn alle Teile platziert sind")]
    public Color successGlowColor = new Color(0f, 1f, 0.5f, 1f);

    [Header("Sprach-Feedback (AudioClips)")]
    [Tooltip("Lob-Audio pro Logo (Index muss zu puzzlePieces passen)")]
    public AudioClip[] lobAudioProLogo = new AudioClip[3];

    [Tooltip("Ansage wenn alle Logos richtig sind")]
    public AudioClip alleRichtigAudio;

    [Header("Untertitel (optional)")]
    [Tooltip("Untertitel-Anzeige, die den gesprochenen Lob-Text zeigt (kann leer bleiben).")]
    public UntertitelAnzeige untertitelAnzeige;

    [Tooltip("Untertitel-Text fuer die sofort abgespielten Logos (Hamburg=0, HAW=1). Der 3. Clip nutzt endSegmente.")]
    [TextArea(1, 3)]
    public string[] lobUntertitelProLogo = new string[3]
    {
        "Sehr gut, den ersten geschafft!",
        "Der zweite sitzt!",
        ""   // Rathaus-Passage/End-Clip: wird ueber endSegmente (getaktet) angezeigt
    };

    [Tooltip("Zeitlich getaktete Untertitel-Segmente fuer den langen End-Clip (Zeiten in Sekunden ab Clip-Start). Wortlaut ggf. an das Audio anpassen.")]
    public UntertitelAnzeige.UntertitelSegment[] endSegmente = new UntertitelAnzeige.UntertitelSegment[]
    {
        new UntertitelAnzeige.UntertitelSegment{ start=0.6f,  ende=5.6f,  text="Toll, ihr habt es geschafft! Auch das dritte Logo sitzt jetzt richtig." },
        new UntertitelAnzeige.UntertitelSegment{ start=5.6f,  ende=11.6f, text="Und guckt doch mal in die beiden großen Tafeln." },
        new UntertitelAnzeige.UntertitelSegment{ start=11.6f, ende=15.6f, text="Bei den Logos habe ich ja nur zwei Beispiele ausgewählt." },
        new UntertitelAnzeige.UntertitelSegment{ start=15.6f, ende=20.6f, text="Auf den Tafeln seht ihr weitere Ingenieurinnen, die große Erfolge erreicht haben." },
        new UntertitelAnzeige.UntertitelSegment{ start=20.6f, ende=24.6f, text="Es sind Bereiche wie Maschinenbau, Kreislaufwirtschaft," },
        new UntertitelAnzeige.UntertitelSegment{ start=24.6f, ende=28.6f, text="Beschichtungstechnik mit Energiewendebezug," },
        new UntertitelAnzeige.UntertitelSegment{ start=28.6f, ende=35.6f, text="Maschinenbauakustik, Anlagenbau, Automobilindustrie." },
        new UntertitelAnzeige.UntertitelSegment{ start=35.6f, ende=39.6f, text="Oder ihr guckt einfach mal beim VDMA unter „Ingenieurinnen“." },
        new UntertitelAnzeige.UntertitelSegment{ start=39.6f, ende=44.6f, text="Dort gibt es ein Video, wo man sich das noch viel genauer angucken kann." },
        new UntertitelAnzeige.UntertitelSegment{ start=44.6f, ende=48.6f, text="Bis bald im Studiengang Maschinenbau und Produktion hier an der HAW –" },
        new UntertitelAnzeige.UntertitelSegment{ start=48.6f, ende=53.6f, text="übrigens mit Themen wie künstliche Intelligenz, Mobilität," },
        new UntertitelAnzeige.UntertitelSegment{ start=53.6f, ende=55.6f, text="Nachhaltigkeit und Co." },
        new UntertitelAnzeige.UntertitelSegment{ start=55.6f, ende=61.6f, text="Und auch die Rolle des Menschen ist ganz wichtig – bei den brennenden Fragen," },
        new UntertitelAnzeige.UntertitelSegment{ start=61.6f, ende=64.6f, text="die wir auf der Welt aktuell haben." },
        new UntertitelAnzeige.UntertitelSegment{ start=64.6f, ende=69.6f, text="und die zum Beispiel in den United Nations Sustainability Goals" },
        new UntertitelAnzeige.UntertitelSegment{ start=69.6f, ende=75.6f, text="ganz toll zusammengefasst und wichtig hervorgehoben werden." },
        new UntertitelAnzeige.UntertitelSegment{ start=75.6f, ende=78.6f, text="Viele Grüße von Professor Randolf Eisenberg," },
        new UntertitelAnzeige.UntertitelSegment{ start=78.6f, ende=80.6f, text="und den Studierenden Nui und Hün," },
        new UntertitelAnzeige.UntertitelSegment{ start=80.6f, ende=85.6f, text="sowie den wissenschaftlichen Mitarbeitenden Frau Schwarz und Herrn Peters." },
        new UntertitelAnzeige.UntertitelSegment{ start=85.6f, ende=88.0f, text="Bis bald!" },
    };

    [Header("Belohnungs-Bilder")]
    [Tooltip("Gerahmte Bilder, die nach Erfolg eingeblendet werden")]
    public GameObject[] belohnungsBilder;

    [Header("Nach Puzzle-Erfolg einblenden")]
    [Tooltip("Objekte die erst erscheinen wenn alle Logos richtig sind (z.B. Roboter, 2. Challenge-Text)")]
    public GameObject[] nachErfolgEinblenden;

    [Header("Rathaus-Passage Index")]
    [Tooltip("Index des Rathaus-Passage-Logos in puzzlePieces (Audio erst bei alle komplett)")]
    public int rathausPassageIndex = 2;

    private bool[] isSnapped;
    private int snappedCount = 0;
    private bool puzzleComplete = false;
    public bool PuzzleComplete => puzzleComplete;
    private bool audioAnnouncementDone = false;
    public bool AudioAnnouncementDone => audioAnnouncementDone;
    private AudioSource audioSource;

    void Start()
    {
        isSnapped = new bool[puzzlePieces.Length];
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;

        // Belohnungs-Bilder initial verstecken
        if (belohnungsBilder != null)
        {
            foreach (var bild in belohnungsBilder)
            {
                if (bild != null) bild.SetActive(false);
            }
        }

        // Nach-Erfolg-Objekte initial verstecken
        if (nachErfolgEinblenden != null)
        {
            foreach (var obj in nachErfolgEinblenden)
            {
                if (obj != null) obj.SetActive(false);
            }
        }

        Debug.Log($"LogoPuzzleSnap gestartet: {puzzlePieces.Length} Teile, snapDistance={snapDistance}m");
        for (int i = 0; i < puzzlePieces.Length; i++)
        {
            if (puzzlePieces[i] != null && targetPositions[i] != null)
            {
                float dist = Vector3.Distance(puzzlePieces[i].position, targetPositions[i].position);
                Debug.Log($"  [{i}] {puzzlePieces[i].name} -> {targetPositions[i].name} (Abstand: {dist:F3}m)");
            }
            else
            {
                Debug.LogWarning($"  [{i}] Referenz fehlt! piece={puzzlePieces[i]} target={targetPositions[i]}");
            }
        }
    }

    void Update()
    {
        if (puzzleComplete) return;

        for (int i = 0; i < puzzlePieces.Length; i++)
        {
            if (isSnapped[i]) continue;
            if (puzzlePieces[i] == null || targetPositions[i] == null) continue;

            float dist = Vector3.Distance(puzzlePieces[i].position, targetPositions[i].position);

            if (dist <= snapDistance)
            {
                SnapPiece(i);
            }
        }
    }

    void SnapPiece(int index)
    {
        isSnapped[index] = true;
        snappedCount++;

        Transform piece = puzzlePieces[index];
        Transform target = targetPositions[index];

        // Grab-Interaktion deaktivieren, damit der Grab das Teil loslässt
        DisableGrab(piece);

        // Position zum Ziel, Rotation 180° um Z korrigieren
        piece.position = target.position;
        piece.rotation = target.rotation * Quaternion.Euler(0, 0, 180);

        if (lockAfterSnap)
        {
            Rigidbody rb = piece.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.isKinematic = true;
            }
        }

        // Sound
        if (snapSound != null && audioSource != null)
            audioSource.PlayOneShot(snapSound);

        Debug.Log($"Teil '{piece.name}' eingerastet! ({snappedCount}/{puzzlePieces.Length})");

        // Rathaus-Passage (Index 2): Audio NICHT sofort — erst bei alle komplett
        // Hamburg (0) und HAW Hamburg (1): Audio sofort abspielen
        if (index != rathausPassageIndex)
        {
            if (lobAudioProLogo != null && index < lobAudioProLogo.Length && lobAudioProLogo[index] != null)
            {
                audioSource.PlayOneShot(lobAudioProLogo[index]);
                Debug.Log($"Audio Lob [{index}]: {lobAudioProLogo[index].name}");
                ZeigeUntertitel(index);
            }
        }

        // Alle fertig? Bilder + Rathaus-Passage-Audio abspielen
        if (snappedCount >= puzzlePieces.Length)
        {
            puzzleComplete = true;
            StartCoroutine(OnPuzzleComplete());
        }
    }

    // Zeigt den Untertitel-Text zum Logo synchron zur laufenden Audio-Wiedergabe.
    void ZeigeUntertitel(int index)
    {
        if (untertitelAnzeige == null) return;
        if (lobUntertitelProLogo == null || index < 0 || index >= lobUntertitelProLogo.Length) return;
        string text = lobUntertitelProLogo[index];
        if (string.IsNullOrEmpty(text)) return;
        untertitelAnzeige.ZeigeWaehrend(text, audioSource);
    }

    void DisableGrab(Transform piece)
    {
        // HandGrab-Child finden und Grabbable/Interactables deaktivieren
        Transform handGrab = piece.Find("[BuildingBlock] HandGrab");
        if (handGrab == null) return;

        // Grabbable deaktivieren — erzwingt Release
        var grabbable = handGrab.GetComponent<Grabbable>();
        if (grabbable != null)
            grabbable.enabled = false;

        var handGrabInteractable = handGrab.GetComponent<HandGrabInteractable>();
        if (handGrabInteractable != null)
            handGrabInteractable.enabled = false;

        var grabInteractable = handGrab.GetComponent<GrabInteractable>();
        if (grabInteractable != null)
            grabInteractable.enabled = false;
    }

    void EnableGrab(Transform piece)
    {
        Transform handGrab = piece.Find("[BuildingBlock] HandGrab");
        if (handGrab == null) return;

        var grabbable = handGrab.GetComponent<Grabbable>();
        if (grabbable != null)
            grabbable.enabled = true;

        var handGrabInteractable = handGrab.GetComponent<HandGrabInteractable>();
        if (handGrabInteractable != null)
            handGrabInteractable.enabled = true;

        var grabInteractable = handGrab.GetComponent<GrabInteractable>();
        if (grabInteractable != null)
            grabInteractable.enabled = true;
    }

    IEnumerator OnPuzzleComplete()
    {
        Debug.Log("PUZZLE KOMPLETT! Hamburg, HAW Hamburg & Rathaus-Passage-Hamburg Logos zusammengesetzt!");

        // Erfolgs-Effekt: Alle Teile kurz aufleuchten
        List<Material> mats = new List<Material>();
        List<Color> originalColors = new List<Color>();

        foreach (Transform piece in puzzlePieces)
        {
            if (piece == null) continue;
            MeshRenderer mr = piece.GetComponent<MeshRenderer>();
            if (mr != null && mr.material != null)
            {
                mats.Add(mr.material);
                originalColors.Add(mr.material.color);
            }
        }

        // Warten bis vorheriges Lob-Audio (Hamburg/HAW) fertig ist
        while (audioSource.isPlaying)
            yield return null;

        // 3x Pulsieren
        for (int pulse = 0; pulse < 3; pulse++)
        {
            for (int i = 0; i < mats.Count; i++)
                mats[i].color = successGlowColor;
            yield return new WaitForSeconds(0.2f);

            for (int i = 0; i < mats.Count; i++)
                mats[i].color = originalColors[i];
            yield return new WaitForSeconds(0.2f);
        }

        // Belohnungs-Bilder einblenden (VOR dem Rathaus-Passage-Audio)
        if (belohnungsBilder != null)
        {
            foreach (var bild in belohnungsBilder)
            {
                if (bild != null) bild.SetActive(true);
            }
            Debug.Log("Belohnungs-Bilder eingeblendet!");
        }

        // Rathaus-Passage-Audio abspielen (das "alle komplett"-Audio)
        if (lobAudioProLogo != null && rathausPassageIndex < lobAudioProLogo.Length
            && lobAudioProLogo[rathausPassageIndex] != null)
        {
            audioSource.PlayOneShot(lobAudioProLogo[rathausPassageIndex]);
            Debug.Log($"Audio Rathaus-Passage [{rathausPassageIndex}]: {lobAudioProLogo[rathausPassageIndex].name}");
            // Zeitlich getaktete Untertitel-Segmente starten.
            if (untertitelAnzeige != null) untertitelAnzeige.ZeigeSegmente(endSegmente);

            // Warten bis Clip UND Untertitel durch sind (die Untertitel laufen
            // länger als der kurze Lob-Clip), erst dann geht es mit Challenge 2 weiter.
            float warten = lobAudioProLogo[rathausPassageIndex].length;
            if (untertitelAnzeige != null && endSegmente != null)
                foreach (var segment in endSegmente)
                    warten = Mathf.Max(warten, segment.ende);
            yield return new WaitForSeconds(warten + 0.5f);
        }

        // alleRichtigAudio falls zugewiesen (aktuell null)
        if (alleRichtigAudio != null)
        {
            audioSource.PlayOneShot(alleRichtigAudio);
            Debug.Log($"Audio Ansage: {alleRichtigAudio.name}");
            yield return new WaitForSeconds(alleRichtigAudio.length + 0.5f);
        }

        // Nach-Erfolg-Objekte einblenden (Panel-Update etc.)
        audioAnnouncementDone = true;
        if (nachErfolgEinblenden != null)
        {
            foreach (var obj in nachErfolgEinblenden)
            {
                if (obj != null) obj.SetActive(true);
            }
            Debug.Log("Nach-Erfolg-Objekte eingeblendet (nach Audio-Ende)!");
        }
    }

    public void ResetPuzzle()
    {
        puzzleComplete = false;
        audioAnnouncementDone = false;
        snappedCount = 0;

        for (int i = 0; i < isSnapped.Length; i++)
            isSnapped[i] = false;

        // Belohnungs-Bilder wieder verstecken
        if (belohnungsBilder != null)
        {
            foreach (var bild in belohnungsBilder)
            {
                if (bild != null) bild.SetActive(false);
            }
        }

        // Nach-Erfolg-Objekte wieder verstecken
        if (nachErfolgEinblenden != null)
        {
            foreach (var obj in nachErfolgEinblenden)
            {
                if (obj != null) obj.SetActive(false);
            }
        }

        // Grab wieder aktivieren und Rigidbodies freigeben
        foreach (Transform piece in puzzlePieces)
        {
            if (piece == null) continue;
            EnableGrab(piece);

            if (!lockAfterSnap) continue;
            Rigidbody rb = piece.GetComponent<Rigidbody>();
            if (rb != null)
                rb.isKinematic = true;
        }
    }
}
