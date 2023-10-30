using System;
using System.Collections.Generic;




public static class MsgCenter
{
    public enum EventType 
    {

    }
    public struct ActionData
    {
        public EventType type;
        public object data;
    }

    private static Dictionary<EventType, Action<ActionData>> mActionDict = new();

    public static void AddAction(EventType type, Action<ActionData> action)
    {
        if (!mActionDict.TryAdd(type, action))
        {
            mActionDict[type] += action;
        }
    }

    public static void RemoveAction(EventType type, Action<ActionData> action)
    {
        if (action != null && mActionDict.ContainsKey(type))
        {
            mActionDict[type] -= action;
            if (mActionDict[type] == null)
            {
                mActionDict.Remove(type);
            }
        }
    }

    public static void Broadcast(EventType type, ActionData data)
    {
        if (mActionDict.ContainsKey(type))
        {
            mActionDict[type]?.Invoke(data);
        }
    }

    public static void ClearAction()
    {
        foreach (var action in mActionDict)
        {
            mActionDict[action.Key] = null;
        }
        mActionDict.Clear();
    }
}
