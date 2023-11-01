using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
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
    public Camera UiCamera { get; private set; }

    public RenderMode RenderMode { get => mCanvas.renderMode; }

    private List<BasePanel> _panelList = new();
    private List<BasePanel> _foreverPanel = new();

    public uUi()
    {
        InitCanvas();
        mCanvas = Object.FindObjectOfType<Canvas>();

        UiCamera = mCanvas.transform.Find("UiCamera").GetComponent<Camera>();
        mBackground = mCanvas.transform.Find("Backgorund");
        mNormal = mCanvas.transform.Find("Normal");
        mFixed = mCanvas.transform.Find("Fixed");
        mDialog = mCanvas.transform.Find("Dialog");
        mTop = mCanvas.transform.Find("Top");

        InitInputEvent();
    }

    private void InitCanvas()
    {
        var canvas = Resources.Load<GameObject>("Canvas");
        canvas = Object.Instantiate(canvas, null, false);
        canvas.name = "Canvas";
        Object.DontDestroyOnLoad(canvas);
        var uiCam = canvas.GetComponentInChildren<Camera>();
        var camData = Camera.main.GetComponent<UniversalAdditionalCameraData>();
        camData.cameraStack.Add(uiCam);
    }

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
            var panel = _panelList[0];
            ClosePanel(panel);
        }
        else
        {
            Time.timeScale = 0;
            //CreatePanel<PanelESC>();
        }
    }
    #endregion

    public T CreatePanel<T>(object param = null) where T : BasePanel
    {
        BasePanel panel = GetForeverPanel<T>();
        if (panel == null)
        {
            panel = Activator.CreateInstance<T>();
            panel.param = param;
            panel.InitPanel();
            SetForeverPanel(panel);
        }
        panel.Show(true);
        panel.transform.SetAsLastSibling();
        _panelList.Insert(0, panel);

        return panel as T;
    }

    #region Get Func
    public Vector3 WorldToScreenPoint(Vector3 worldPostion)
    {
        return UiCamera.WorldToScreenPoint(worldPostion);
    }

    public T GetPanel<T>() where T : BasePanel
    {
        foreach (var panel in _panelList)
        {
            if (panel is T) return panel as T;
        }
        return null;
    }

    public T GetForeverPanel<T>() where T : BasePanel
    {
        foreach (var panel in _foreverPanel)
        {
            if (panel is T) return panel as T;
        }
        return null;
    }
    #endregion

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
        var panel = uAsset.LoadGameObject(name, parent);
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

    public void SetForeverPanel(BasePanel panel)
    {
        if (!panel.forever || _foreverPanel.Contains(panel)) return;
        
        _foreverPanel.Add(panel);
    }
    #endregion

    private void EnableLastPanel()
    {
        if (_panelList.Count > 0)
        {
            var panel = _panelList[0];
            panel.Show(true);
        }
        else
        {
            //CreatePanel<PanelPlayerUi>();
        }
    }

    #region Close Panel
    public void ClosePanel(BasePanel panel, bool showNext = true)
    {
        if (panel == null || !_panelList.Contains(panel)) return;

        panel.OnClose();
        _panelList.Remove(panel);
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
            var panel = _panelList[0];
            ClosePanel(panel, false);
        }
        for (int i = _foreverPanel.Count - 1; i >= 0; i--)
        {
            Object.Destroy(_foreverPanel[i].gameObject);
        }
        _panelList.Clear();
        _foreverPanel.Clear();
    }
    #endregion
}
