using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
using Object = UnityEngine.Object;

public enum PanelLevel
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

    private Dictionary<string, UiPanelSOConfig> mUiPanelConfigDict = new();
    private List<BasePanel> mPanelList = new();
    private List<BasePanel> mForeverPanel = new();

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
        if (mPanelList.Count > 1)
        {
            Time.timeScale = 1;
            var panel = mPanelList[0];
            ClosePanel(panel);
        }
        else
        {
            Time.timeScale = 0;
            //CreatePanel<PanelESC>();
        }
    }
    #endregion

    public T CreatePanel<T>(object param = null) where T : BasePanel, new()
    {
        BasePanel panel = GetForeverPanel<T>();
        if(panel == null)
        {
            panel = new T();
            InitPanel($"{typeof(T).Name}SO", panel);
        }

        panel.Show(true);
        panel.param = param;
        panel.transform.SetAsLastSibling();
        mPanelList.Insert(0, panel);

        return panel as T;
    }

    private void InitPanel(string configName, BasePanel panel)
    {
        if (!mUiPanelConfigDict.TryGetValue(configName, out var config))
        {
            config = uAsset.LoadAsset<UiPanelSOConfig>(configName);
            mUiPanelConfigDict.Add(configName, config);
        }
        var go = LoadPanel(config);
        panel.SetPanelData(go, config.Forever);
        panel.InitPanel();
        if (config.Forever)
        {
            mForeverPanel.Add(panel);
        }
    }

    #region Get Func
    public Vector3 WorldToScreenPoint(Vector3 worldPostion)
    {
        return UiCamera.WorldToScreenPoint(worldPostion);
    }

    public T GetPanel<T>() where T : BasePanel
    {
        foreach (var panel in mPanelList)
        {
            if (panel is T) return panel as T;
        }
        return null;
    }

    public bool TryGetPanel<T>(out BasePanel panel) where T : BasePanel
    {
        panel = null;
        foreach (var item in mPanelList)
        {
            if (item is not T) continue;
            panel = item as T;
            return true;
        }
        return false;
    }

    public T GetForeverPanel<T>() where T : BasePanel
    {
        foreach (var panel in mForeverPanel)
        {
            if (panel is T) return panel as T;
        }
        return null;
    }
    #endregion

    #region UiHandle Func
    public GameObject LoadPanel(UiPanelSOConfig config)
    {
        Transform parent = mNormal;
        switch (config.Level)
        {
            case PanelLevel.Background:
                parent = mBackground;
                break;
            case PanelLevel.Normal:
                parent = mNormal;
                break;
            case PanelLevel.Fixed:
                parent = mFixed;
                break;
            case PanelLevel.Dialog:
                parent = mDialog;
                break;
            case PanelLevel.Top:
                parent = mTop;
                break;
            default:
                parent = mNormal;
                break;
        }
        var panel = Object.Instantiate(config.PanelPrefab, parent, false);
        panel.name = config.PanelPrefab.name;
        SetComponent(panel);
        return panel;
    }

    private void SetComponent(GameObject panel)
    {
        var canvas = panel.GetOrAddComponent<Canvas>();
        panel.GetOrAddComponent<CanvasGroup>();
        panel.GetOrAddComponent<GraphicRaycaster>();

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
    #endregion

    private void EnableLastPanel()
    {
        if (mPanelList.Count > 0)
        {
            var panel = mPanelList[0];
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
        if (panel == null || !mPanelList.Contains(panel)) return;

        panel.OnClose();
        mPanelList.Remove(panel);
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
        for (int i = mPanelList.Count - 1; i >= 0; i--)
        {
            var panel = mPanelList[0];
            ClosePanel(panel, false);
        }
        for (int i = mForeverPanel.Count - 1; i >= 0; i--)
        {
            Object.Destroy(mForeverPanel[i].gameObject);
        }
        mPanelList.Clear();
        mForeverPanel.Clear();
    }
    #endregion

    public void ChangedLanguage(string language)
    {
        TEXT.Init(language);
        var texts = Object.FindObjectsOfType<MultiLanguageText>();
        foreach (var item in texts)
        {
            item.ChangedLanguage();
        }
    }
}
