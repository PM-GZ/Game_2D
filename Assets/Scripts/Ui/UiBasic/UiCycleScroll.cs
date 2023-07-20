using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;




[RequireComponent(typeof(ScrollRect))]
public class UiCycleScroll : UiLayoutBase
{
    [SerializeField] private ScrollRect _Scroll;
    public RectTransform content { get => _Scroll.content; }
    public RectTransform viewport { get => _Scroll.viewport; }

    public Vector2 childSize = new Vector2(100, 100);
    public Vector2 spacing;
    public GridLayoutGroup.Axis axis;

    private Vector2 _childFullSize
    {
        get
        {
            Vector2 size;
            size.x = childSize.x + padding.left + spacing.x;
            size.y = childSize.y + padding.top + spacing.y;
            return size;
        }
    }

    public int row;
    public int column;
    private int _dataCount;
    private int _childCount;
    public List<UiItemBase> items { get; private set; } = new();
    private Action<UiItemBase, int> _onCreate;
    private Action<UiItemBase, int> _onPosChanged;
    private Action<UiItemBase, int> _onRefresh;
    private Action<UiItemBase, int> _onSelect;
    private Action<UiItemBase, int> _onCreateAll;

    private Vector2 _lastDelta;
    private int _minIndex;
    private int _maxIndex;
    private int _minContent;
    private int _maxContent;

    public void InitItem<T>(int dataCount, Action<UiItemBase, int> onCreate = null, Action<UiItemBase, int> onPosChanged = null, Action<UiItemBase, int> onRefresh = null, Action<UiItemBase, int> onSelect = null, Action<UiItemBase, int> onCreateAll = null) where T : UiItemBase
    {
        _dataCount = dataCount;
        _onCreate = onCreate;
        _onPosChanged = onPosChanged;
        _onRefresh = onRefresh;
        _onSelect = onSelect;
        _onCreateAll = onCreateAll;

        _Scroll.horizontal = axis == GridLayoutGroup.Axis.Horizontal;
        _Scroll.vertical = axis == GridLayoutGroup.Axis.Vertical;

        SetMaxItemCount();
        SetContent();
        CreateItem<T>();
        _lastDelta = content.anchoredPosition;
        _Scroll.onValueChanged.AddListener(OnScrollValueChanged);
    }

    private void SetMaxItemCount()
    {
        var hor = axis == GridLayoutGroup.Axis.Horizontal;
        if (hor)
        {
            column = Mathf.CeilToInt(_dataCount / row);
        }
        else
        {
            row = Mathf.CeilToInt(_dataCount / column);
        }

        int xCount = Mathf.CeilToInt(viewport.rect.size.x / _childFullSize.x);
        int yCount = Mathf.CeilToInt(viewport.rect.size.y / _childFullSize.y);
        _childCount = hor ? row * xCount : column * yCount;
        _childCount += hor ? row : column;
        _maxContent = hor ? column : row;
    }

    private void SetContent()
    {
        Vector2 size = viewport.rect.size;
        if (axis == GridLayoutGroup.Axis.Horizontal)
        {
            SetContentRect(Vector2.zero, Vector2.up);
            size.x = (childSize.x * column) + (spacing.x * (column - 1)) + padding.left;
        }
        else
        {
            SetContentRect(Vector2.up, Vector2.one);
            size.y = (childSize.y * row) + (spacing.y * (row - 1)) + padding.top;
        }
        content.anchoredPosition = Vector2.zero;
        content.sizeDelta = size;
    }

    private void SetContentRect(Vector2 anchorMin, Vector2 anchorMax)
    {
        content.pivot = Vector2.up;
        content.anchorMin = anchorMin;
        content.anchorMax = anchorMax;
    }

    private void CreateItem<T>() where T : UiItemBase
    {
        for (int i = 0; i < _childCount; i++)
        {
            var item = UiUtility.CreateItem<T>(content);
            item.index = i;
            SetItemRect(item);
            SetItemPos(item, i);

            items.Add(item);
        }
        _minIndex = 0;
        _maxIndex = items.Count - 1;
    }

    private void SetItemRect(UiItemBase item)
    {
        var rect = item.rectTransform;
        rect.anchorMin = rect.anchorMax = Vector2.up;
        rect.sizeDelta = childSize;
    }

    private void SetItemPos(UiItemBase item, int index)
    {
        var rect = item.rectTransform;
        rect.anchoredPosition = GetItemPos(index);
        item.pos = rect.anchoredPosition;
    }

    private Vector2 GetItemPos(int index)
    {
        Vector2 pos = childSize / 2;
        if (axis == GridLayoutGroup.Axis.Horizontal)
        {
            pos.x += (childSize.x + spacing.x) * (index / row) + padding.left;
            pos.y = -pos.y - ((childSize.y + spacing.y) * (index % row) + padding.top);
        }
        else
        {
            pos.x += (childSize.x + spacing.x) * (index % column) + padding.left;
            pos.y = -pos.y - ((childSize.y + spacing.y) * (index / column) + padding.top);
        }
        return pos;
    }

    private void OnScrollValueChanged(Vector2 delta)
    {
        if (items.Count == 0) return;
        delta = content.anchoredPosition - _lastDelta;
        if (axis == GridLayoutGroup.Axis.Horizontal)
        {
            if (delta.x == 0 || Mathf.Abs(delta.x) <= spacing.x) return;
            HorizontalMove(delta.x);
        }
        else
        {
            if (delta.y == 0 || Mathf.Abs(delta.y) <= spacing.y) return;
            VerticalMove(delta.y);
        }
        _lastDelta = content.anchoredPosition;
    }

    private void HorizontalMove(float x)
    {
        if (x < 0)
        {
            MoveToRight();
        }
        else
        {
            MoveToLeft();
        }
    }

    private void SetItemLastSibling(UiItemBase item)
    {
        item.rectTransform.anchoredPosition = GetItemPos(_maxIndex);
        item.transform.SetAsLastSibling();
        _onPosChanged?.Invoke(item, _maxIndex);
    }
    private void SetItemFirstSibling(UiItemBase item)
    {
        item.rectTransform.anchoredPosition = GetItemPos(_minIndex);
        item.transform.SetAsFirstSibling();
        _onPosChanged?.Invoke(item, _minIndex);
    }

    private void MoveToRight()
    {
        if (content.offsetMin.x >= 0) return;
        for (int i = 0; i < row; i++)
        {
            if (_maxIndex + 1 >= _dataCount) return;
            var item = GetItem<UiItemBase>(0);
            var itemPos = item.transform.position;
            var viewportPos = viewport.position;
            if (Main.Ui.RenderMode != RenderMode.ScreenSpaceOverlay)
            {
                itemPos = Main.Ui.GetScreenPostion(item.transform.position);
                viewportPos = Main.Ui.GetScreenPostion(viewport.transform.position);
            }
            if (itemPos.x - viewportPos.x > childSize.x + padding.left * 2)
            {
                ++_maxIndex;
                ++_minIndex;
                SetItemLastSibling(item);
            }
        }
    }

    private void MoveToLeft()
    {
        if (content.offsetMax.x <= viewport.rect.size.x) return;
        for (int i = 0; i < row; i++)
        {
            if (_minIndex - 1 < 0) return;
            var item = GetItem<UiItemBase>(0);
            var itemPos = item.transform.position;
            var viewportPos = viewport.position;
            if (Main.Ui.RenderMode != RenderMode.ScreenSpaceOverlay)
            {
                itemPos = Main.Ui.GetScreenPostion(item.transform.position);
                viewportPos = Main.Ui.GetScreenPostion(viewport.transform.position);
            }
            if (itemPos.x - viewportPos.x < childSize.x + padding.left * 2)
            {
                item = GetItem<UiItemBase>(_childCount - 1);
                --_maxIndex;
                --_minIndex;
                SetItemFirstSibling(item);
            }
        }
    }

    private void VerticalMove(float y)
    {

    }

    public T GetItem<T>(int index) where T : Component
    {
        if (items.Count == 0) return default;
        return content.GetChild(index % _childCount).GetComponent<T>();
    }

    public override void SetLayoutHorizontal()
    {

    }

    public override void SetLayoutVertical()
    {

    }
}
