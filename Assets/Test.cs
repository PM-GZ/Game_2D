using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Test : MonoBehaviour
{
    public UiBaseScroll scroll;


    [InitializeOnLoadMethod]
    static void EditorInit()
    {
        TEXT.Init("CN");
    }

    void Start()
    {
        TableRole role = new TableRole();
        foreach (var item in role.mData)
        {
            Debug.Log($"{item.Value.ID}\n");
            Debug.Log($"{item.Value.Star}\n");
            Debug.Log($"{item.Value.RoleName}\n");
            Debug.Log($"{item.Value.PrefabName}\n");
        }

        scroll.SetCycleCellGroup<UiSuppliesListItem>(new UiCycleScroll.CycleScrollData
        {
            DataCount = 20,
            CellCount = 10
        });

        Invoke("ChangeLanguage", 2);
        Invoke("ChangeLanguage2", 4);
    }

    private void ChangeLanguage()
    {
        TEXT.Init("EN");
        var texts = Object.FindObjectsOfType<MultiLanguageText>();
        foreach (var item in texts)
        {
            item.ChangedLanguage();
        }
    }

    private void ChangeLanguage2()
    {
        TEXT.Init("CN");
        var texts = Object.FindObjectsOfType<MultiLanguageText>();
        foreach (var item in texts)
        {
            item.ChangedLanguage();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
