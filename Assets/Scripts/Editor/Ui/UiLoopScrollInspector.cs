using UnityEditor;
using UnityEngine;
using UnityEngine.UI;


[CanEditMultipleObjects, CustomEditor(typeof(UiLoopScroll))]
public class UiLoopScrollInspector : Editor
{
    private UiLoopScroll mScroll;
    private SerializedProperty content;
    private SerializedProperty Padding;
    private SerializedProperty axis;
    private SerializedProperty radius;
    private SerializedProperty autoResetPose;
    private SerializedProperty autoResetPoseSpeed;

    private void OnEnable()
    {
        content = serializedObject.FindProperty("Content");
        Padding = serializedObject.FindProperty("Padding");
        axis = serializedObject.FindProperty("Axis");
        radius = serializedObject.FindProperty("Radius");
        autoResetPose = serializedObject.FindProperty("AutoResetPose");
        autoResetPoseSpeed = serializedObject.FindProperty("AutoResetPoseSpeed");
    }

    public override void OnInspectorGUI()
    {
        mScroll = target as UiLoopScroll;
        serializedObject.Update();

        EditorGUILayout.PropertyField(content, new GUIContent("Content"));
        EditorGUILayout.PropertyField(axis, new GUIContent("Axis"));
        EditorGUILayout.PropertyField(Padding, new GUIContent("Padding"));
        radius.floatValue = EditorGUILayout.FloatField("Radius", radius.floatValue);
        autoResetPose.boolValue = EditorGUILayout.Toggle("AutoResetPose", autoResetPose.boolValue);
        autoResetPoseSpeed.floatValue = EditorGUILayout.FloatField("AutoResetPoseSpeed", autoResetPoseSpeed.floatValue);

        serializedObject.ApplyModifiedProperties();
    }
}
