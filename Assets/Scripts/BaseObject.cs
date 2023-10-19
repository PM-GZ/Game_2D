using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseObject
{
    private class InvokeData
    {
        public Action action;
        public float invokeTime;
        public float intervalTime;
    }
    static private List<WeakReference> mObject = new List<WeakReference>();

    private List<IEnumerator> mCoroutineList = new();
    private Dictionary<IEnumerator, Stack<object>> mCoroutineDict = new();
    private List<InvokeData> mInvokeList = new();

    public BaseObject()
    {
        mObject.Add(new WeakReference(this));
    }

    #region update
    public static void UpdateAll()
    {
        for (int i = 0; i < mObject.Count; i++)
        {
            if (mObject[i].Target != null)
            {
                var obj = (BaseObject)mObject[i].Target;
                obj.UpdateIEnumerator();
            }
            else
            {
                mObject.RemoveAt(i);
            }
        }

        
    }

    private void UpdateIEnumerator()
    {
        if (mCoroutineList != null && mCoroutineList.Count > 0)
        {
            UpdateCoroutine();
        }

        UpdateInvoke();
    }

    private void UpdateInvoke()
    {
        for (int i = mInvokeList.Count - 1; i >= 0; i--)
        {
            var invoke = mInvokeList[i];
            if(invoke.invokeTime >= Time.realtimeSinceStartup)
            {
                invoke?.action();
                if(invoke.intervalTime == 0)
                {
                    mInvokeList.Remove(invoke);
                }
                else
                {
                    invoke.invokeTime = Time.realtimeSinceStartup + invoke.intervalTime;
                }
            }
        }
    }
    #endregion

    #region Invoke
    public void Invoke(Action action, float time)
    {
        InvokeRepeat(action, time, 0);
    }

    public void InvokeRepeat(Action action, float time, float interval)
    {
        var invoke = new InvokeData
        {
            action = action,
            invokeTime = time + Time.realtimeSinceStartup,
            intervalTime = interval,
        };
        mInvokeList.Add(invoke);
    }

    public void CancelInvoke(Action action)
    {
        for (int i = mInvokeList.Count - 1; i >= 0; i--)
        {
            if (mInvokeList[i].action == action)
            {
                mInvokeList.RemoveAt(i);
                return;
            }
        }
    }
    #endregion

    #region Ð­³Ì
    protected void StartCoroutine(IEnumerator coroutine)
    {
        mCoroutineList ??= new();
        mCoroutineDict ??= new();

        var stack = new Stack<object>();
        stack.Push(coroutine);
        mCoroutineDict.Add(coroutine, stack);
        mCoroutineList.Add(coroutine);
    }

    private void UpdateCoroutine()
    {
        for (int i = mCoroutineList.Count - 1; i >= 0; i--)
        {
            if (i >= mCoroutineList.Count) continue;
            var coroutine = mCoroutineList[i];
            var stack = mCoroutineDict[coroutine];
            if (!ExcuteCoroutine(stack))
            {
                stack.Pop();
            }
            if (stack.Count == 0 && i < mCoroutineList.Count)
            {
                mCoroutineList.RemoveAt(i);
                mCoroutineDict.Remove(coroutine);
            }
        }
    }

    private bool ExcuteCoroutine(Stack<object> routineStack)
    {
        object routine = routineStack.Peek();
        IEnumerator ienumator = routine as IEnumerator;

        if (ienumator == null) return false;

        if (ienumator.MoveNext())
        {
            if (ienumator.Current != null)
                routineStack.Push(ienumator.Current);
        }
        else
        {
            return false;
        }
        return true;
    }

    protected void StopCoroutine(IEnumerator coroutine)
    {
        if (mCoroutineList.Contains(coroutine))
        {
            mCoroutineList.Remove(coroutine);
        }
        if (mCoroutineDict.ContainsKey(coroutine))
        {
            mCoroutineDict[coroutine].Clear();
            mCoroutineDict.Remove(coroutine);
        }
    }

    protected void StopAllCoroutine()
    {
        mCoroutineList?.Clear();
        mCoroutineDict?.Clear();
        mInvokeList?.Clear();
    }
    #endregion
}
