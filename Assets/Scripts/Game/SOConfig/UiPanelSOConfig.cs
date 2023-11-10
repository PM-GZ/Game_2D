using UnityEngine;



[CreateAssetMenu(fileName = "UiConfig", menuName = "≈‰÷√/UiConfig")]
public class UiPanelSOConfig : ScriptableObject
{
    public GameObject PanelPrefab;
    public PanelLevel Level;
    public bool Forever;
    public RedDotSystem.RedDotType RedDotType;
    public AudioClip Bgm;
}
