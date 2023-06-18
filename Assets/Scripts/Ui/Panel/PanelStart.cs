using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[PanelBind("PanelStart", PanelType.Normal)]
public class PanelStart : BasePanel
{
    [UiBind("Start")] private UiButton mStart;
    [UiBind("Continue")] private UiButton mContinue;
    [UiBind("Setting")] private UiButton mSetting;
    [UiBind("Quit")] private UiButton mQuit;



    public override void OnStart()
    {
        InitButtons();
    }

    private void InitButtons()
    {
        mStart.onClick = OnStartClick;
        mQuit.onClick = OnQuit;
    }

    private void OnStartClick()
    {
        Jump<PanelGameLevel>(false, false);
    }

    private void OnQuit()
    {
        Application.Quit();
    }
}
