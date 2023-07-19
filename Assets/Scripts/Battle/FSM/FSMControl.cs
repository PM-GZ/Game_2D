using System.Collections.Generic;
using FSMStateMode = FSMStateBase.FSMStateMode;



public class FSMControl
{
    public FSMStateMode StateMdoe { get; private set; }
    public FSMStateBase CurState { get; private set; }

    private Dictionary<FSMStateMode, FSMStateBase> mStateDict = new();

    public void AddState(FSMStateMode mode, FSMStateBase state)
    {
        if(mStateDict.TryGetValue(mode, out var value))
        {
            mStateDict[mode] = state;
        }
        else
        {
            mStateDict.Add(mode, state);
        }
    }

    public void SwitchState(FSMStateMode mode)
    {
        StateMdoe = mode;
        CurState?.OnExit();
        CurState = mStateDict[mode];
        CurState?.OnEnter();
    }

    public void Update()
    {
        CurState?.OnUpdate();
    }
    public void FixedUpdate()
    {
        CurState?.OnFixedUpdate();
    }
}
