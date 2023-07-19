using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCtrl : RoleCtrlBase
{
    protected override void OnStart()
    {
        fsmCtrl = new FSMControl();
        fsmCtrl.AddState(FSMStateBase.FSMStateMode.Walk, new PlayerWalkState(fsmCtrl, gameObject));
        fsmCtrl.SwitchState(FSMStateBase.FSMStateMode.Walk);
    }
}
