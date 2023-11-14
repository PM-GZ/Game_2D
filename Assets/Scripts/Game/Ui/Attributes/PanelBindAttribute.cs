using System;


[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public class PanelBind : Attribute
{
    public string name;

    public PanelBind(string name)
    {
        this.name = name;
    }
}
