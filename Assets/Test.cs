using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public UiBaseScroll scroll;

    void Start()
    {
        TableGoods goods = new TableGoods();

        foreach (var item in goods.mData)
        {
            Debug.Log($"{item.Key} \t {item.Value.ID}");
            Debug.Log($"{item.Key} \t {item.Value.goodName}");
            Debug.Log($"{item.Key} \t {item.Value.PrefabName}");
            Debug.Log($"{item.Key} \t {item.Value.IconPath}");
        }

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
