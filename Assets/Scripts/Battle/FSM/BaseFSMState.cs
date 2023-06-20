using System.Collections;
using UnityEngine;

public abstract class BaseFSMState
{
    public enum FSMStateMode : byte
    {
        Idle,
        Walk,
        Run
    }

    public FSMControl fsmCtrl { get; private set; }
    public GameObject role { get; private set; }

    public BaseFSMState(FSMControl fsmCtrl, GameObject role)
    {
        this.fsmCtrl = fsmCtrl;
        this.role = role;
    }

    public abstract void OnEnter();
    public abstract void OnUpdate();
    public abstract void OnFixedUpdate();
    public abstract void OnExit();
}
