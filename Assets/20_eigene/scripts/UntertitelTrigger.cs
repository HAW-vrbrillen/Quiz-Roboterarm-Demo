using UnityEngine;

/// <summary>
/// Koppelt einen Untertitel an eine AudioSource: Solange die Quelle spielt, zeigt
/// die UntertitelAnzeige den hinterlegten Text; danach blendet sie automatisch aus.
///
/// Ideal fuer einzelne Narrations-/Ansage-Clips (z.B. in Challenge 2 am Roboterarm):
/// einfach dieses Script auf das GameObject mit der AudioSource legen, "anzeige"
/// zuweisen und den Text eintragen.
///
/// Hinweis: Fuer eine AudioSource, die per PlayOneShot MEHRERE verschiedene Clips
/// abspielt (wie LogoPuzzleSnap), besser direkt im jeweiligen Script
/// anzeige.ZeigeWaehrend(text, source) je Clip aufrufen.
/// </summary>
public class UntertitelTrigger : MonoBehaviour
{
    [Tooltip("Die AudioSource, deren Wiedergabe den Untertitel ausloest. Leer = AudioSource auf diesem GameObject.")]
    public AudioSource audioQuelle;

    [Tooltip("Der anzuzeigende Untertitel-Text (Transkript des Clips).")]
    [TextArea(1, 4)]
    public string untertitelText = "";

    [Tooltip("Die Untertitel-Anzeige (kamerafixiertes Feld).")]
    public UntertitelAnzeige anzeige;

    private bool _warPlaying;

    void Awake()
    {
        if (audioQuelle == null)
            audioQuelle = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (audioQuelle == null || anzeige == null) return;

        bool playing = audioQuelle.isPlaying;
        // Steigende Flanke: Wiedergabe beginnt -> Untertitel zeigen.
        // (Das Ausblenden uebernimmt die Anzeige selbst, sobald die Quelle endet.)
        if (playing && !_warPlaying && !string.IsNullOrEmpty(untertitelText))
            anzeige.ZeigeWaehrend(untertitelText, audioQuelle);

        _warPlaying = playing;
    }
}
