using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(ScrollRect))]
public class UiCycleScroll : UiBaseBasic, ILayoutGroup
{
    public struct CycleListData
    {
        public int DataCount;
        public int CellCount;
        public Action<int> OnCellCreate;
        public Action<int> OnCellMove;
        public Action<int> OnCellSelected;
        public Action OnCellCreated;
    }

    [SerializeField] private ScrollRect _Scroll;

    public RectTransform Content { get => _Scroll.content; }
    public RectTransform Viewport { get => _Scroll.viewport; }

    public GridLayoutGroup.Axis Axis;
    [Min(1)] public int AxisCount;
    public Vector2 CellSize, Spaceing;
    public RectOffset Padding;

    private const int DEFAULT_MAX_CELL_COUNT = 30;
    private Vector2 CellHalfSize { get => CellSize / 2; }
    private List<UiBaseItem> mCellList;
    private int mRowCount, mColumnCount;

    private CycleListData mCycleListData;
    private Vector2 mContentPosition;
    private int mCurMinIndex, mCurMaxIndex;



    public void SetGroup<T>(CycleListData data) where T : UiBaseItem
    {
        StartSetGroup<T>(data);
    }

    private void StartSetGroup<T>(CycleListData data) where T : UiBaseItem
    {
        if (data.CellCount == 0)
        {
            mCurMaxIndex = data.DataCount > DEFAULT_MAX_CELL_COUNT ? DEFAULT_MAX_CELL_COUNT - 1 : data.DataCount - 1;
        }
        else
        {
            mCurMaxIndex = data.DataCount > data.CellCount ? data.CellCount - 1 : data.DataCount - 1;
        }
        mCycleListData = data;

        mCurMinIndex = 0;
        mContentPosition = Content.position;

        ClearCells();
        CreateAllCell<T>();
        _Scroll.onValueChanged.AddListener(OnScrollValueChanged);
    }

    private void ClearCells()
    {
        for (int i = 0; i < Content.childCount; i++)
        {
            Destroy(Content.GetChild(i).gameObject);
        }
        mCellList?.Clear();
    }

    private void CreateAllCell<T>() where T : UiBaseItem
    {
        mCellList ??= new List<UiBaseItem>();
        for (int i = 0; i < mCurMaxIndex + 1; i++)
        {
            var cell = UiUtility.CreateItem<T>(Content) as UiBaseItem;
            mCellList.Add(cell);
            mCycleListData.OnCellCreate?.Invoke(i);
        }
        mCycleListData.OnCellCreated?.Invoke();
    }

    private void OnScrollValueChanged(Vector2 value)
    {
        if (Axis == GridLayoutGroup.Axis.Horizontal)
        {
            float delta = Content.position.x - mContentPosition.x;

            if (delta < 0)
            {
                CellMoveToRight();
            }
            else if (delta > 0)
            {
                CellMoveToLeft();
            }
        }
        else if (Axis == GridLayoutGroup.Axis.Vertical)
        {
            float delta = Content.position.y - mContentPosition.y;

            if (delta < 0)
            {
                CellMoveToTop();
            }
            else if (delta > 0)
            {
                CellMoveToBottom();
            }
        }
        mContentPosition = Content.position;
    }

    #region Cell Move Funs
    private (Vector3, Vector3) GetScreenPostion(Vector3 cellPos, Vector3 viewportPos)
    {
        cellPos = Main.Ui.GetScreenPostion(cellPos);
        viewportPos = Main.Ui.GetScreenPostion(viewportPos);
        return (cellPos, viewportPos);
    }

    private void CellAsLastSibling(RectTransform rect)
    {
        rect.anchoredPosition = GetCellPosition(++mCurMaxIndex);
        ++mCurMinIndex;
        rect.SetAsLastSibling();
        mCycleListData.OnCellMove?.Invoke(mCurMaxIndex);
    }

    private void CellAsFirstSibling(RectTransform rect)
    {
        rect.anchoredPosition = GetCellPosition(--mCurMinIndex);
        --mCurMaxIndex;
        rect.SetAsFirstSibling();
        mCycleListData.OnCellMove?.Invoke(mCurMinIndex);
    }

    private void CellMoveToRight()
    {
        if (Content.offsetMax.x <= Viewport.rect.width) return;

        for (int i = 0; i < mRowCount; i++)
        {
            var rect = Content.GetChild(0).transform as RectTransform;
            var cellPos = rect.position;
            var viewportPos = Viewport.position;
            if (Main.Ui.RenderMode != RenderMode.ScreenSpaceOverlay)
            {
                (cellPos, viewportPos) = GetScreenPostion(cellPos, viewportPos);
            }
            if (cellPos.x > viewportPos.x - CellHalfSize.x || mCurMaxIndex == mCycleListData.DataCount - 1) return;

            CellAsLastSibling(rect);
        }
    }

    private void CellMoveToLeft()
    {
        if (Content.anchoredPosition.x >= 0) return;

        for (int i = 0; i < mRowCount; i++)
        {
            var rect = Content.GetChild(Content.childCount - 1).transform as RectTransform;
            var cellPos = rect.position;
            var viewportPos = Viewport.position;
            //if (Main.Ui.RenderMode != RenderMode.ScreenSpaceOverlay)
            //{
            //    (cellPos, viewportPos) = GetScreenPostion(cellPos, viewportPos);
            //}
            if (mCurMinIndex == 0 || cellPos.x < viewportPos.x + Viewport.rect.width + CellHalfSize.x) return;

            CellAsFirstSibling(rect);
        }
    }

    private void CellMoveToBottom()
    {
        if (Content.offsetMin.y >= Viewport.rect.height) return;

        for (int i = 0; i < mColumnCount; i++)
        {
            var rect = Content.GetChild(0).transform as RectTransform;
            var cellPos = rect.position;
            var viewportPos = Viewport.position;
            if (Main.Ui.RenderMode != RenderMode.ScreenSpaceOverlay)
            {
                (cellPos, viewportPos) = GetScreenPostion(cellPos, viewportPos);
            }
            if (cellPos.y < viewportPos.y + CellHalfSize.y || mCurMaxIndex == mCycleListData.DataCount - 1) return;

            CellAsLastSibling(rect);
        }
    }

    private void CellMoveToTop()
    {
        if (Content.anchoredPosition.y <= 0) return;

        for (int i = 0; i < mColumnCount; i++)
        {
            var rect = Content.GetChild(Content.childCount - 1).transform as RectTransform;
            var cellPos = rect.position;
            var viewportPos = Viewport.position;
            if(Main.Ui.RenderMode != RenderMode.ScreenSpaceOverlay)
            {
               (cellPos, viewportPos) = GetScreenPostion(cellPos, viewportPos);
            }
            if (mCurMinIndex == 0 || cellPos.y > viewportPos.y - Viewport.rect.height - CellHalfSize.y) return;

            CellAsFirstSibling(rect);
        }
    }
    #endregion

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
            InitCellRect();
        }
        else
        {
            CalculateRowAndColnumCount();
            InitContent();
            InitCellPosition();
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
        if (Axis == GridLayoutGroup.Axis.Horizontal)
        {
            SetContentRect(Vector2.up, Vector2.zero);
        }
        else
        {
            SetContentRect(Vector2.one, Vector2.up);
        }
        float x = CellSize.y * mColumnCount + Padding.top + Spaceing.y * mColumnCount - Spaceing.y;
        float y = CellSize.x * mRowCount + Padding.left + Spaceing.x * mRowCount - Spaceing.x;
        Content.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, x);
        Content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, y);
        Content.anchoredPosition = Vector2.zero;
    }

    private void SetContentRect(Vector2 anchorMax, Vector2 anchorMin)
    {
        Content.anchorMax = anchorMax;
        Content.anchorMin = anchorMin;
    }

    private void InitCellRect()
    {
        mCellList = Content.GetComponentsInChildren<UiBaseItem>().ToList();

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
        float width = Content.rect.size.x;
        float height = Content.rect.size.y;

        if (Axis == GridLayoutGroup.Axis.Vertical)
        {
            mColumnCount = AxisCount;

            if (mCycleListData.DataCount > mColumnCount)
                mRowCount = mCycleListData.DataCount / mColumnCount + (mCycleListData.DataCount % mColumnCount == 0 ? 0 : 1);
        }
        else if (Axis == GridLayoutGroup.Axis.Horizontal)
        {
            mRowCount = AxisCount;

            if (mCycleListData.DataCount > mRowCount)
                mColumnCount = mCycleListData.DataCount / mRowCount + (mCycleListData.DataCount % mRowCount == 0 ? 0 : 1);
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

    private void InitCellPosition()
    {
        for (int i = 0; i < mCellList.Count; i++)
        {
            var pos = GetCellPosition(i);
            var rect = mCellList[i].rectTransform;
            rect.anchoredPosition = pos;
        }
    }

    private Vector2 GetCellPosition(int index)
    {
        Vector2 pos = Vector2.zero;
        if (Axis == GridLayoutGroup.Axis.Horizontal)
        {
            int row = index % mRowCount;
            int column = index / mRowCount;

            pos.x = column * CellSize.x + column * Spaceing.x + Padding.left + CellHalfSize.x;
            pos.y = -(row * CellSize.y + row * Spaceing.y + Padding.top + CellHalfSize.y);
        }
        else if (Axis == GridLayoutGroup.Axis.Vertical)
        {
            int row = index / mColumnCount;
            int column = index % mColumnCount;

            pos.x = column * CellSize.x + column * Spaceing.x + Padding.left + CellHalfSize.x;
            pos.y = -(row * CellSize.y + row * Spaceing.y + Padding.top + CellHalfSize.y);
        }
        return pos;
    }

    public void UpdateInspector()
    {
        SetScrollView();
    }
}
