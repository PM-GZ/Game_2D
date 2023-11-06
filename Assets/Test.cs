using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public UiBaseScroll scroll;

    void Start()
    {
        TEXT.Init();

        scroll.SetCycleCellGroup<UiSuppliesListItem>(new UiCycleScroll.CycleScrollData
        {
            DataCount = 20,
            CellCount = 10
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
