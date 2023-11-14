using TMPro;
using UnityEngine;



[PanelBind("DialogueBox")]
public class DialogueBox : BasePanel
{
    [UiBind("ContentTxt")] private TMP_Text mContentTxt;
    [UiBind("Options")] private Transform mOptions;


    public override void OnStart()
    {
        base.OnStart();
    }
}
