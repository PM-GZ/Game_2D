using System;


[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public class PanelBind : Attribute
{
    public string name;
    public PanelType panelType;
    public bool forever;

    public PanelBind(string name, PanelType panelType, bool forever = false)
    {
        this.name = name;
        this.panelType = panelType;
        this.forever = forever;
    }
}
