using System;




[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public class RoleBindAttribute : Attribute
{
    public string roleName;
    public bool pool;
    public int maxCount;

    public RoleBindAttribute(string roleName, bool pool = false, int maxCount = 30)
    {
        this.roleName = roleName;
        this.pool = pool;
        this.maxCount = maxCount;
    }
}
