using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;



[InitializeOnLoad]
public static class UiHierarchyEditor
{
    private static Texture mUiMarkIcon;
    private static Texture mUiScrpitIcon;
    private static Texture mBindIcon; 
    private static Texture mNameRepeatIcon;
    private static Texture mMissIcon;
    private static Dictionary<string, Type> mUiPanels = new();
    private static Dictionary<string, Type> mBindDict = new();
    private static HashSet<string> mRepeatBinds = new();
    private static Type mType;

    static UiHierarchyEditor()
    {
        GetAllPanel();
        InitIcon();
        EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyWindowItemOnGUI;
        EditorApplication.hierarchyChanged += OnHierarchyChanged;
    }

    private static void GetAllPanel()
    {
        var base_type = typeof(BasePanel);
        var attribute_type = typeof(PanelBind);
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        foreach (var assembly in assemblies)
        {
            if (assembly.FullName.StartsWith("Unity")) continue;
            if (assembly.FullName.StartsWith("System")) continue;
            if (assembly.FullName.StartsWith("Mono")) continue;
            if (assembly.FullName.StartsWith("Bee")) continue;
            var types = assembly.GetTypes();
            foreach (var type in types)
            {
                if (type.IsSubclassOf(base_type))
                {
                    var attributes = type.GetCustomAttributes(attribute_type, false);
                    if (attributes.Length > 0)
                    {
                        var page_name = ((PanelBind)attributes[0]).name;
                        mUiPanels[page_name] = type;
                    }
                    else
                    {
                        mUiPanels[type.Name] = type;
                    }
                }
            }
        }
    }

    private static void InitIcon()
    {
        string floder = "Assets/Scripts/Editor/Ui/Icons/";
        mUiMarkIcon = AssetDatabase.LoadAssetAtPath<Texture2D>($"{floder}UiMark.png");
        mUiScrpitIcon = AssetDatabase.LoadAssetAtPath<Texture2D>($"{floder}UiScript.png");
        mBindIcon = AssetDatabase.LoadAssetAtPath<Texture2D>($"{floder}Binding.png");
        mNameRepeatIcon = EditorGUIUtility.FindTexture("CollabError");
        mMissIcon = EditorGUIUtility.FindTexture("console.warnicon.sml");
    }


    private static void OnHierarchyWindowItemOnGUI(int instanceID, Rect rect)
    {
        if (PrefabStageUtility.GetCurrentPrefabStage() == null) return;

        var obj = EditorUtility.InstanceIDToObject(instanceID);
        if (obj == null) return;

        var go = obj as GameObject;
        go.TryGetComponent<UiBaseBasic>(out var uibasic);
        go.TryGetComponent<Image>(out var img);
        if (uibasic != null)
        {
            var rect_postion = new Vector2(rect.position.x - 30f, rect.position.y);
            if (uibasic.GetType() == typeof(UiMark))
            {
                GUI.DrawTexture(new Rect(rect_postion, rect.height * Vector2.one), mUiMarkIcon, ScaleMode.ScaleToFit, true);
            }
            else
            {
                GUI.DrawTexture(new Rect(rect_postion, rect.height * Vector2.one), mUiScrpitIcon, ScaleMode.ScaleToFit, true);
            }

            if (mBindDict.ContainsKey(uibasic.name))
            {
                if (mRepeatBinds.Contains(uibasic.name))
                {
                    GUIContent content = new GUIContent($"{uibasic.name}重名！");
                    GUI.skin.label.CalcMinMaxWidth(content, out var min_width, out var max_width);
                    var position_x = rect.position.x + rect.width - min_width;
                    EditorGUI.DrawRect(rect, new Color(1, 0, 0, 0.3f));
                    EditorGUI.LabelField(new Rect(position_x, rect.position.y, min_width, rect.height), content);
                    GUI.DrawTexture(new Rect(new Vector2(position_x + min_width, rect.position.y), rect.height * Vector2.one), mNameRepeatIcon, ScaleMode.ScaleToFit, true);
                }
                else
                {
                    if (uibasic.TryGetComponent(mBindDict[uibasic.name], out var component))
                    {
                        GUI.DrawTexture(new Rect(rect.position, rect.height * Vector2.one), mBindIcon, ScaleMode.ScaleToFit, true);
                    }
                    else
                    {
                        GUIContent content = new GUIContent($"没挂载{mBindDict[uibasic.name].Name}组件");
                        GUI.skin.label.CalcMinMaxWidth(content, out var min_width, out var max_width);
                        var position_x = rect.position.x + rect.width - min_width;
                        EditorGUI.DrawRect(rect, new Color(1, 0, 0, 0.3f));
                        EditorGUI.LabelField(new Rect(position_x, rect.position.y, min_width, rect.height), content);
                        GUI.DrawTexture(new Rect(new Vector2(position_x + min_width, rect.position.y), rect.height * Vector2.one), mMissIcon, ScaleMode.ScaleToFit, true);
                    }
                }
            }
        }
    }

    private static void OnHierarchyChanged()
    {
        mBindDict.Clear();
        mRepeatBinds.Clear();

        var stage = PrefabStageUtility.GetCurrentPrefabStage();
        if (stage == null) return;

        var root = stage.prefabContentsRoot;
        if(TryGetPageType(root.name, out mType))
        {
            var uiWidgetAttributeType = typeof(UiBindAttribute);
            var fields = mType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            foreach (var field in fields)
            {
                var attributes = field.GetCustomAttributes(uiWidgetAttributeType, false);
                if (attributes.Length > 0)
                {
                    var widget = attributes[0] as UiBindAttribute;
                    mBindDict[widget.name] = field.FieldType;
                }
            }
        }

        Dictionary<string, GameObject> bindingNames = new Dictionary<string, GameObject>();
        var controls = root.GetComponentsInChildren<UiBaseBasic>(true);
        foreach (var control in controls)
        {
            if (!bindingNames.ContainsKey(control.name))
            {
                bindingNames.Add(control.name, control.gameObject);
                continue;
            }

            if (!ReferenceEquals(control.gameObject, bindingNames[control.name]))
            {
                mRepeatBinds.Add(control.name);
            }
        }
    }

    public static bool TryGetPageType(string name, out Type type)
    {
        return mUiPanels.TryGetValue($"{name}", out type);
    }
}
