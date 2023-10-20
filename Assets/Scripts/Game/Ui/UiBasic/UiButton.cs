using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


[RequireComponent(typeof(Button))]
[RequireComponent(typeof(Image))]
public class UiButton : BaseUiBasic, IPointerClickHandler
{
    [SerializeField] private Button _Button;
    [SerializeField] private Image _Image;
    [SerializeField] private TMP_Text _Text;
    public Button button { get => _Button; }
    public Image image { get => _Image; }

    public Action onClick;


    public void SetText(string text)
    {
        _Text.text = text;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        onClick?.Invoke();
    }
}
