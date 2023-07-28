using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;




[RequireComponent(typeof(ScrollRect))]
public class UiCycleScroll : BaseUiBasic
{
    [SerializeField] private ScrollRect _Scroll;
    [SerializeField] private GridLayoutGroup _gridLayout;
    public RectTransform content { get => _Scroll.content; }
    public RectTransform viewport { get => _Scroll.viewport; }

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

        GetChildCount();
        CreateItem<T>();
        _lastDelta = content.anchoredPosition;
        _Scroll.onValueChanged.AddListener(OnScrollValueChanged);
    }

    private void GetChildCount()
    {
        if(_gridLayout.constraint == GridLayoutGroup.Constraint.FixedRowCount)
        {
            float count = _Scroll.content.sizeDelta.x / (_gridLayout.cellSize.x + _gridLayout.spacing.x) - _gridLayout.padding.left;
            _dataCount = Mathf.CeilToInt(count) * _gridLayout.constraintCount;
            _dataCount += _gridLayout.constraintCount * 4;
        }
        else if(_gridLayout.constraint == GridLayoutGroup.Constraint.FixedColumnCount)
        {
            float count = _Scroll.content.sizeDelta.y / (_gridLayout.cellSize.y + _gridLayout.spacing.y) - _gridLayout.padding.top;
            _dataCount = Mathf.CeilToInt(count) * _gridLayout.constraintCount;
            _dataCount += _gridLayout.constraintCount * 4;
        }
        else
        {
            _dataCount = 30;
        }
    }

    private void CreateItem<T>() where T : UiItemBase
    {
        for (int i = 0; i < _childCount; i++)
        {
            var item = UiUtility.CreateItem<T>(content);
            item.index = i;

            items.Add(item);
        }
        _minIndex = 0;
        _maxIndex = items.Count - 1;
    }

    private void OnScrollValueChanged(Vector2 delta)
    {
        if (items.Count == 0) return;
        delta = content.anchoredPosition - _lastDelta;
        if (_gridLayout.startAxis == GridLayoutGroup.Axis.Horizontal)
        {
            if (delta.x == 0 || Mathf.Abs(delta.x) <= _gridLayout.spacing.x) return;
            HorizontalMove(delta.x);
        }
        else
        {
            if (delta.y == 0 || Mathf.Abs(delta.y) <= _gridLayout.spacing.y) return;
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

    private void VerticalMove(float y)
    {

    }

    private void MoveToRight()
    {
        if (content.offsetMin.x >= 0) return;
        for (int i = 0; i < _gridLayout.constraintCount; i++)
        {
            var item = items[0];
            var rect = item.transform as RectTransform;
            float x = rect.anchoredPosition.x - rect.sizeDelta.x / 2;
            if (x <= 0)
            {
                SetItemLastSibling(item);
            }
        }
    }

    private void MoveToLeft()
    {
        if (content.offsetMax.x <= viewport.rect.size.x) return;
        for (int i = 0; i < _gridLayout.constraintCount; i++)
        {
            var item = items[0];
        }
    }

    private void SetItemLastSibling(UiItemBase item)
    {
        item.transform.SetAsLastSibling();
        _onPosChanged?.Invoke(item, _maxIndex);
    }
    private void SetItemFirstSibling(UiItemBase item)
    {
        item.transform.SetAsFirstSibling();
        _onPosChanged?.Invoke(item, _minIndex);
    }

    public T GetItem<T>(int index) where T : Component
    {
        if (items.Count == 0) return default;
        return content.GetChild(index % _childCount).GetComponent<T>();
    }
}
