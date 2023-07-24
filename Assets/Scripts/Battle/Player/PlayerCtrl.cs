using UnityEngine;





public class PlayerCtrl : GameBehaviour
{
    public FSMControl fsmCtrl;
    private PlayerCollider _playerCollider;
    private Collider2D[] _collider2Ds = new Collider2D[10];

    #region override
    public override void OnStart()
    {
        InitRole();
        InitFSM();
    }

    public override void OnUpdate()
    {
        fsmCtrl.Update();
        DetectRoundObj();
    }

    public override void OnFixedUpdate()
    {
        fsmCtrl.FixedUpdate();
    }
    #endregion

    #region Init
    private void InitRole()
    {
        LoadRole();
        Main.Data.Player.SetRoleGO(gameObject);
        _playerCollider = gameObject.AddComponent<PlayerCollider>();
    }

    private void LoadRole()
    {
        var roleData = Main.Data.Player.player.roleData;
        gameObject = uAsset.LoadGameObject(roleData.RoleName);
        gameObject.name = $"Player[{roleData.RoleName}]";
        gameObject.tag = "Player";
    }

    private void InitFSM()
    {
        fsmCtrl = new FSMControl();
        fsmCtrl.AddState(FSMStateBase.FSMStateMode.Walk, new PlayerWalkState(fsmCtrl, gameObject));
        fsmCtrl.SwitchState(FSMStateBase.FSMStateMode.Walk);
    }
    #endregion

    private void DetectRoundObj()
    {
        Physics2D.OverlapCircleNonAlloc(transform.position, 6, _collider2Ds);
        Main.Data.Player.SetRoundObj(_collider2Ds);
    }
}
