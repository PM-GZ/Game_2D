using UnityEngine.UI;



public class UiLayoutBase : LayoutGroup, IBaseUiBasic
{
    public override void CalculateLayoutInputVertical() { }

    public override void SetLayoutHorizontal() { }

    public override void SetLayoutVertical() { }

    public void Show(bool show)
    {
        gameObject.SetActive(show);
    }
}
