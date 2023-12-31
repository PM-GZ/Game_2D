using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class UiBaseBasic : MonoBehaviour, IUiBaseBasic
{
    public RectTransform rectTransform { get => transform as RectTransform; }

    private List<IEnumerator> mCoroutines = new();

    private void Start()
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

    public void Show(bool show)
    {
        gameObject.SetActive(show);
    }

    public new void StartCoroutine(IEnumerator e)
    {
        mCoroutines.Add(e);
        base.StartCoroutine(e);
    }
}
