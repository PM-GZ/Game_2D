using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class UiBaseScroll : UiBaseBasic, ILayoutGroup, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public enum ScrollType
    {
        Loop,
        Cycle,
    }

    public struct CycleScrollData
    {
        public int DataCount;
        public int CellCount;
        public Action<int> OnCellCreate;
        public Action<int> OnCellMove;
        public Action<int> OnCellSelected;
        public Action OnCellCreated;
    }

    public ScrollType _ScrollType;
    public RectTransform Content;
    public GridLayoutGroup.Axis Axis;
    [Min(1)] public int AxisCount = 1;
    public Vector2 CellSize, Spacing;
    public RectOffset Padding;

    protected const int DEFAULT_MAX_CELL_COUNT = 30;
    protected Vector2 CellHalfSize { get => CellSize / 2; }
    protected List<UiBaseItem> mCellList;
    protected int mRowCount, mColumnCount;

    protected Transform mParent;
    protected bool mAxisHorizontal;
    protected CycleScrollData mScrollData;
    protected int mCurMinIndex, mCurMaxIndex;
    protected Vector2 mDelta;



    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        mDelta = eventData.delta;
    }

    public virtual void OnEndDrag(PointerEventData eventData)
    {

    }


    public void SetCycleCellGroup<T>(CycleScrollData data) where T : UiBaseItem
    {
        mScrollData = data;
        CalculateCellCount();
        ClearCells();
        CreateAllCell<T>();
        InitScroll();
    }

    private void CalculateCellCount()
    {
        var data = mScrollData;
        if (data.CellCount == 0)
        {
            data.CellCount = data.DataCount > DEFAULT_MAX_CELL_COUNT ? DEFAULT_MAX_CELL_COUNT : data.DataCount;
        }
        else
        {
            data.CellCount = data.DataCount > data.CellCount ? data.CellCount : data.DataCount;
        }
        mCurMinIndex = 0;
        mCurMaxIndex = data.CellCount - 1;
    }

    private void ClearCells()
    {
        if (mCellList == null) return;

        for (int i = 0; i < transform.childCount; i++)
        {
            Destroy(mCellList[i].gameObject);
        }
        mCellList?.Clear();
    }

    protected void CreateAllCell<T>() where T : UiBaseItem
    {
        mCellList ??= new List<UiBaseItem>();
        for (int i = 0; i < mCurMaxIndex + 1; i++)
        {
            var cell = UiUtility.CreateItem<T>(Content) as UiBaseItem;
            mCellList.Add(cell);
            mScrollData.OnCellCreate?.Invoke(i);
        }
        mScrollData.OnCellCreated?.Invoke();
    }

    public virtual void InitScroll()
    {
        
    }
    

    public virtual void SetLayoutHorizontal()
    {
        SetCellPosition(0);
    }

    public virtual void SetLayoutVertical()
    {
        SetCellPosition(1);
    }

    private void SetCellPosition(int axis)
    {
        if (axis == 0)
        {
            InitCellRect();
        }
        else
        {
            if (_ScrollType == ScrollType.Loop) return;

            CalculateRowAndColnumCount();
            InitContentRect();
            InitCellPosition();
        }
    }

    private void InitContentRect()
    {
        if (_ScrollType == ScrollType.Cycle) return;

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

    private void InitCellRect()
    {
        mCellList = GetComponentsInChildren<UiBaseItem>().ToList();

        foreach (var cell in mCellList)
        {
            var rect = cell.rectTransform;
            rect.anchorMin = Vector2.up;
            rect.anchorMax = Vector2.up;
            rect.sizeDelta = CellSize;
        }
    }

    private void CalculateRowAndColnumCount()
    {
        float width = rectTransform.rect.size.x;
        float height = rectTransform.rect.size.y;

        if (Axis == GridLayoutGroup.Axis.Vertical)
        {
            mAxisHorizontal = false;
            mColumnCount = AxisCount;

            if (mScrollData.DataCount > mColumnCount)
                mRowCount = mScrollData.DataCount / mColumnCount + (mScrollData.DataCount % mColumnCount == 0 ? 0 : 1);
        }
        else if (Axis == GridLayoutGroup.Axis.Horizontal)
        {
            mAxisHorizontal = true;
            mRowCount = AxisCount;

            if (mScrollData.DataCount > mRowCount)
                mColumnCount = mScrollData.DataCount / mRowCount + (mScrollData.DataCount % mRowCount == 0 ? 0 : 1);
        }
        else
        {
            if (CellSize.x + Spacing.x <= 0)
                mRowCount = int.MaxValue;
            else
                mRowCount = Mathf.Max(1, Mathf.FloorToInt((width - Padding.horizontal + Spacing.x + 0.001f) / (CellSize.x + Spacing.x)));

            if (CellSize.y + Spacing.y <= 0)
                mColumnCount = int.MaxValue;
            else
                mColumnCount = Mathf.Max(1, Mathf.FloorToInt((height - Padding.vertical + Spacing.y + 0.001f) / (CellSize.y + Spacing.y)));
        }
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

    public void UpdateInspector()
    {
        SetLayoutHorizontal();
        SetLayoutVertical();
        OnInspectorUpdate();
    }

    protected virtual void OnInspectorUpdate()
    {

    }
}
