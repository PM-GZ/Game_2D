using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class UiUtility
{
    public static T CreateItem<T>(Transform parent = null) where T : UiBaseItem
    {
        var name = typeof(T).Name;
        var prefab = uAsset.LoadAsset<GameObject>(name);
        var item = Object.Instantiate(prefab, parent, false);
        return item.GetComponent<T>();
    }

    public static List<T> CreateItems<T>(int count, Transform parent = null) where T : UiBaseItem
    {
        var list = new List<T>();
        for (int i = 0; i < count; i++)
        {
            var item = CreateItem<T>(parent);
            list.Add(item);
        }
        return list;
    }

    public static Vector2 GetWorldSpacePos(RectTransform ui, Vector3 target)
    {
        var root = ui.parent as RectTransform;
        Vector3 targetUiPos = Camera.main.WorldToScreenPoint(target);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(root, targetUiPos, Main.Ui.uiCamera, out var pos);
        return pos;
    }
}
