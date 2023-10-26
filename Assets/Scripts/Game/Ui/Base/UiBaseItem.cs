using System;
using UnityEngine.EventSystems;




public class UiBaseItem : UiBaseBasic, IPointerClickHandler, IPointerEnterHandler
{
    public enum ItemType
    {
        None,
        Info,
    }

    public Action<UiBaseItem, int> onClick;
    public Action onPointClick;
    public Action onPointEnter;

    private int _index = -1;
    public bool isSelect { get; protected set; }
    public ItemType itemType { get; private set; } = ItemType.None;


    public void Init(int index, Action<UiBaseItem, int> onClick)
    {
        _index = index;
        this.onClick = onClick;
    }

    public virtual void OnPointerClick(PointerEventData eventData)
    {
        onClick?.Invoke(this, _index);
    }

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        onPointEnter?.Invoke();
    }

    public virtual void OnClick(Action onClick = null)
    {
        onPointClick?.Invoke();
    }

    public void SetItemType(ItemType type)
    {
        itemType = type;
    }
}
