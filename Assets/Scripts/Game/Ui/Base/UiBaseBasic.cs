using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class UiBaseBasic : MonoBehaviour, IUiBaseBasic
{
    public bool UseRedDot;
    public RectTransform rectTransform { get => transform as RectTransform; }

    private List<IEnumerator> mCoroutines = new();

    public void Show(bool show)
    {
        gameObject.SetActive(show);
    }

    public new void StartCoroutine(IEnumerator e)
    {
        mCoroutines.Add(e);
        base.StartCoroutine(e);
    }

    public void ShowRedDot(bool show)
    {

    }

    private void OnDestroy()
    {
        foreach (var cor in mCoroutines)
        {
            StopCoroutine(cor);
        }
        mCoroutines.Clear();
    }
}
