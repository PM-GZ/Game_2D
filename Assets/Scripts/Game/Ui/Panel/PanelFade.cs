using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System;

[PanelBind("PanelFade", PanelType.Top, true)]
public class PanelFade : PanelBase
{
    [UiBind("Bg")] private Image _img;

    public override void OnStart()
    {
    }

    public override void OnEnable()
    {
    }

    public void StartFade()
    {
        _img.DOFade(1, 0.5f).SetLink(gameObject);
    }

    public void EndFade()
    {
        _img.DOFade(0, 0.5f).OnComplete(() => { Close(); }).SetLink(gameObject);
    }
}