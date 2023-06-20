using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWalkState : BaseFSMState
{
    public PlayerWalkState(FSMControl fsmCtrl, GameObject role) : base(fsmCtrl, role)
    {
    }

    public override void OnEnter()
    {
    }

    public override void OnUpdate()
    {
        var x = Input.GetAxis("Horizontal");
        var y = Input.GetAxis("Vertical");
        if (x != 0 || y != 0)
        {
            role.transform.Translate(new Vector3(x, y, 0), Space.World);
        }
    }

    public override void OnFixedUpdate()
    {

    }

    public override void OnExit()
    {
    }
}
