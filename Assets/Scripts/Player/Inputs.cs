using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public static class Inputs
{
    public static void Add(InputAction action, Action<InputAction.CallbackContext> onAction)
    {
        if (action == null)
            return;

        action.Enable();

        action.started += onAction;
        action.performed += onAction;
        action.canceled += onAction;
    }

    public static void Remove(InputAction action, Action<InputAction.CallbackContext> onAction)
    {
        if (action == null)
            return;

        action.started -= onAction;
        action.performed -= onAction;
        action.canceled -= onAction;

        action.Disable();
    }
}
