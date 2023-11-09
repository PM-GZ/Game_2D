using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;



public class UiLoopScroll : UiBaseScroll
{
    public float AutoResetPoseSpeed;


    private int mDisplayCellIndex, mDisplayDataIndex;
    private float mMinRect, mMaxRect;
    private Vector2[] mInitCellPosArray;
    private Tweener mTween;
    private IEnumerator mAutoPoseCoroutine;



    public override void OnBeginDrag(PointerEventData eventData)
    {
        base.OnBeginDrag(eventData);
        mTween?.Kill();
        if (mAutoPoseCoroutine != null)
        {
            StopCoroutine(mAutoPoseCoroutine);
        }
    }

    public override void OnDrag(PointerEventData eventData)
    {
        base.OnDrag(eventData);
        OnScrollValueChanged();
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);
        float delta = (mAxisHorizontal ? mDelta.x : mDelta.y);
        float time = Mathf.Abs(delta / 100);
        mTween = DOTween.To(MoveBuffer, delta, 0, time).OnComplete(AutoRestPose).SetLink(gameObject);
    }

    private void MoveBuffer(float a)
    {
        mDelta.x = mDelta.y = a;
        OnScrollValueChanged();
    }

    private void AutoRestPose()
    {
        mAutoPoseCoroutine = StartAutoReset();
        StartCoroutine(mAutoPoseCoroutine);
    }

    private IEnumerator StartAutoReset()
    {
        var cell = transform.GetChild(mDisplayCellIndex);
        while (!CheckCellPosDrawNearZero(cell))
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i);
                child.localPosition = Vector2.MoveTowards(child.localPosition, mInitCellPosArray[i], Time.deltaTime * AutoResetPoseSpeed);
            }
            yield return null;
        }
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).localPosition = mInitCellPosArray[i];
        }
        mAutoPoseCoroutine = null;
    }

    private bool CheckCellPosDrawNearZero(Transform cell)
    {
        if (mAxisHorizontal)
        {
            if (cell.localPosition.x >= -0.01f && cell.localPosition.x <= 0.01f)
                return true;
        }
        else
        {
            if (cell.localPosition.y >= -0.01f && cell.localPosition.y <= 0.01f)
                return true;
        }
        return false;
    }

    public override void InitScroll()
    {
        base.InitScroll();
        mInitCellPosArray = new Vector2[transform.childCount];
        mDisplayCellIndex = mDisplayDataIndex = transform.childCount / 2;
        InitCellRect(Vector2.one / 2, Vector2.one / 2);
        InitCellPosition();
        InitMinAndMaxRect();
    }

    private void InitMinAndMaxRect()
    {
        if (mAxisHorizontal)
        {
            mMinRect = transform.GetChild(0).localPosition.x - CellHalfSize.x;
            mMaxRect = transform.GetChild(transform.childCount - 1).localPosition.x + CellHalfSize.x;
        }
        else
        {
            mMinRect = transform.GetChild(0).localPosition.y + CellHalfSize.y;
            mMaxRect = transform.GetChild(transform.childCount - 1).localPosition.y - CellHalfSize.y;
        }
    }

    private void OnScrollValueChanged()
    {
        mDelta.x = Mathf.Clamp(mDelta.x, -CellHalfSize.x, CellHalfSize.x);
        mDelta.y = Mathf.Clamp(mDelta.x, -CellHalfSize.y, CellHalfSize.y);
        if (Axis == GridLayoutGroup.Axis.Horizontal)
        {
            MoveCells(new Vector2(mDelta.x, 0));
            if (mDelta.x < 0)
            {
                CellMoveToRight();
            }
            else if (mDelta.x > 0)
            {
                CellMoveToLeft();
            }
        }
        else if (Axis == GridLayoutGroup.Axis.Vertical)
        {
            MoveCells(new Vector2(0, mDelta.y));
            if (mDelta.y < 0)
            {
                CellMoveToTop();
            }
            else if (mDelta.y > 0)
            {
                CellMoveToBottom();
            }
        }
    }

    private void MoveCells(Vector2 delta)
    {
        foreach (var item in mCellList)
        {
            item.rectTransform.anchoredPosition += delta;
        }
    }

    private void CellMoveToRight()
    {
        for (int i = 0; i < mRowCount; i++)
        {
            var rect = Content.GetChild(0) as RectTransform;
            if (rect.anchoredPosition.x < mMinRect)
            {
                CellAsLastSibling(rect);
            }
        }
    }

    private void CellMoveToLeft()
    {
        for (int i = 0; i < mRowCount; i++)
        {
            var rect = Content.GetChild(transform.childCount - 1) as RectTransform;
            if (rect.anchoredPosition.x > mMaxRect)
            {
                CellAsFirstSibling(rect);
            }
        }
    }

    private void CellMoveToBottom()
    {
        for (int i = 0; i < mColumnCount; i++)
        {
            var rect = Content.GetChild(0).transform as RectTransform;
            CellAsLastSibling(rect);
        }
    }

    private void CellMoveToTop()
    {
        for (int i = 0; i < mColumnCount; i++)
        {
            var rect = Content.GetChild(mCellList.Count - 1) as RectTransform;
            CellAsFirstSibling(rect);
        }
    }

    private void CellAsLastSibling(RectTransform rect)
    {
        ++mDisplayDataIndex;
        ++mDisplayCellIndex;
        CheckDisplayIndex();
        Vector2 pos = rect.anchoredPosition;
        if (mAxisHorizontal)
        {
            pos.x = (transform.GetChild(transform.childCount - 1) as RectTransform).anchoredPosition.x + CellSize.x + Spacing.x;
        }
        else
        {

        }
        rect.anchoredPosition = pos;
        rect.SetAsLastSibling();
        mScrollData.OnCellMove?.Invoke(mDisplayDataIndex);
    }

    private void CellAsFirstSibling(RectTransform rect)
    {
        --mDisplayDataIndex;
        --mDisplayCellIndex;
        CheckDisplayIndex();
        Vector2 pos = rect.anchoredPosition;
        if (mAxisHorizontal)
        {
            pos.x = (transform.GetChild(0) as RectTransform).anchoredPosition.x - CellSize.x - Spacing.x;
        }
        else
        {

        }
        rect.anchoredPosition = pos;
        rect.SetAsFirstSibling();
        mScrollData.OnCellMove?.Invoke(mDisplayDataIndex);
    }

    private void CheckDisplayIndex()
    {
        if (mDisplayDataIndex < 0)
        {
            mDisplayDataIndex = mScrollData.DataCount - 1;
        }
        else if (mDisplayDataIndex >= mScrollData.DataCount)
        {
            mDisplayDataIndex = 0;
        }

        if (mDisplayCellIndex < 0)
        {
            mDisplayCellIndex = transform.childCount - 1;
        }
        else if (mDisplayCellIndex >= transform.childCount)
        {
            mDisplayCellIndex = 0;
        }
    }

    private void InitCellPosition()
    {
        int index = -mDisplayDataIndex;
        for (int i = 0; i < mCellList.Count; i++)
        {
            var pos = GetCellPosition(index);
            var rect = mCellList[i].rectTransform;
            rect.anchoredPosition = pos;
            index++;
            mInitCellPosArray[i] = pos;
        }
    }

    private Vector2 GetCellPosition(int index)
    {
        Vector2 pos = Vector2.zero;
        if (Axis == GridLayoutGroup.Axis.Horizontal)
        {
            int row = index % mRowCount;
            int column = index / mRowCount;

            pos.x = column * CellSize.x + column * Spacing.x + Padding.left;
            pos.y = -(row * CellSize.y + row * Spacing.y + Padding.top);
        }
        else if (Axis == GridLayoutGroup.Axis.Vertical)
        {
            int row = index / mColumnCount;
            int column = index % mColumnCount;

            pos.x = column * CellSize.x + column * Spacing.x + Padding.left;
            pos.y = -(row * CellSize.y + row * Spacing.y + Padding.top);
        }
        return pos;
    }
}
