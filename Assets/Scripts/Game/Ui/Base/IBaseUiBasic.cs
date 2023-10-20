using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBaseUiBasic
{
    public GameObject gameObject { get; }
    public void Show(bool show);
}
