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
        Main.Input.OnSendInput += OnMoveInput;
    }

    private void OnMoveInput(GameInput.InputKey key, InputAction.CallbackContext context)
    {
        if (key != GameInput.InputKey.Game_Move) return;

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
        Main.Input.OnSendInput -= OnMoveInput;
    }
}
