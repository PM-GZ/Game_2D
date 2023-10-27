using UnityEditor;


[CanEditMultipleObjects, CustomEditor(typeof(UiCycleScroll))]
public class UiCycleScrollInspector : Editor
{
    private UiCycleScroll mTarget;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        mTarget = target as UiCycleScroll;

        mTarget.UpdateInspector();
    }
}
