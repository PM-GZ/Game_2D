using System;
using UnityEngine;
using UnityEngine.EventSystems;




public class UiLoopScroll : UiBaseScroll
{
    [Range(0, 1)] public float Radius;
    public float OffsetY;
    [Range(0, 1)] public float SizeRate;
    public bool Fade;
    public RectOffset FadeRange;

    private float mLength;
    private float mOffsetRadius;
    private Vector2 mCenter;
    private int Median { get => mCellList.Count / 2; }
    private float mAngle;


    public override void OnBeginDrag(PointerEventData eventData)
    {
        base.OnBeginDrag(eventData);
    }

    public override void OnDrag(PointerEventData eventData)
    {
        base.OnDrag(eventData);
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);
    }

    public override void InitScroll()
    {
        base.InitScroll();

        mLength = (CellSize.x + Spacing.x) * transform.childCount;
        mOffsetRadius = 1f / transform.childCount;
        mCenter = rectTransform.rect.size / 2;
        SetCellPos();
        SetCellSibling();
    }

    private void SetCellPos()
    {
        mAngle = 360f / mCellList.Count * Mathf.Deg2Rad;
        for (int i = 0; i < transform.childCount; i++)
        {
            var cell = transform.GetChild(i);
            Vector3 pos = GetCellPos(i);
            cell.localPosition = pos;
            float size = GetScaleTimes(i * mOffsetRadius, 1, 0.5f);
            cell.localScale = Vector3.one * size;
        }
    }

    private Vector3 GetCellPos(int index)
    {
        Vector3 pos = Vector3.zero;
        pos.x = GetX(index * mOffsetRadius, mLength);
        return pos;
    }

    private float GetX(float ratio, float length)
    {
        if (0 <= ratio && ratio < 0.25f)
        {
            return length * ratio;
        }
        else if (0.25f <= ratio && ratio < 0.75f)
        {
            return length * (0.5f - ratio);
        }
        else
        {
            return length * (ratio - 1);
        }
    }

    public float GetScaleTimes(float radio, float max, float min)
    {
        float scaleOffset = (max - min) / 0.5f;
        if (radio < 0.5f)
        {
            return max - scaleOffset * radio;
        }
        else
        {
            return max - scaleOffset * (1 - radio);
        }
    }

    private void SetCellSibling()
    {
        for (int i = 0; i < Median; i++)
        {

        }
    }
}