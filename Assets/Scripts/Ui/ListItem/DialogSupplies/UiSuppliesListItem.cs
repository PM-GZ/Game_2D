using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;



public class UiSuppliesListItem : UiItemBase
{
    [SerializeField] private UiButton _btn;
    [SerializeField] private Image _icon;
    [SerializeField] private TMP_Text _num;
    [SerializeField] private GameObject _highLit;


    public bool isSelect { get; private set; }

    public void SetData(Action onClick = null)
    {
        _btn.onClick = onClick;
    }

    public void SetHighLit()
    {
        isSelect = !_highLit.activeSelf;
        _highLit.SetActive(isSelect);
    }
}
