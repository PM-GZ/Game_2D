using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseRoleCtrl : MonoBehaviour
{
    public FSMControl fsmCtrl { get; private set; } = new();
    public Dictionary<BaseFSMState.FSMStateMode, BaseFSMState> fsmStateDict { get; set; } = new();

    #region Unity 生命周期方法
    private void Start()
    {
        OnStart(fsmCtrl);
    }

    private void Update()
    {
        fsmCtrl.Update();
        OnUpdate();
    }

    private void FixedUpdate()
    {
        OnFixedUpdate();
    }
    #endregion


    protected virtual void OnStart(FSMControl fsmCtrl)
    {

    }

    protected virtual void OnUpdate()
    {

    }

    protected virtual void OnFixedUpdate()
    {

    }
}
