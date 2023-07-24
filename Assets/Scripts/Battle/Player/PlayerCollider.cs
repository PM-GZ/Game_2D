using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCollider : MonoBehaviour
{
    private bool _autoInteraction;
    private Coroutine _coroutine;

    #region Unity Func

    private void Start()
    {
        Main.Input.OnSendInput += OnInteractionInput;
    }

    private void OnEnable()
    {
        Main.Input.OnSendInput += OnInteractionInput;
    }

    private void OnDisable()
    {
        Main.Input.OnSendInput -= OnInteractionInput;
        StopAllCoroutines();
        CancelInvoke();
    }

    private void OnDestroy()
    {
        Main.Input.OnSendInput -= OnInteractionInput;
        StopAllCoroutines();
        CancelInvoke();
    }

    #endregion


    private void OnInteractionInput(GameInput.InputKey key, InputAction.CallbackContext context)
    {
        if (key != GameInput.InputKey.Game_Interaction)
        {
            _autoInteraction = false;
            return;
        }
        if (!context.performed) return;
        if (Main.Data.Player.nearObj == null) return;

        _autoInteraction = true;
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
        }
        DoBehaviour();
    }

    private void DoBehaviour()
    {
        switch (Main.Data.Player.nearObjTag)
        {
            case uPlayerData.ObjTagType.Plant:
                _coroutine = StartCoroutine(MoveToObj(PickFruit));
                break;
            case uPlayerData.ObjTagType.Food:
                break;
        }
    }

    private IEnumerator MoveToObj(Action behaviour)
    {
        Transform role = transform.root;
        Collider2D target = Main.Data.Player.nearObj;
        float dis = Vector3.Distance(role.position, target.transform.position);
        while (dis > 0.5f)
        {
            if (!_autoInteraction) yield break;

            float moveSpeed = 3 * Time.fixedDeltaTime;
            role.transform.position = Vector3.MoveTowards(role.transform.position, target.transform.position, moveSpeed);
            dis = Vector3.Distance(role.position, target.transform.position);
            yield return new WaitForFixedUpdate();
        }
        behaviour?.Invoke();
        _coroutine = null;
    }

    #region Behaviour Func
    private void PickFruit()
    {
        Collider2D obj = Main.Data.Player.nearObj;
        PlantBase plant = obj.GetComponent<PlantBase>();
        plant.PickFruit();
    }
    #endregion
}
