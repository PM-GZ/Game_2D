using System;
using UnityEngine;



[CreateAssetMenu(fileName = "UiConfig", menuName = "≈‰÷√/UiConfig")]
public class UiSOConfig : ScriptableObject
{
    public GameObject PanelPrefab;
    public System.Object PanelScript;
    public PanelLevel Level;
    public bool Forever;
    public RedDotSystem.RedDotType RedDotType;
}
