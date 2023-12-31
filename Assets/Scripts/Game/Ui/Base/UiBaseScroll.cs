using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class UiBaseScroll : UiBaseBasic, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public struct CycleScrollData
    {
        public int DataCount;
        public int CellCount;
        public Action<int> OnCellCreate;
        public Action<int> OnCellMove;
        public Action<int> OnCellSelected;
        public Action OnCellCreated;
    }

    public RectTransform Content;
    public GridLayoutGroup.Axis Axis;
    public Vector2 CellSize, Spacing;
    public RectOffset Padding;

    protected const int DEFAULT_MAX_CELL_COUNT = 30;
    protected Vector2 CellHalfSize { get => CellSize / 2; }
    protected List<UiBaseListItem> mCellList;
    protected int mRowCount, mColumnCount;

    protected bool mAxisHorizontal;
    protected CycleScrollData mScrollData;
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


    public void SetCycleCellGroup<T>(CycleScrollData data) where T : UiBaseListItem
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

    protected void CreateAllCell<T>() where T : UiBaseListItem
    {
        mCellList ??= new List<UiBaseListItem>();
        for (int i = 0; i < mScrollData.CellCount; i++)
        {
            var cell = UiUtility.CreateItem<T>(Content) as UiBaseListItem;
            mCellList.Add(cell);
            mScrollData.OnCellCreate?.Invoke(i);
        }
        mScrollData.OnCellCreated?.Invoke();
    }

    public virtual void InitScroll()
    {
        mAxisHorizontal = Axis == GridLayoutGroup.Axis.Horizontal;
        mCellList = GetComponentsInChildren<UiBaseListItem>().ToList();
        CalculateRowAndColnumCount(1);
        SetLayoutHorizontal();
        SetLayoutVertical();
    }

    public virtual void SetLayoutHorizontal()
    {
    }

    public virtual void SetLayoutVertical()
    {
    }

    protected void InitCellRect(Vector2 min, Vector2 max)
    {
        foreach (var cell in mCellList)
        {
            var rect = cell.rectTransform;
            rect.anchorMin = min;
            rect.anchorMax = max;
            rect.sizeDelta = CellSize;
        }
    }

    protected void CalculateRowAndColnumCount(int axisCount)
    {
        if (Axis == GridLayoutGroup.Axis.Vertical)
        {
            mColumnCount = axisCount;

            if (mScrollData.DataCount > mColumnCount)
                mRowCount = mScrollData.DataCount / mColumnCount + (mScrollData.DataCount % mColumnCount == 0 ? 0 : 1);
        }
        else
        {
            mRowCount = axisCount;

            if (mScrollData.DataCount > mRowCount)
                mColumnCount = mScrollData.DataCount / mRowCount + (mScrollData.DataCount % mRowCount == 0 ? 0 : 1);
        }
    }

    public void UpdateInspector()
    {
        InitScroll();
        OnInspectorUpdate();
    }

    protected virtual void OnInspectorUpdate()
    {

    }
}
