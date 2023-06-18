using System.Collections.Generic;
using FSMStateMode = BaseFSMState.FSMStateMode;



public class FSMControl
{
    public FSMStateMode StateMdoe { get; private set; }
    public BaseFSMState CurState { get; private set; }

    private Dictionary<FSMStateMode, BaseFSMState> mStateDict = new();

    public void AddState(FSMStateMode mode, BaseFSMState state)
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
}
