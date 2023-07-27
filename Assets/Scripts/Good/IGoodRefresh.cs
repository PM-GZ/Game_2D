using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGoodRefresh
{
    bool fixedRefresh { get ;}
    uint randomGoodID { get; }

    TreasureChestBase.RandomGoodData randomGoodData { get; set; }
}
