using UnityEngine;
using UnityEngine.UI;




public class UiCycleScroll : UiBaseScroll
{
    [SerializeField] private ScrollRect _Scroll;

    public RectTransform Viewport { get => _Scroll.viewport; }


    protected int mCurMinIndex, mCurMaxIndex;
    private Vector3 mPreContentPos;

    public override void InitScroll()
    {
        base.InitScroll();

        mCurMinIndex = 0;
        mCurMaxIndex = transform.childCount - 1;
        _Scroll.onValueChanged.AddListener(OnScrollValueChanged);
    }

    public override void SetLayoutHorizontal()
    {
        SetCellPosition(0);
    }

    public override void SetLayoutVertical()
    {
        SetCellPosition(1);
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
    private void SetCellPosition(int axis)
    {
        if (axis == 0)
        {
            InitCellRect(Vector2.up, Vector2.up);
        }
        else
        {
            InitContentRect();
            InitCellPosition();
        }
    }

    private void InitContentRect()
    {
        Vector2 size;
        if (mAxisHorizontal)
        {
            SetContentAnchors(Vector2.zero, Vector2.up);
        }
        else
        {
            SetContentAnchors(Vector2.up, Vector2.one);
        }
        size.x = mColumnCount * CellSize.x + mColumnCount * Spacing.x + Padding.left - Spacing.x;
        size.y = mRowCount * CellSize.y + mRowCount * Spacing.y + Padding.top - Spacing.y;
        Content.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x);
        Content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y);
    }

    private void SetContentAnchors(Vector2 min, Vector2 max)
    {
        Content.anchorMin = min;
        Content.anchorMax = max;
    }

    private void InitCellPosition()
    {
        for (int i = 0; i < mCellList.Count; i++)
        {
            var pos = GetCellPosition(i);
            var rect = mCellList[i].rectTransform;
            rect.anchoredPosition = pos;
        }
    }

    public Vector2 GetCellPosition(int index)
    {
        Vector2 pos = Vector2.zero;
        if (Axis == GridLayoutGroup.Axis.Horizontal)
        {
            int row = index % mRowCount;
            int column = index / mRowCount;

            pos.x = column * CellSize.x + column * Spacing.x + Padding.left + CellHalfSize.x;
            pos.y = -(row * CellSize.y + row * Spacing.y + Padding.top + CellHalfSize.y);
        }
        else if (Axis == GridLayoutGroup.Axis.Vertical)
        {
            int row = index / mColumnCount;
            int column = index % mColumnCount;

            pos.x = column * CellSize.x + column * Spacing.x + Padding.left + CellHalfSize.x;
            pos.y = -(row * CellSize.y + row * Spacing.y + Padding.top + CellHalfSize.y);
        }
        return pos;
    }
}
