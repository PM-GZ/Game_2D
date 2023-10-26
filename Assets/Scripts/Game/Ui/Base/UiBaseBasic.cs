using UnityEngine;



public class UiBaseBasic : MonoBehaviour, IBaseUiBasic
{
    public RectTransform rectTransform { get => transform as  RectTransform; }



    public void Show(bool show)
    {
        gameObject.SetActive(show);
    }

    void IBaseUiBasic.Show(bool show)
    {
        gameObject.SetActive(show);
    }
}
