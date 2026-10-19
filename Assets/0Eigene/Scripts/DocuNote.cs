using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Dokumentations- und Navigations-Helfer.
/// Text im Inspector eingeben + Objekte als Sprungziele verlinken.
/// </summary>
public class DocuNote : MonoBehaviour
{
    [Header("Dokumentation")]
    [TextArea(3, 10)]
    public string notiz = "";

    [Header("Verlinkte Objekte (Sprungziele)")]
    public List<GameObject> verknuepfteObjekte = new List<GameObject>();
}
