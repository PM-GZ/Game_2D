using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UObject = UnityEngine.Object;

public sealed class GameInput
{
    private PlayerInput _playerInput;

    public Action onInputChanged;

    public void Init()
    {
        InitInput();
    }

    private void InitInput()
    {
        var inputObj = new GameObject("[Input System]");
        UObject.DontDestroyOnLoad(inputObj);
        _playerInput = inputObj.GetOrAddComponent<PlayerInput>();
        _playerInput.camera = Camera.main;
        _playerInput.uiInputModule = UObject.FindAnyObjectByType<InputSystemUIInputModule>();
    }
}
