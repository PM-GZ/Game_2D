using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Object = UnityEngine.Object;




public enum PanelType
{
    Background,
    Normal,
    Fixed,
    Dialog,
    Top
}

public class uUi : BaseObject
{
    private Canvas mCanvas;
    private Transform mBackground;
    private Transform mNormal;
    private Transform mFixed;
    private Transform mDialog;
    private Transform mTop;
    private Camera mUiCamera;

    public RenderMode RenderMode { get => mCanvas.renderMode; }

    private Stack<PanelBase> _panelList = new();
    private Dictionary<string, PanelBase> _foreverPanel = new();


    #region override
    public override void Init()
    {
        mCanvas = GameObject.Find("Canvas").GetComponent<Canvas>();

        mUiCamera = mCanvas.transform.Find("UiCamera").GetComponent<Camera>();
        mBackground = mCanvas.transform.Find("Backgorund");
        mNormal = mCanvas.transform.Find("Normal");
        mFixed = mCanvas.transform.Find("Fixed");
        mDialog = mCanvas.transform.Find("Dialog");
        mTop = mCanvas.transform.Find("Top");

        InitInputEvent();
    }
    #endregion

    private void InitInputEvent()
    {
        Main.Input.OnSendInput += OnECSInput;
    }

    #region Input Action Func
    private void OnECSInput(GameInput.InputKey key, InputAction.CallbackContext context)
    {
        if (key != GameInput.InputKey.Ui_Esc || context.performed == false) return;
        if (_panelList.Count > 1)
        {
            Time.timeScale = 1;
            var panel = _panelList.Peek();
            ClosePanel(panel);
        }
        else
        {
            Time.timeScale = 0;
            CreatePanel<PanelESC>();
        }
    }
    #endregion


    public Vector3 GetScreenPostion(Vector3 worldPostion)
    {
        return mUiCamera.WorldToScreenPoint(worldPostion);
    }

    public T CreatePanel<T>() where T : PanelBase
    {
        if (!_foreverPanel.TryGetValue(typeof(T).Name, out var panel))
        {
            panel = Activator.CreateInstance<T>();
            panel.InitPanel();
            SetForeverPanel(panel);
        }
        panel.Show(true);
        panel.transform.SetAsLastSibling();
        _panelList.Push(panel);

        return panel as T;
    }

    #region UiHandle Func
    public GameObject LoadPanelGO(string name, PanelType panelType)
    {
        Transform parent = mNormal;
        switch (panelType)
        {
            case PanelType.Background:
                parent = mBackground;
                break;
            case PanelType.Normal:
                parent = mNormal;
                break;
            case PanelType.Fixed:
                parent = mFixed;
                break;
            case PanelType.Dialog:
                parent = mDialog;
                break;
            case PanelType.Top:
                parent = mTop;
                break;
        }
        var panel = uAsset.LoadAsset<GameObject>(name);
        panel = Object.Instantiate(panel, parent);
        panel.name = name;
        SetComponent(panel);
        return panel;
    }

    private void SetComponent(GameObject panel)
    {
        var canvas = panel.AddComponent<Canvas>();
        panel.AddComponent<CanvasGroup>();
        panel.AddComponent<GraphicRaycaster>();

        canvas.vertexColorAlwaysGammaSpace = true;
        SetSortingOrder(panel.transform, canvas);
    }

    private void SetSortingOrder(Transform parent, Canvas canvas)
    {
        int sortOrder = parent.GetComponent<Canvas>().sortingOrder;
        foreach (var child in parent.GetComponentsInChildren<Canvas>(true))
        {
            if (child == parent) continue;
            ++sortOrder;
        }
        canvas.overrideSorting = true;
        canvas.sortingOrder = sortOrder;
    }

    public void SetForeverPanel(PanelBase panel)
    {
        if (!panel.forever) return;
        _foreverPanel.Add(panel.gameObject.name, panel);
    }
    #endregion

    private void EnableLastPanel()
    {
        if (_panelList.Count > 0)
        {
            var panel = _panelList.Peek();
            panel.Show(true);
        }
        else
        {

        }
    }

    #region Close Panel
    public void ClosePanel(PanelBase panel, bool showNext = true)
    {
        panel.OnClose();
        _panelList.Pop();
        if (panel.forever)
        {
            panel.Show(false);
        }
        else
        {
            Object.Destroy(panel.gameObject);
        }
        if (showNext)
        {
            EnableLastPanel();
        }
    }

    public void CloseAll()
    {
        for (int i = _panelList.Count - 1; i >= 0; i--)
        {
            var panel = _panelList.Peek();
            ClosePanel(panel, false);
        }
    }
    #endregion
}
