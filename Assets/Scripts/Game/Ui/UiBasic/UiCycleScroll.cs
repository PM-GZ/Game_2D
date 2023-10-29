using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;




public class UiCycleScroll : UiBaseScroll
{
    [SerializeField] private ScrollRect _Scroll;

    public RectTransform Viewport { get => _Scroll.viewport; }


    private Vector3 mPreContentPos;

    public override void InitScroll()
    {
        base.InitScroll();
        _Scroll.onValueChanged.AddListener(OnScrollValueChanged);
    }

    private void OnScrollValueChanged(Vector2 delta)
    {
        if (Axis == GridLayoutGroup.Axis.Horizontal)
        {
            float move = Content.position.x - mPreContentPos.x;

            if (move < 0)
            {
                CellMoveToRight();
            }
            else if (move > 0)
            {
                CellMoveToLeft();
            }
        }
        else if (Axis == GridLayoutGroup.Axis.Vertical)
        {
            float move = Content.position.y - mPreContentPos.y;

            if (move < 0)
            {
                CellMoveToTop();
            }
            else if (move > 0)
            {
                CellMoveToBottom();
            }
        }
        mPreContentPos = Content.position;
    }

    #region Cycle Move Func
    private void CellAsLastSibling(RectTransform rect)
    {
        ++mCurMinIndex;
        rect.anchoredPosition = GetCellPosition(++mCurMaxIndex);
        rect.SetAsLastSibling();
        mScrollData.OnCellMove?.Invoke(mCurMaxIndex);
    }

    private void CellAsFirstSibling(RectTransform rect)
    {
        --mCurMaxIndex;
        rect.anchoredPosition = GetCellPosition(--mCurMinIndex);
        rect.SetAsFirstSibling();
        mScrollData.OnCellMove?.Invoke(mCurMinIndex);
    }

    protected bool CanMove(bool asFirstSibling)
    {
        var rect = (asFirstSibling ? Content.GetChild(Content.childCount - 1) : Content.GetChild(0)) as RectTransform;
        if (Axis == GridLayoutGroup.Axis.Horizontal)
        {
            if (asFirstSibling)
            {
                if (mCurMinIndex == 0 || rect.position.x < Viewport.position.x + Viewport.rect.width + CellHalfSize.x)
                    return false;
            }
            else
            {
                if (mCurMaxIndex == mScrollData.DataCount - 1 || rect.position.x > Viewport.position.x - CellHalfSize.x)
                    return false;
            }
        }
        else
        {
            if (asFirstSibling)
            {
                if (mCurMinIndex == 0 || rect.position.y > -Viewport.rect.height + Viewport.position.y - CellHalfSize.y)
                    return false;
            }
            else
            {
                if (mCurMaxIndex == mScrollData.DataCount - 1 || rect.position.y < Viewport.position.y + CellHalfSize.y)
                    return false;
            }
        }
        return true;
    }

    private void CellMoveToRight()
    {
        for (int i = 0; i < mRowCount; i++)
        {
            if (!CanMove(false)) return;
            var rect = Content.GetChild(0) as RectTransform;
            CellAsLastSibling(rect);
        }
    }

    private void CellMoveToLeft()
    {
        for (int i = 0; i < mRowCount; i++)
        {
            if (!CanMove(true)) return;
            var rect = Content.GetChild(mCellList.Count - 1) as RectTransform;
            CellAsFirstSibling(rect);
        }
    }

    private void CellMoveToBottom()
    {
        for (int i = 0; i < mColumnCount; i++)
        {
            if (!CanMove(false)) return;
            var rect = Content.GetChild(0).transform as RectTransform;
            CellAsLastSibling(rect);
        }
    }

    private void CellMoveToTop()
    {
        for (int i = 0; i < mColumnCount; i++)
        {
            if (!CanMove(true)) return;
            var rect = Content.GetChild(mCellList.Count - 1) as RectTransform;
            CellAsFirstSibling(rect);
        }
    }
    #endregion


    protected override void OnInspectorUpdate()
    {
        SetScrollRect();
    }

    private void SetScrollRect()
    {
        if (_Scroll == null) return;
        bool hor = Axis == GridLayoutGroup.Axis.Horizontal;
        _Scroll.horizontal = hor;
        _Scroll.vertical = !hor;
    }
}
