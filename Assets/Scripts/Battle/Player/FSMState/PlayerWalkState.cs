using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerWalkState : BaseFSMState
{
    private float mWaklX, mWaklY;


    public PlayerWalkState(FSMControl fsmCtrl, GameObject role) : base(fsmCtrl, role)
    {
    }

    public override void OnEnter()
    {
    }

    public override void OnUpdate()
    {
        mWaklX = Input.GetAxis("Horizontal");
        mWaklY = Input.GetAxis("Vertical");
        
    }

    public override void OnFixedUpdate()
    {
        if (mWaklX != 0 || mWaklY != 0)
        {
            role.transform.Translate(new Vector3(mWaklX, mWaklY, 0) * Time.fixedDeltaTime * 3, Space.World);
        }
    }

    public override void OnExit()
    {
    }
}
