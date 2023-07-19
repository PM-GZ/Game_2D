using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerWalkState : FSMStateBase
{
    private float mWaklX, mWaklY;


    public PlayerWalkState(FSMControl fsmCtrl, GameObject role) : base(fsmCtrl, role)
    {
    }

    public override void OnEnter()
    {
        Main.Input.OnSendInput += OnSendMove;
    }

    private void OnSendMove(GameInput.InputKey name, InputAction.CallbackContext context)
    {
        var value = context.ReadValue<Vector2>();
        mWaklX = value.x;
        mWaklY = value.y;
    }

    public override void OnUpdate()
    {
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
        Main.Input.OnSendInput -= OnSendMove;
    }
}
