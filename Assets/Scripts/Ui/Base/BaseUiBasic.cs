using UnityEngine;



public class BaseUiBasic : MonoBehaviour, IBaseUiBasic
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
