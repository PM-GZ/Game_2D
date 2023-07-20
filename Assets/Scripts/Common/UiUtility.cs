using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UiUtility
{
    public static T CreateItem<T>(Transform parent = null) where T : UiItemBase
    {
        var name = typeof(T).Name;
        var prefab = Main.Asset.LoadAsset<GameObject>(name);
        var item = Object.Instantiate(prefab, parent, false);
        return item.GetComponent<T>();
    }

    public static List<T> CreateItems<T>(int count, Transform parent = null) where T : UiItemBase
    {
        var list = new List<T>();
        for (int i = 0; i < count; i++)
        {
            var item = CreateItem<T>(parent);
            list.Add(item);
        }
        return list;
    }
}
