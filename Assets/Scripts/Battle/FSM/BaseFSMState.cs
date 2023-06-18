using System.Collections;




public abstract class BaseFSMState
{
    public enum FSMStateMode : byte
    {
        Idle,
        Walk,
        Run
    }


    public abstract void OnEnter();
    public abstract void OnUpdate();
    public abstract void OnExit();
}
