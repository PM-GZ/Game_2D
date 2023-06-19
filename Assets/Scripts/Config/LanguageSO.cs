using System;
using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(fileName = "LanguageCfg", menuName = "≈‰÷√/”Ô—‘≈‰÷√")]
public class LanguageSO : ScriptableObject
{
    public List<LanguageData> languages = new();
}

[Serializable]
public struct LanguageData
{
    public string field;
    public List<LanguageText> languages;

    public LanguageData(string field, List<LanguageText> languages)
    {
        this.field = string.Empty;
        this.languages = new()
        {
            new LanguageText
            {
                language = "ºÚ÷–",
                text = string.Empty
            }
        };
    }
}

[Serializable]
public struct LanguageText
{
    public string language;
    public string text;
}
