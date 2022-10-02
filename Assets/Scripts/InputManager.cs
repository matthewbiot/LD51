using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private static InputManager s_Instance;

    public static InputManager instance => s_Instance;

    [SerializeField] PlayerInput m_PlayerInput;

    public string ControlScheme => m_PlayerInput.currentControlScheme;


    public void Awake()
    {
        if (s_Instance != null && s_Instance != this)
            Destroy(this);
        else
        {
            s_Instance = this;
            DontDestroyOnLoad(this);
        }
    }

    public void AddControlsChangedListener(Action<PlayerInput> listener)
    {
        m_PlayerInput.onControlsChanged += listener;
    }

    public void RemoveControlsChangedListener(Action<PlayerInput> listener)
    {
        m_PlayerInput.onControlsChanged -= listener;
    }
}
