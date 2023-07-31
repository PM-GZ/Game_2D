using System;
using UnityEngine.EventSystems;




public class UiItemBase : BaseUiBasic, IPointerClickHandler
{
    public Action<UiItemBase, int> onClick;
    private int _index = -1;


    public void Init(int index, Action<UiItemBase, int> onClick)
    {
        _index = index;
        this.onClick = onClick;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        onClick?.Invoke(this, _index);
    }
}
