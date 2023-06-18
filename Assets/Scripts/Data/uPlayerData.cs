using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class uPlayerData : BaseData
{
    public GameObject Role { get; private set; }

    public override void Init()
    {

    }

    public void InitRole()
    {
        CreateRole();
    }

    private void CreateRole()
    {
        var role = Main.Asset.LoadAsset<GameObject>("Robot");
        Role = Object.Instantiate(role);
        Object.DontDestroyOnLoad(Role);
    }
}
