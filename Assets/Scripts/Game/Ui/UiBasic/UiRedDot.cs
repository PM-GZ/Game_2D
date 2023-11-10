using System;
using TMPro;
using UnityEngine;
using RedDotPos = RedDotSystem.RedDotPos;
using RedDotType = RedDotSystem.RedDotType;



public class UiRedDot : UiBaseBasic
{

    public RedDotPos Pos = RedDotPos.RightTop;
    public RedDotType RedType = RedDotType.Dot;
    public GameObject RedDot;
    public TMP_Text Number;

    public void SetData(RedDotPos pos, RedDotType type)
    {
        this.Pos = pos;
        this.RedType = type;

        SetRedDotPos();
        SetRedDotType();
    }

    private void SetRedDotType()
    {
        switch (RedType)
        {
            case RedDotType.Dot:
                Number.gameObject.SetActive(false);
                break;
            case RedDotType.Number:
                Number.gameObject.SetActive(true);
                break;
        }
    }

    private void SetRedDotPos()
    {
        switch (Pos)
        {
            case RedDotPos.LeftTop:
                SetRedDotRect(Vector2.up, Vector2.up, Vector2.up);
                break;
            case RedDotPos.RightTop:
                SetRedDotRect(Vector2.one, Vector2.one, Vector2.one);
                break;
            case RedDotPos.Center:
                SetRedDotRect(Vector2.one / 2, Vector2.one / 2, Vector2.one / 2);
                break;
            case RedDotPos.LeftBottom:
                SetRedDotRect(Vector2.zero, Vector2.zero, Vector2.zero);
                break;
            case RedDotPos.RightBottom:
                SetRedDotRect(Vector2.right, Vector2.right, Vector2.right);
                break;
        }
        rectTransform.anchoredPosition = Vector2.zero;
    }

    public void SetRedDotPos(Vector2 pos)
    {
        SetRedDotRect(Vector2.one / 2, Vector2.one / 2, Vector2.one / 2);
        rectTransform.anchoredPosition = pos;
    }

    private void SetRedDotRect(Vector2 min, Vector2 max, Vector2 pivot)
    {
        rectTransform.anchorMin = min;
        rectTransform.anchorMax = max;
        rectTransform.pivot = pivot;
    }
}