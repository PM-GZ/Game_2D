using System;
using System.Collections;
using System.Collections.Generic;

public class BaseObject
{
    static List<WeakReference> mObject = new List<WeakReference>();
    private List<IEnumerator> mCoroutineList = new();
    //private Dictionary<IEnumerator, Stack<object>> mCoroutineDict = new();

    public BaseObject()
    {
        mObject.Add(new WeakReference(this));
    }

    #region
    public static void UpdateAll()
    {
        for (int i = 0; i < mObject.Count; i++)
        {
            if (mObject[i].Target != null)
            {
                var obj = (BaseObject)mObject[i].Target;
                obj.Update();
            }
            else
            {
                mObject.RemoveAt(i);
            }
        }
    }

    private void Update()
    {
        if (mCoroutineList != null && mCoroutineList.Count > 0)
        {
            UpdateCoroutine();
        }
    }
    #endregion

    #region Ð­³Ì
    protected void StartCoroutine(IEnumerator coroutine)
    {
        mCoroutineList ??= new();
        //mCoroutineDict ??= new();

        //var stack = new Stack<object>();
        //stack.Push(coroutine);
        //mCoroutineDict.Add(coroutine, stack);
        mCoroutineList.Add(coroutine);
    }

    private void UpdateCoroutine()
    {
        for (int i = mCoroutineList.Count - 1; i >= 0; i--)
        {
            var coroutine = mCoroutineList[i];
            if (!coroutine.MoveNext())
            {
                mCoroutineList.RemoveAt(i);
            }
            //var stack = mCoroutineDict[coroutine];
            //if (!ExcuteCoroutine(stack))
            //{
            //    stack.Pop();
            //}
            //if (stack.Count == 0 && i < mCoroutineList.Count)
            //{
            //    mCoroutineList.RemoveAt(i);
            //    mCoroutineDict.Remove(coroutine);
            //}
        }
    }

    //private bool ExcuteCoroutine(Stack<object> routineStack)
    //{
    //    object routine = routineStack.Peek();
    //    IEnumerator ienumator = routine as IEnumerator;

    //    if (ienumator == null) return false;

    //    if (ienumator.MoveNext())
    //    {
    //        if (ienumator.Current != null)
    //            routineStack.Push(ienumator.Current);
    //    }
    //    else
    //    {
    //        return false;
    //    }
    //    return true;
    //}

    protected void StopCoroutine(IEnumerator coroutine)
    {
        if (mCoroutineList.Contains(coroutine))
        {
            mCoroutineList.Remove(coroutine);
        }
        //if (mCoroutineDict.ContainsKey(coroutine))
        //{
        //    mCoroutineDict[coroutine].Clear();
        //    mCoroutineDict.Remove(coroutine);
        //}
    }

    protected void StopAllCoroutine()
    {
        mCoroutineList?.Clear();
        //mCoroutineDict?.Clear();
    }
    #endregion
}
