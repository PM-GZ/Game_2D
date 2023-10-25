using System.Collections.Generic;
using UnityEngine;




public class GameBehaviour : BaseObject
{
    public static List<GameBehaviour> behaviours { get; protected set; } = new();


    public GameObject gameObject { get; protected set; }
    public Transform transform { get => gameObject.transform; }

    public GameBehaviour()
    {
        OnStart();
        behaviours.Add(this);
    }

    public static void FixedUpdate()
    {
        for (int i = behaviours.Count - 1; i >= 0 && behaviours.Count > 0; i--)
        {
            behaviours[i].OnFixedUpdate();
        }
    }
    public static void Update()
    {
        for (int i = behaviours.Count - 1; i >= 0 && behaviours.Count > 0; i--)
        {
            behaviours[i].OnUpdate();
        }
    }
    public static void LateUpdate()
    {
        for (int i = behaviours.Count - 1; i >= 0 && behaviours.Count > 0; i--)
        {
            behaviours[i].OnLateUpdate();
        }
    }

    public void Show(bool show)
    {
        gameObject.SetActive(show);
        if (show)
        {
            OnEnable();
        }
        else
        {
            OnDisable();
        }
    }

    public void Destroy()
    {
        StopAllCoroutine();
        behaviours.Remove(this);
        OnDestroy();
        Object.Destroy(gameObject);
    }



    public virtual void OnStart() { }
    public virtual void OnEnable() { }
    public virtual void OnReset() { }
    public virtual void OnFixedUpdate() { }
    public virtual void OnUpdate() { }
    public virtual void OnLateUpdate() { }
    public virtual void OnDisable() { }
    public virtual void OnRelease() { }
    public virtual void OnDestroy() { }
}
