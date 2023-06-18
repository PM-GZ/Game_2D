using System.Collections;
using System.Collections.Generic;

public class BaseObject
{
    private static List<BaseObject> mBehaviourList = new();
    private static List<IEnumerator> mCoroutineList = new();
    private static Dictionary<IEnumerator, Stack<object>> mCoroutineDict = new();

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
    public IEnumerator StartCoroutine(IEnumerator coroutine)
    {
        var stack = new Stack<object>();
        stack.Push(coroutine);
        mCoroutineDict.Add(coroutine, stack);
        mCoroutineList.Add(coroutine);
        yield return coroutine;
    }

    private void UpdateCoroutine()
    {
        for (int i = mCoroutineList.Count - 1; i >= 0; --i)
        {
            var coroutine = mCoroutineList[i];
            var stack = mCoroutineDict[coroutine];
            if (!ExcuteCoroutine(coroutine))
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

    private bool ExcuteCoroutine(IEnumerator coroutine)
    {
        var stack = mCoroutineDict[coroutine];
        var obj = stack.Peek() as IEnumerator;

        if (obj == null || !obj.MoveNext()) return false;
        if (obj.Current == null) return false;

        stack.Push(obj.Current);

        return true;
    }
    #endregion

    public virtual void OnUpdate() { }
}
