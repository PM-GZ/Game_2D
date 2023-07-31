using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;



public class UiSuppliesListItem : UiItemBase
{
    [SerializeField] private Image _icon;
    [SerializeField] private TMP_Text _num;
    [SerializeField] private GameObject _highLit;



    public void SetData(Action onClick = null)
    {
        onPointClick = onClick;
    }

    public void SetHighLit()
    {
        isSelect = !_highLit.activeSelf;
        _highLit.SetActive(isSelect);
    }
}
