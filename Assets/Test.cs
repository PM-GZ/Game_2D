using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public UiCycleScroll scroll;

    void Start()
    {
        scroll.SetGroup<UiSuppliesListItem>(new UiCycleScroll.CycleListData
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
