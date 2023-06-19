using System.Collections;
using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

public class MultiColumnTreeView : LanguageTreeView
{
    public MultiColumnTreeView(TreeViewState state, MultiColumnHeader multiColumnHeader) : base(state, multiColumnHeader)
    {
    }

    //public static MultiColumnHeaderState CreateDefaultMultiColumnHeaderState(LanguageSO cfg)
    //{
    //    if (cfg.languages == null || cfg.languages.Count == 0) return null;
    //    var maxColumn = GetMaxColumn(cfg);
    //    for (int i = 0; i < maxColumn; i++)
    //    {
    //        headerContent = new GUIContent("Name");

    //    }
    //}

    private static int GetMaxColumn(LanguageSO cfg)
    {
        int maxColumn = 0;
        foreach (var item in cfg.languages)
        {
            if(item.languages.Count > maxColumn)
            {
                maxColumn = item.languages.Count;
            }
        }
        return maxColumn;
    }
}
