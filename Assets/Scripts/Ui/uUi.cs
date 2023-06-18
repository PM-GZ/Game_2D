using System;
using System.Collections.Generic;
using UnityEngine;
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

    private List<BasePanel> mPanelList = new();

    public override void Init()
    {
        mCanvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        Object.DontDestroyOnLoad(mCanvas);

        mUiCamera = mCanvas.transform.Find("UiCamera").GetComponent<Camera>();
        mBackground = mCanvas.transform.Find("Backgorund");
        mNormal = mCanvas.transform.Find("Normal");
        mFixed = mCanvas.transform.Find("Fixed");
        mDialog = mCanvas.transform.Find("Dialog");
        mTop = mCanvas.transform.Find("Top");
    }

    public Vector3 GetScreenPostion(Vector3 worldPostion)
    {
        return mUiCamera.WorldToScreenPoint(worldPostion);
    }

    public void CreatePanel<T>() where T : BasePanel
    {
        var panel = Activator.CreateInstance<T>();
        mPanelList.Add(panel);
        panel.InitPanel();
    }

    public GameObject LoadPanel(string name, PanelType panelType)
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
        var panel = Main.Asset.LoadAsset<GameObject>(name);
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

        SetSortingOrder(panel.transform, canvas);
    }

    private void SetSortingOrder(Transform parent, Canvas canvas)
    {
        int sortOrder = parent.GetComponent<Canvas>().sortingOrder;
        foreach (var child in parent.GetComponentsInChildren<Canvas>(true))
        {
            if(child == parent) continue;
            ++sortOrder;
        }
        canvas.overrideSorting = true;
        canvas.sortingOrder = sortOrder;
    }

    public void ClosePanel(BasePanel panel)
    {
        panel.OnClose();
        mPanelList.Remove(panel);
        EnableLastPanel();
    }

    private void EnableLastPanel()
    {

    }

    public T CreateItem<T>(Transform parent = null) where T : UiItemBase
    {
        var name = typeof(T).Name;
        var prefab = Main.Asset.LoadAsset<GameObject>(name);
        var item = Object.Instantiate(prefab, parent);
        return item.GetComponent<T>();
    }
}
