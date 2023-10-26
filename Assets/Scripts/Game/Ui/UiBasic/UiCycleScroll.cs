using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;




[RequireComponent(typeof(ScrollRect))]
public class UiCycleScroll : UiBaseBasic, ILayoutGroup
{
    [SerializeField] private ScrollRect _Scroll;

    public RectTransform Content { get => _Scroll.content; }
    public RectTransform Viewport { get => _Scroll.viewport; }

    public GridLayoutGroup.Axis Axis;
    private GridLayoutGroup.Constraint mConstraint;
    public int ConstraintCount;
    public TextAnchor ChildAlignment;
    public Vector2 CellSize;
    public Vector2 Spaceing;
    public RectOffset Padding;


    private List<RectTransform> mCellList;
    private int mRowCount, mColumnCount;


    //public void SetGroup<UiBaseItem>



    public void SetLayoutHorizontal()
    {
        SetCellPosition(0);
    }

    public void SetLayoutVertical()
    {
        SetCellPosition(1);
    }

    private void SetCellPosition(int axis)
    {
        if (axis == 0)
        {
            SetScrollView();
            InitCells();
        }
        else
        {
            CalculateRowAndColnumCount();
            InitContent();
            SetChildrenInitPos();
        }
    }

    private void SetScrollView()
    {
        bool hor = Axis == GridLayoutGroup.Axis.Horizontal;
        _Scroll.horizontal = hor;
        _Scroll.vertical = !hor;
    }

    private void InitContent()
    {
        if(Axis == GridLayoutGroup.Axis.Horizontal)
        {
            SetContentRect(Vector2.up, Vector2.zero);
            mConstraint = GridLayoutGroup.Constraint.FixedRowCount;
        }
        else
        {
            SetContentRect(Vector2.one, Vector2.up);
            mConstraint = GridLayoutGroup.Constraint.FixedColumnCount;
        }
        float x = CellSize.x * mColumnCount + Padding.left + Spaceing.x * mColumnCount - Spaceing.x;
        float y = CellSize.y * mRowCount + Padding.top + Spaceing.y * mRowCount - Spaceing.y;
        Content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, x);
        Content.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, y);
    }

    private void SetContentRect(Vector2 anchorMax, Vector2 anchorMin)
    {
        Content.anchorMax = anchorMax;
        Content.anchorMin = anchorMin;
    }

    private void InitCells()
    {
        mCellList = Content.GetComponentsInChildren<RectTransform>().ToList();
        mCellList.Remove(Content);

        foreach (var cell in mCellList)
        {
            cell.anchorMin = Vector2.up;
            cell.anchorMax = Vector2.up;
            cell.sizeDelta = CellSize;
        }
    }

    private void CalculateRowAndColnumCount()
    {
        int cellCount = mCellList.Count;
        float width = Content.rect.size.x;
        float height = Content.rect.size.y;

        if (mConstraint == GridLayoutGroup.Constraint.FixedColumnCount)
        {
            mRowCount = ConstraintCount;

            if (cellCount > mRowCount)
                mColumnCount = cellCount / mRowCount + (cellCount % mRowCount == 0 ? 0 : 1);
        }
        else if (mConstraint == GridLayoutGroup.Constraint.FixedRowCount)
        {
            mColumnCount = ConstraintCount;

            if (cellCount > mColumnCount)
                mRowCount = cellCount / mColumnCount + (cellCount % mColumnCount == 0 ? 0 : 1);
        }
        else
        {
            if (CellSize.x + Spaceing.x <= 0)
                mRowCount = int.MaxValue;
            else
                mRowCount = Mathf.Max(1, Mathf.FloorToInt((width - Padding.horizontal + Spaceing.x + 0.001f) / (CellSize.x + Spaceing.x)));

            if (CellSize.y + Spaceing.y <= 0)
                mColumnCount = int.MaxValue;
            else
                mColumnCount = Mathf.Max(1, Mathf.FloorToInt((height - Padding.vertical + Spaceing.y + 0.001f) / (CellSize.y + Spaceing.y)));
        }
    }

    private void SetChildrenInitPos()
    {
        int childrenCount = mCellList.Count;

        int cellsPerMainAxis, actualCellCountX, actualCellCountY;
        if (Axis == GridLayoutGroup.Axis.Horizontal)
        {
            cellsPerMainAxis = mRowCount;
            actualCellCountX = Mathf.Clamp(mRowCount, 1, childrenCount);
            actualCellCountY = Mathf.Clamp(mColumnCount, 1, Mathf.CeilToInt(childrenCount / (float)cellsPerMainAxis));
        }
        else
        {
            cellsPerMainAxis = mColumnCount;
            actualCellCountY = Mathf.Clamp(mColumnCount, 1, childrenCount);
            actualCellCountX = Mathf.Clamp(mRowCount, 1, Mathf.CeilToInt(childrenCount / (float)cellsPerMainAxis));
        }

        Vector2 requiredSpace = new Vector2(
            actualCellCountX * CellSize.x + (actualCellCountX - 1) * Spaceing.x,
            actualCellCountY * CellSize.y + (actualCellCountY - 1) * Spaceing.y
        );
        Vector2 startOffset = new Vector2(
            GetStartOffset(0, requiredSpace.x),
            GetStartOffset(1, requiredSpace.y)
        );

        for (int i = 0; i < childrenCount; i++)
        {
            int positionX;
            int positionY;
            if (Axis == GridLayoutGroup.Axis.Horizontal)
            {
                positionX = i % cellsPerMainAxis;
                positionY = i / cellsPerMainAxis;
            }
            else
            {
                positionX = i / cellsPerMainAxis;
                positionY = i % cellsPerMainAxis;
            }

            SetChildAxis(mCellList[i], 0, startOffset.x + (CellSize[0] + Spaceing[0]) * positionX, CellSize[0]);
            SetChildAxis(mCellList[i], 1, startOffset.y + (CellSize[1] + Spaceing[1]) * positionY, CellSize[1]);
        }
    }
    private float GetStartOffset(int axis, float requiredSpaceWithoutPadding)
    {
        float requiredSpace = requiredSpaceWithoutPadding + (axis == 0 ? Padding.horizontal : Padding.vertical);
        float availableSpace = rectTransform.rect.size[axis];
        float surplusSpace = availableSpace - requiredSpace;
        float alignmentOnAxis = GetAlignmentOnAxis(axis);
        return (axis == 0 ? Padding.left : Padding.top) + surplusSpace * alignmentOnAxis;
    }

    private float GetAlignmentOnAxis(int axis)
    {
        if (axis == 0)
            return ((int)ChildAlignment % 3) * 0.5f;
        else
            return ((int)ChildAlignment / 3) * 0.5f;
    }

    private void SetChildAxis(RectTransform rect, int axis, float pos, float size)
    {
        if (rect == null)
            return;

        Vector2 anchoredPosition = rect.anchoredPosition;
        anchoredPosition[axis] = (axis == 0) ? (pos + size * rect.pivot[axis] * 1) : (-pos - size * (1f - rect.pivot[axis]) * 1);
        rect.anchoredPosition = anchoredPosition;
    }
}
