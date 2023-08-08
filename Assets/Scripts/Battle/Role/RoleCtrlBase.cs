using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoleCtrlBase : GameBehaviour
{
    public FSMControl fsmCtrl;


    public RoleCtrlBase()
    {
        var t = GetType();
        var attrs = t.GetCustomAttributes(true);
        if (attrs == null || attrs.Length == 0) return;

        var attr = attrs[0] as RoleBindAttribute;
        gameObject = uAsset.LoadGameObject(attr.roleName); 
        OnStart();
        if (gameObject.activeSelf)
        {
            OnEnable();
        }
        behaviours.Add(this);
    }
}
