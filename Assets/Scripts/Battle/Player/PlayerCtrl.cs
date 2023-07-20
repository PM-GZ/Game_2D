using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[RoleBind("Cat")]
public class PlayerCtrl : RoleCtrlBase
{

    public override void OnStart()
    {
        fsmCtrl = new FSMControl();
        fsmCtrl.AddState(FSMStateBase.FSMStateMode.Walk, new PlayerWalkState(fsmCtrl, gameObject));
        fsmCtrl.SwitchState(FSMStateBase.FSMStateMode.Walk);
    }

    public override void OnUpdate()
    {
        fsmCtrl.Update();
    }

    public override void OnFixedUpdate()
    {
        fsmCtrl.FixedUpdate();
    }
}
