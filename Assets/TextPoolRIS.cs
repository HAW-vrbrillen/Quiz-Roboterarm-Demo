using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TextEntry
{
    public string key;
    public string value;
}

public class TextPoolRIS : MonoBehaviour
{
    // Liste von Text-Einträgen, die im Inspector sichtbar sind
    public List<TextEntry> textEntries = new List<TextEntry>();

    private Dictionary<string, string> textDictionary;

    private void Awake()
    {
        // Initialisiere das Dictionary basierend auf den Einträgen
        textDictionary = new Dictionary<string, string>();
        textEntries.Add(new TextEntry { key = "besonderheit2", value =

            "Die Kabine ist viel heller und geräumiger." +
            "The cabin is much brighter and more spacious. " +
            "La cabine est beaucoup plus lumineuse et spacieuse. " +
            "La cabina es mucho más luminosa y espaciosa." 
             });
        foreach (var entry in textEntries)
        {
            if (!textDictionary.ContainsKey(entry.key.ToLower()))
            {
                textDictionary.Add(entry.key.ToLower(), entry.value);
            }
        }
    }

    // Methode zum Abrufen eines Textes basierend auf dem Key
    public string GetTextById(string textId)
    {
        if (textDictionary.TryGetValue(textId.ToLower(), out string text))
        {
            return text;
        }
        else
        {
            return null; // Gibt null zurück, wenn kein Key gefunden wird
        }
    }
}
