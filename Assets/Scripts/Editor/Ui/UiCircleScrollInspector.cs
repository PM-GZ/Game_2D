using UnityEditor;
using UnityEngine;
using UnityEngine.UI;


[CanEditMultipleObjects, CustomEditor(typeof(UiCircleScroll))]
public class UiCircleScrollInspector : Editor
{
    private UiCircleScroll mScroll;
    private SerializedProperty content;
    private SerializedProperty cellSize;
    private SerializedProperty padding;
    private SerializedProperty axis;
    private SerializedProperty radius;
    private SerializedProperty autoResetPose;
    private SerializedProperty autoResetPoseSpeed;

    private void OnEnable()
    {
        content = serializedObject.FindProperty("Content");
        cellSize = serializedObject.FindProperty("CellSize");
        padding = serializedObject.FindProperty("Padding");
        axis = serializedObject.FindProperty("Axis");
        radius = serializedObject.FindProperty("Radius");
        autoResetPose = serializedObject.FindProperty("AutoResetPose");
        autoResetPoseSpeed = serializedObject.FindProperty("AutoResetPoseSpeed");
    }

    public override void OnInspectorGUI()
    {
        mScroll = target as UiCircleScroll;
        serializedObject.Update();

        EditorGUILayout.PropertyField(content, new GUIContent("Content"));
        EditorGUILayout.PropertyField(cellSize, new GUIContent("CellSize"));
        EditorGUILayout.PropertyField(padding, new GUIContent("Padding"));
        EditorGUILayout.PropertyField(axis, new GUIContent("Axis"));
        radius.floatValue = EditorGUILayout.FloatField("Radius", radius.floatValue);
        autoResetPose.boolValue = EditorGUILayout.Toggle("AutoResetPose", autoResetPose.boolValue);
        autoResetPoseSpeed.floatValue = EditorGUILayout.FloatField("AutoResetPoseSpeed", autoResetPoseSpeed.floatValue);

        serializedObject.ApplyModifiedProperties();
    }
}
