using TMPro;
using UnityEngine.UI;






[PanelBind("PanelLoading", PanelType.Top, true)]
public class PanelLoading : PanelBase
{
    [UiBind("Progress")] private Slider _PregressSlider;
    [UiBind("RatioText")] private TMP_Text _RatioText;
    [UiBind("StepText")] private TMP_Text _stepText;


    public override void OnStart()
    {
        _PregressSlider.value = 0;
        _RatioText.text = "0%";
    }

    public void SetProgrssValue(float progress)
    {
        _PregressSlider.value = progress;
        _RatioText.text = (progress * 100).ToString("N2") + "%";
    }

    public void SetProgrssStep(string step)
    {
        _stepText.text = step;
    }
}
