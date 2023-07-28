using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoodsBase : MonoBehaviour
{
    public bool nothingGoods { get; protected set; }

    #region Unity Func
    public virtual void Start()
    {
    }

    public virtual void OnEnable()
    {
    }

    public virtual void OnDisable()
    {
        StopAllCoroutines();
        CancelInvoke();
    }

    public virtual void OnDestroy()
    {
        StopAllCoroutines();
        CancelInvoke();
    }
    #endregion
}
