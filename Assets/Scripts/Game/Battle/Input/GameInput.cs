using System;
using UnityEngine;
using UnityEngine.InputSystem;

public sealed class GameInput
{
    public enum InputKey : byte
    {
        /// <summary>
        /// Key WSAD/↑↓←→
        /// </summary>
        Game_Move,
        /// <summary>
        /// Mouse Right
        /// </summary>
        Game_Fight,
        /// <summary>
        /// Key E
        /// </summary>
        Game_Interaction,

        /// <summary>
        /// Key Esc
        /// </summary>
        Ui_Esc
    }

    #region Action Map
    public const string ACTION_MAP_GAMEPLAY = "Game";
    public const string ACTION_MAP_UI = "Ui";
    #endregion

    #region ActionKey
    public const string GAME_ACTION_KEY_MOVE = "Move";
    public const string GAME_ACTION_KEY_FIGHT = "Fight";
    public const string GAME_ACTION_KEY_INTERACTION = "Interaction";

    public const string UI_ACTION_KEY_ESC = "Esc";
    #endregion




    private InputActionAsset _InputAsset;
    private InputActionMap _GameMap;
    private InputActionMap _UiMap;
    public event Action<InputKey, InputAction.CallbackContext> OnSendInput;

    public GameInput()
    {
        Init();
    }



    #region 初始化
    private void Init()
    {
        LoadInputAsset();
        SwitchInput(true, false);

        InitInputSystemAction();
        InitInputActions();
    }

    private void LoadInputAsset()
    {
        _InputAsset = Resources.Load<InputActionAsset>(Constant.INPUT_SYSTEM_PATH);
        _GameMap = _InputAsset.FindActionMap(ACTION_MAP_GAMEPLAY);
        _UiMap = _InputAsset.FindActionMap(ACTION_MAP_UI);
    }

    private void InitInputSystemAction()
    {
    }

    private void InitInputActions()
    {
        //------------------- Game Action
        AddPerformedAndCanel(GAME_ACTION_KEY_MOVE, OnGameMovePerform, OnGameMoveCanel);
        AddPerformed(GAME_ACTION_KEY_FIGHT, OnGameFightPreformed);
        AddPerformed(GAME_ACTION_KEY_INTERACTION, OnGameInteractionPreformed);

        //------------------- Ui Action
        AddPerformed(UI_ACTION_KEY_ESC, OnUiEscPreformed);
    }
    #endregion

    #region 事件回调方法
    //------------------- Game Action
    private void OnGameMovePerform(InputAction.CallbackContext context)
    {
        OnSendInput(InputKey.Game_Move, context);
    }
    private void OnGameMoveCanel(InputAction.CallbackContext context)
    {
        OnSendInput(InputKey.Game_Move, context);
    }

    private void OnGameFightPreformed(InputAction.CallbackContext context)
    {
        OnSendInput(InputKey.Game_Fight, context);
    }

    private void OnGameInteractionPreformed(InputAction.CallbackContext context)
    {
        OnSendInput(InputKey.Game_Interaction, context);
    }

    //------------------- Ui Action
    private void OnUiEscPreformed(InputAction.CallbackContext context)
    {
        OnSendInput(InputKey.Ui_Esc, context);
    }
    #endregion

    #region 添加、移除 Input 事件
    private void AddPerformed(string actionName, Action<InputAction.CallbackContext> action)
    {
        if (!TryGetInputAction(actionName, out var inputAction)) return;
        inputAction.performed += action;
    }
    private void RemovePerformed(string actionName, Action<InputAction.CallbackContext> action)
    {
        if (!TryGetInputAction(actionName, out var inputAction)) return;
        inputAction.performed -= action;
    }

    private void AddPerformedAndCanel(string actionName, Action<InputAction.CallbackContext> perform, Action<InputAction.CallbackContext> cancel)
    {
        if (!TryGetInputAction(actionName, out var inputAction)) return;
        inputAction.performed += perform;
        inputAction.canceled += cancel;
    }
    private void RemovePerformedAndCanel(string actionName, Action<InputAction.CallbackContext> perform, Action<InputAction.CallbackContext> cancel)
    {
        if (!TryGetInputAction(actionName, out var inputAction)) return;
        inputAction.performed -= perform;
        inputAction.canceled -= cancel;
    }
    #endregion

    #region 改建

    #endregion

    public void SwitchInput(bool ui, bool gameplay)
    {
        if (ui)
        {
            _UiMap.Enable();
        }
        else
        {
            _UiMap.Disable();
        }

        if (gameplay)
        {
            _GameMap.Enable();
        }
        else
        {
            _GameMap.Disable();
        }
    }

    private bool TryGetInputAction(string actionName, out InputAction input)
    {
        input = _InputAsset.FindAction(actionName);
        return input != null;
    }

    private void RemoveEvent()
    {
        //------------------- Game Action
        RemovePerformedAndCanel(GAME_ACTION_KEY_MOVE, OnGameMovePerform, OnGameMoveCanel);
        RemovePerformed(GAME_ACTION_KEY_FIGHT, OnGameFightPreformed);

        //------------------- Ui Action
        RemovePerformed(UI_ACTION_KEY_ESC, OnUiEscPreformed);
    }

    public void Dispose()
    {
        RemoveEvent();
        _UiMap?.Dispose();
        _GameMap?.Dispose();
    }
}
