using System;
using UnityEngine;




[DisallowMultipleComponent]
public class Main : MonoBehaviour
{
    public static uAsset Asset { get; private set; }
    public static GameInput Input { get; private set; }
    public static uData Data { get; private set; }
    public static uScene Scene { get; private set; }
    public static uUi Ui { get; private set; }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);

        InitManager();
        InitUi();
    }

    private void InitManager()
    {
        Asset = new();
        Input = new();
        Input.Init();
        Data = new();
        Scene = new();
        Ui = new();
    }

    private void InitUi()
    {
        Ui.CreatePanel<PanelStart>();
    }

    private void Update()
    {
        BaseObject.Update();
    }

    private void FixedUpdate()
    {
    }

    private void LateUpdate()
    {
    }

    private void OnApplicationFocus(bool focus)
    {
        
    }

    private void OnApplicationPause(bool pause)
    {
        
    }

    private void OnApplicationQuit()
    {
    }
}