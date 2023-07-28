using TMPro;
using UnityEngine;
using UnityEngine.UI;

[PanelBind("DialogSupplies", PanelType.Dialog, true)]
public class DialogSupplies : PanelBase
{
    public struct Param
    {
        public Vector3 pos;
    }



    [UiBind("Content")] private RectTransform _content;
    [UiBind("Title")] private TMP_Text _title;
    [UiBind("GoodsList")] private ScrollRect _goodsList;



    public override void OnStart()
    {
        base.OnStart();

        InitUiPos();
    }

    private void InitUiPos()
    {
        if (param == null) return;

        Param data = (Param)param;
        Vector2 pos = UiUtility.GetWorldSpacePos(_content, data.pos);
        _content.anchoredPosition = pos;
    }
}
