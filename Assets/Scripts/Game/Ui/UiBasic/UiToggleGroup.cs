using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


[RequireComponent(typeof(ToggleGroup))]
public class UiToggleGroup : BaseUiBasic
{
    [SerializeField] private ToggleGroup _ToggleGroup;
    [SerializeField] private List<Toggle> _Toggles = new();



    private UnityAction<bool, int> _OnToggle;

    public void InitToggle(UnityAction<bool, int> onToggle)
    {
        _OnToggle = onToggle;
        for (int i = 0; i < _Toggles.Count; i++)
        {
            _ToggleGroup.RegisterToggle(_Toggles[i]);
            _Toggles[i].group = _ToggleGroup;
            int index = i;
            _Toggles[i].onValueChanged.AddListener((isOn)=> { _OnToggle?.Invoke(isOn, index); });
        }
    }

    public void SetDefaultToggle(int index)
    {
        if (_Toggles.Count == 0) return;
        _Toggles[index].isOn = true;
    }
}
