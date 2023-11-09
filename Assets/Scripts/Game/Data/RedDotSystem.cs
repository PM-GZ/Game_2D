using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class RedDotSystem : BaseData
{
    public enum RedDotType
    {
        Dot,
        Number,
    }


    public struct RedDotData
    {
        public BasePanel Parent;
        public List<UiBaseBasic> Children;
    }


    private Dictionary<Type, RedDotData> mRedDotDict = new();

    public void RegisterRedDot<T>() where T : BasePanel
    {
        if (!Main.Ui.TryGetPanel<T>(out var panel)) return;

        RedDotData data = new RedDotData
        {
            Parent = panel,
            Children = new(),
        };
        var children = panel.gameObject.GetComponentsInChildren<UiBaseBasic>().ToList();
        if (children == null) return;

        foreach (var child in data.Children)
        {
            if(!child.UseRedDot) continue;
            data.Children.Add(child);
        }

        var type = typeof(T);
        if(mRedDotDict.ContainsKey(type))
        {
            mRedDotDict[type] = data;
        }
        else
        {
            mRedDotDict.Add(type, data);
        }
    }

    public void RemoveRedDot<T>()
    {
        if(!mRedDotDict.TryGetValue(typeof(T), out var data)) return;

        mRedDotDict.Remove(typeof(T));
    }

    public RedDotData GetRedDotData<T>()
    {
        if (mRedDotDict.TryGetValue(typeof(T), out var data)) 
            return data;

        return default;
    }
}
