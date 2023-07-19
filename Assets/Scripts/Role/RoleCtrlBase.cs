using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoleCtrlBase : MonoBehaviour
{
    public FSMControl fsmCtrl;

    #region Unity �������ڷ���
    private void Start()
    {
        OnStart();
    }

    private void Update()
    {
        fsmCtrl?.Update();
        OnUpdate();
    }

    private void FixedUpdate()
    {
        fsmCtrl?.FixedUpdate();
        OnFixedUpdate();
    }
    #endregion


    protected virtual void OnStart()
    {

    }

    protected virtual void OnUpdate()
    {

    }

    protected virtual void OnFixedUpdate()
    {

    }
}
