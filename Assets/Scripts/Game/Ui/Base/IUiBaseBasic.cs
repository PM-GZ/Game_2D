using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUiBaseBasic
{
    public GameObject gameObject { get; }
    public void Show(bool show);
}
