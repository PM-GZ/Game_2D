using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;




[RequireComponent(typeof(ScrollRect))]
public class UiCycleScroll : BaseUiBasic
{
    [Header("先使用布局组件调整布局，运行时使用本组件中计算的结果")]
    [SerializeField] private ScrollRect _Scroll;
    [SerializeField] private GridLayoutGroup _gridLayout;
    [SerializeField] private ContentSizeFitter _sizeFitter;

    public RectTransform content { get => _Scroll.content; }
    public RectTransform viewport { get => _Scroll.viewport; }
    private Vector2 _halfSize { get => _gridLayout.cellSize / 2; }

    private int _dataCount;
    private int _itemCount;
    public List<UiItemBase> items { get; private set; }
    private Action<UiItemBase, int> _onCreate;
    private Action<UiItemBase, int> _onPosChanged;
    private Action<UiItemBase, int> _onRefresh;
    private Action<UiItemBase, int> _onSelect;
    private Action _onCreateAll;

    private Vector2 _lastDelta;
    private int _dataIndex;
    private int _dataIndex2;


    public void InitItem<T>(int dataCount, Action<UiItemBase, int> onCreate = null, Action<UiItemBase, int> onPosChanged = null, Action<UiItemBase, int> onRefresh = null, Action<UiItemBase, int> onSelect = null, Action onCreateAll = null) where T : UiItemBase
    {
        _sizeFitter.enabled = false;
        _gridLayout.enabled = false;

        _dataCount = dataCount;
        _onCreate = onCreate;
        _onPosChanged = onPosChanged;
        _onRefresh = onRefresh;
        _onSelect = onSelect;
        _onCreateAll = onCreateAll;

        GetChildCount();
        CreateItem<T>();
        SetContent();
        _lastDelta = content.anchoredPosition;
        _Scroll.onValueChanged.AddListener(OnScrollValueChanged);
    }

    private void GetChildCount()
    {
        if (_gridLayout.constraint == GridLayoutGroup.Constraint.FixedRowCount)
        {
            float count = viewport.rect.width / (_gridLayout.cellSize.x + _gridLayout.spacing.x);
            _itemCount = Mathf.CeilToInt(count) * _gridLayout.constraintCount;
        }
        else if (_gridLayout.constraint == GridLayoutGroup.Constraint.FixedColumnCount)
        {
            float count = viewport.rect.height / (_gridLayout.cellSize.y + _gridLayout.spacing.y);
            _itemCount = Mathf.CeilToInt(count) * _gridLayout.constraintCount;
        }
        else
        {
            _itemCount = 30;
        }
        _itemCount += _gridLayout.constraintCount * 2;
        int diffValue = _itemCount % _gridLayout.constraintCount;
        if (diffValue != 0)
        {
            _itemCount += _gridLayout.constraintCount - diffValue;
        }
    }

    private void CreateItem<T>() where T : UiItemBase
    {
        items = new(_itemCount);
        for (int i = 0; i < _itemCount; i++)
        {
            var item = UiUtility.CreateItem<T>(content);
            item.Init(i, _onSelect);
            SetItem(item, i);

            _dataIndex2++;
            items.Add(item);
            _onCreate?.Invoke(item, i);
        }
        _onCreateAll?.Invoke();
    }

    private void SetItem(UiItemBase item, int index)
    {
        item.rectTransform.sizeDelta = _gridLayout.cellSize;
        item.rectTransform.anchorMin = Vector2.up;
        item.rectTransform.anchorMax = Vector2.up;

        var pos = GetItemPos(index);
        item.rectTransform.anchoredPosition = pos;
    }

    private void SetContent()
    {
        int count = _dataCount / _gridLayout.constraintCount;
        if (_gridLayout.constraint == GridLayoutGroup.Constraint.FixedRowCount)
        {
            content.anchorMin = Vector2.zero;
            content.anchorMax = Vector2.up;
            float x = count * (_gridLayout.cellSize.x + _gridLayout.spacing.x) + _gridLayout.padding.left -_gridLayout.spacing.x;
            content.sizeDelta = new Vector2(x, viewport.rect.height);
        }
        else if (_gridLayout.constraint == GridLayoutGroup.Constraint.FixedColumnCount)
        {
            content.anchorMin = Vector2.up;
            content.anchorMax = Vector2.one;
            float y = count * (_gridLayout.cellSize.y + _gridLayout.spacing.y) + _gridLayout.padding.top - _gridLayout.spacing.y;
            content.sizeDelta = new Vector2(viewport.rect.width, y);
        }
    }

    private void OnScrollValueChanged(Vector2 delta)
    {
        if (items.Count == 0) return;
        delta = content.anchoredPosition - _lastDelta;
        if (_gridLayout.constraint == GridLayoutGroup.Constraint.FixedRowCount)
        {
            if (delta.x == 0) return;
            HorizontalMove(delta.x);
        }
        else
        {
            if (delta.y == 0) return;
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
        if (y < 0)
        {
            MoveToTop();
        }
        else
        {
            MoveToBotton();
        }
    }

    private void MoveToRight()
    {
        if (content.anchoredPosition.x >= 0 || _dataIndex2 == _dataCount) return;

        var item = GetItem<UiItemBase>(0);
        float x = Mathf.Abs(content.anchoredPosition.x);
        if (x - _gridLayout.spacing.x >= item.rectTransform.anchoredPosition.x + _halfSize.x)
        {
            for (int i = 0; i < _gridLayout.constraintCount; i++)
            {
                item = GetItem<UiItemBase>(0);
                SetItemLastSibling(item);
                _dataIndex++;
                _dataIndex2++;

                if (_dataIndex2 == _dataCount) return;
            }
        }
    }

    private void MoveToLeft()
    {
        if (content.offsetMin.x >= 0 || _dataIndex == 0) return;

        var item = GetItem<UiItemBase>(items.Count - 1);
        float x = content.rect.width - viewport.rect.width + content.offsetMin.x;
        float itemX = content.rect.width - item.rectTransform.anchoredPosition.x;
        if (x - _halfSize.x >= itemX + _gridLayout.spacing.x)
        {
            for (int i = 0; i < _gridLayout.constraintCount; i++)
            {
                _dataIndex--;
                _dataIndex2--;
                item = GetItem<UiItemBase>(items.Count - 1);
                SetItemFirstSibling(item);

                if (_dataIndex == 0) return;
            }
        }
    }

    private void MoveToBotton()
    {
        if (content.anchoredPosition.y <= 0 || _dataIndex2 == _dataCount) return;

        var item = GetItem<UiItemBase>(0);
        float y = Mathf.Abs(item.rectTransform.anchoredPosition.y) + _halfSize.y;
        if (content.anchoredPosition.y - _gridLayout.spacing.x >= y)
        {
            for (int i = 0; i < _gridLayout.constraintCount; i++)
            {
                item = GetItem<UiItemBase>(0);
                SetItemLastSibling(item);
                _dataIndex++;
                _dataIndex2++;

                if (_dataIndex2 == _dataCount) return;
            }
        }
    }

    private void MoveToTop()
    {
        if (content.offsetMin.y >= 0 || _dataIndex == 0) return;

        var item = GetItem<UiItemBase>(items.Count - 1);
        float y = content.rect.height - viewport.rect.height - content.offsetMax.y;
        float itemY = content.rect.height + item.rectTransform.anchoredPosition.y;
        if (y - _halfSize.y >= itemY + _gridLayout.spacing.y)
        {
            for (int i = 0; i < _gridLayout.constraintCount; i++)
            {
                _dataIndex--;
                _dataIndex2--;
                item = GetItem<UiItemBase>(items.Count - 1);
                SetItemFirstSibling(item);

                if (_dataIndex == 0) return;
            }
        }
    }

    private Vector2 GetItemPos(int index)
    {
        Vector2 pos = Vector2.zero;
        if (_gridLayout.constraint == GridLayoutGroup.Constraint.FixedRowCount)
        {
            int x = index / _gridLayout.constraintCount;
            int y = index % _gridLayout.constraintCount;
            float posX = x * (_gridLayout.cellSize.x + _gridLayout.spacing.x) + _gridLayout.padding.left + _halfSize.x;
            float posY = y * (_gridLayout.cellSize.y + _gridLayout.spacing.y) + _gridLayout.padding.top + _halfSize.y;
            pos = new Vector2(posX, -posY);
        }
        else if (_gridLayout.constraint == GridLayoutGroup.Constraint.FixedColumnCount)
        {
            int x = index % _gridLayout.constraintCount;
            int y = index / _gridLayout.constraintCount;
            float posX = x * (_gridLayout.cellSize.x + _gridLayout.spacing.x) + _gridLayout.padding.left + _halfSize.x;
            float posY = y * (_gridLayout.cellSize.y + _gridLayout.spacing.y) + _gridLayout.padding.top + _halfSize.y;
            pos = new Vector2(posX, -posY);
        }
        return pos;
    }

    private void SetItemLastSibling(UiItemBase item)
    {
        item.rectTransform.anchoredPosition = GetItemPos(_dataIndex2);
        item.transform.SetAsLastSibling();
        _onPosChanged?.Invoke(item, _dataIndex2);
    }
    private void SetItemFirstSibling(UiItemBase item)
    {
        item.rectTransform.anchoredPosition = GetItemPos(_dataIndex);
        item.transform.SetAsFirstSibling();
        _onPosChanged?.Invoke(item, _dataIndex);
    }

    public void Refresh()
    {
        for (int i = 0; i < _itemCount; i++)
        {
            _onRefresh?.Invoke(items[i], i);
        }
    }

    public T GetItem<T>(int index) where T : Component
    {
        if (items.Count == 0) return default;
        return content.GetChild(index % items.Count).GetComponent<T>();
    }
}
