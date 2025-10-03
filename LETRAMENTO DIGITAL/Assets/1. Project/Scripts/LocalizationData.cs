using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LocalizationData
{
    public List<LocalizedText> texts = new List<LocalizedText>();
    
    /// <summary>
    /// Gets localized text by ID and language
    /// </summary>
    public string GetText(string id, SystemLanguage language)
    {
        var text = texts.Find(t => t.id == id);
        if (text == null)
        {
            Debug.LogWarning($"Text with ID '{id}' not found in localization data");
            return $"[MISSING: {id}]";
        }
        
        return language switch
        {
            SystemLanguage.Portuguese => text.portuguese,
            SystemLanguage.English => text.english,
            _ => text.english
        };
    }
    
    /// <summary>
    /// Checks if text ID exists in the data
    /// </summary>
    public bool HasText(string id)
    {
        return texts.Exists(t => t.id == id);
    }
    
    /// <summary>
    /// Adds new localized text entry
    /// </summary>
    public void AddText(string id, string portuguese, string english)
    {
        if (HasText(id))
        {
            Debug.LogWarning($"Text with ID '{id}' already exists");
            return;
        }
        
        texts.Add(new LocalizedText
        {
            id = id,
            portuguese = portuguese,
            english = english
        });
    }
}

[Serializable]
public class LocalizedText
{
    public string id;
    public string portuguese;
    public string english;
}