using System.Collections.Generic;
using System.Reflection;
using UnityEngine;



public class PanelBase : BaseObject
{
    public GameObject gameObject { get; private set; }
    public Transform transform { get; private set; }
    public CanvasGroup canvasGroup { get; private set; }
    public bool forever { get; private set; }

    public void InitPanel()
    {
        LoadPanel();
        InitField();
        OnStart();
        if(canvasGroup.alpha == 1)
        {
            OnEnable();
        }
    }

    private void LoadPanel()
    {
        var type = GetType();
        var objs = type.GetCustomAttributes(true);
        if (objs.Length > 0)
        {
            var attr = objs[0] as PanelBind;

            gameObject = Main.Ui.LoadPanelGO(attr.name, attr.panelType);
            transform = gameObject.transform;
            canvasGroup = gameObject.GetComponent<CanvasGroup>();
            forever = attr.forever;
        }
        else
        {
            Debug.LogError($"√ª”–…Ë÷√ [PanelBind]");
        }
    }

    private void InitField()
    {
        var uiBinds = new Dictionary<string, FieldInfo>();
        var type = GetType();
        var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        foreach (var field in fields)
        {
            var attribute = field.GetCustomAttribute(typeof(UiBindAttribute), false);
            var bind = attribute as UiBindAttribute;
            uiBinds.Add(bind.name, field);
        }

        foreach (var ui in gameObject.GetComponentsInChildren<IBaseUiBasic>(true))
        {
            if (uiBinds.TryGetValue(ui.gameObject.name, out var field))
            {
                var control_type = ui.GetType();

                if (field.FieldType == control_type || field.FieldType.IsSubclassOf(control_type))
                {
                    field.SetValue(this, ui);
                }
                else
                {
                    if (ui.gameObject.TryGetComponent(field.FieldType, out var component))
                    {
                        field.SetValue(this, component);
                    }
                    else
                    {
                        Debug.LogError($"PAGE:{type.Name} WIDGET:{ui.gameObject.name} TYPE {field.FieldType.Name} NOT MATCH TO {control_type.Name}");
                    }
                }
            }
        }
    }

    public void Show(bool show)
    {
        if (show)
        {
            OnEnable();
        }
        else
        {
            OnDisable();
        }
        canvasGroup.blocksRaycasts = show;
        canvasGroup.alpha = show ? 1 : 0;
    }

    public void Close()
    {
        StopAllCoroutine();
        Main.Ui.ClosePanel(this);
    }

    public virtual void OnStart() { }
    public virtual void OnEnable() { }
    public virtual void OnDisable() { }
    public virtual void OnClose() { }
}
