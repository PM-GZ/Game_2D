using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using Unity.VisualScripting;
using UObject = UnityEngine.Object;

public sealed class GameInput
{
    #region Action Map
    public const string ACTION_MAP_GAMEPLAY = "Gameplay";
    public const string ACTION_MAP_UI = "Ui";
    #endregion

    #region ActionKey
    public const string ACTION_KEY_MOVE = "Move";
    #endregion



    public PlayerInput playerInput { get; private set; }

    public Action onInputChanged;

    public void Init()
    {
        InitInput();
    }

    private void InitInput()
    {
        var inputObj = new GameObject("[Input System]");
        UObject.DontDestroyOnLoad(inputObj);
        playerInput = inputObj.GetOrAddComponent<PlayerInput>();
        playerInput.actions = Main.Asset.LoadAsset<InputActionAsset>("Input System");
        playerInput.camera = Camera.main;
        playerInput.uiInputModule = UObject.FindAnyObjectByType<InputSystemUIInputModule>();
    }
}
