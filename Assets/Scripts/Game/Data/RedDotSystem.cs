using System;
using System.Collections.Generic;
using RedDotType = RedDotSystem.RedDotType;




public class RedDotSystem : BaseData
{
    public enum RedDotType
    {
        Dot,
        Number,
    }

    public enum RedDotPos
    {
        LeftTop,
        RightTop,
        Center,
        LeftBottom,
        RightBottom,
    }
}
