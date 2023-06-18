using System;


[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public class PanelBind : Attribute
{
    public string name;
    public PanelType panelType;

    public PanelBind(string name, PanelType panelType)
    {
        this.name = name;
        this.panelType = panelType;
    }
}
