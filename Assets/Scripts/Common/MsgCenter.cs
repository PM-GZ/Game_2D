using System;
using System.Collections.Generic;




public class MsgCenter
{
    public enum EventType { }
    public struct ActionData
    {
        public EventType type;
        public object data;
    }

    private Dictionary<EventType, Action<ActionData>> actionDict = new();

    public void AddAction(EventType type, Action<ActionData> action)
    {
        if (!actionDict.TryAdd(type, action))
        {
            actionDict[type] += action;
        }
    }

    public void RemoveAction(EventType type, Action<ActionData> action)
    {
        if (action != null && actionDict.ContainsKey(type))
        {
            actionDict[type] -= action;
            if (actionDict[type] == null)
            {
                actionDict.Remove(type);
            }
        }
    }

    public void Broadcast(EventType type, ActionData data)
    {
        if (actionDict.ContainsKey(type))
        {
            actionDict[type]?.Invoke(data);
        }
    }

    public void ClearAction()
    {
        foreach (var action in actionDict)
        {
            actionDict[action.Key] = null;
        }
        actionDict.Clear();
    }
}
