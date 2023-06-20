using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCtrl : BaseRoleCtrl
{
    protected override void OnStart()
    {
        fsmCtrl = new FSMControl();
        fsmCtrl.AddState(BaseFSMState.FSMStateMode.Walk, new PlayerWalkState(fsmCtrl, gameObject));
        fsmCtrl.SwitchState(BaseFSMState.FSMStateMode.Walk);
    }
}
