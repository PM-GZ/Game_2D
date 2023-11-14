using TMPro;



[PanelBind("DialogErrorTip")]
public class DialogErrorTip : BasePanel
{
    [UiBind("ErrorTip")] private TMP_Text _errorTip;


    public void SetTip(string tip)
    {
        _errorTip.text = tip;
    }
}
