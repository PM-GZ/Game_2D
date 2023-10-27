using UnityEngine;



public class UiBaseBasic : MonoBehaviour, IUiBaseBasic
{
    public RectTransform rectTransform { get => transform as  RectTransform; }



    public void Show(bool show)
    {
        gameObject.SetActive(show);
    }
}
