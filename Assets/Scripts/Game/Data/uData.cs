using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class uData
{
    public RedDotSystem RedDot { get; private set; }


    public void Init()
    {
        RedDot = new();
    }
}
