using System;


[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
public class UiBindAttribute : Attribute
{
    public string name;

    public UiBindAttribute(string name)
    {
        this.name = name;
    }
}
