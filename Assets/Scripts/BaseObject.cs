using System.Collections;
using System.Collections.Generic;

public class BaseObject
{
    private static List<BaseObject> mBehaviourList = new();
    private List<IEnumerator> mCoroutineList = new();
    private Dictionary<IEnumerator, Stack<object>> mCoroutineDict = new();

    public BaseObject()
    {
        mBehaviourList.Add(this);
        Init();
    }

    public virtual void Init() { }

    #region
    public static void Update()
    {
        for (int i = mBehaviourList.Count - 1; i >= 0; i--)
        {
            var obj = mBehaviourList[i];
            if (obj == null)
            {
                mBehaviourList.Remove(obj);
                continue;
            }
            obj.UpdateCoroutine();
            obj.OnUpdate();
        }
    }
    #endregion

    #region Ð­³Ì
    public void StartCoroutine(IEnumerator coroutine)
    {
        var stack = new Stack<object>();
        stack.Push(coroutine);
        mCoroutineDict.Add(coroutine, stack);
        mCoroutineList.Add(coroutine);
    }

    private void UpdateCoroutine()
    {
        for (int i = mCoroutineList.Count - 1; i >= 0 && mCoroutineList.Count > 0; --i)
        {
            var coroutine = mCoroutineList[i];
            var stack = mCoroutineDict[coroutine];
            if (!ExcuteCoroutine(stack))
            {
                stack.Pop();
            }
            if (stack.Count == 0 && i < mCoroutineList.Count)
            {
                mCoroutineList.Remove(coroutine);
                mCoroutineDict.Remove(coroutine);
            }
        }
    }

    private bool ExcuteCoroutine(Stack<object> routineStack)
    {
        object routine = routineStack.Peek();
        IEnumerator ienumator = routine as IEnumerator;

        if (ienumator == null) return false;

        if (ienumator.MoveNext() && ienumator.Current != null)
        {
            routineStack.Push(ienumator.Current);
        }

        return true;
    }

    public void StopCoroutine(IEnumerator coroutine)
    {
        if (mCoroutineDict.ContainsKey(coroutine))
        {
            mCoroutineDict[coroutine].Clear();
            mCoroutineList.Remove(coroutine);
            mCoroutineDict.Remove(coroutine);
        }
    }

    public void StopAllCoroutine()
    {
        mCoroutineList.Clear();
        mCoroutineDict.Clear();
    }
    #endregion

    public virtual void OnUpdate() { }
}
