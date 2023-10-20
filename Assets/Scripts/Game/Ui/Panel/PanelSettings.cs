using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;

[PanelBind("PanelSettings", PanelType.Dialog, true)]
public class PanelSettings : PanelBase
{
    private enum OptionType
    {
        Window,
        Frameless,
        FullScreen,
    }


    [UiBind("Dropdown")] private TMP_Dropdown dropdown;
    [UiBind("DPI")] private TMP_Dropdown dpiDropdown;



    [DllImport("user32.dll")]
    static extern IntPtr SetWindowLong(IntPtr hwnd, int _nIndex, int dwNewLong);
    [DllImport("user32.dll")]
    static extern bool SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
    [DllImport("user32.dll")]
    static extern IntPtr GetForegroundWindow();

    const int GWL_STYLE = -16;
    const int WS_POPUP = 0x800000;

    private List<string> options = new()
    {
        "窗口化","无边框","全屏"
    };

    public override void OnStart()
    {
        base.OnStart();

        var dpis = Screen.resolutions;
        List<string> dpiStr = new();
        for (int i = 0; i < dpis.Length; i++)
        {
            string dpi = $"{dpis[i].width}*{dpis[i].height}";
            dpiStr.Add(dpi);
        }

        dpiDropdown.ClearOptions();
        dpiDropdown.AddOptions(dpiStr);
        dpiDropdown.onValueChanged.AddListener(OnDPIOptionChanged);
        dropdown.ClearOptions();
        dropdown.AddOptions(options);
        dropdown.onValueChanged.AddListener(OnScreenModeOptionChanged);
    }

    private void OnDPIOptionChanged(int index)
    {
        var dpi = Screen.resolutions[index];
        Screen.SetResolution(dpi.width, dpi.height, Screen.fullScreen);
    }

    private void OnScreenModeOptionChanged(int index)
    {
        switch ((OptionType)index)
        {
            case OptionType.Window:
                Screen.fullScreen = false;
                Screen.fullScreenMode = FullScreenMode.Windowed;
                break;
            case OptionType.Frameless:
                Screen.fullScreen = false;
                Screen.fullScreenMode = FullScreenMode.MaximizedWindow;
                break;
            case OptionType.FullScreen:
                Screen.fullScreen = true;
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                break;
        }
    }
}
