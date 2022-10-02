using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Instruction : MonoBehaviour
{
    [SerializeField] CanvasGroup m_Group;
    [SerializeField] GameObject m_Keyboard;
    [SerializeField] GameObject m_Gamepad;

    [SerializeField] bool m_UseControlsScheme = true;

    private Coroutine m_FadeCoroutine;

    void Start()
    {
        InputManager.instance?.AddControlsChangedListener(ControlsChanged);
    }

    void Destroy()
    {
        InputManager.instance?.RemoveControlsChangedListener(ControlsChanged);
    }

    public void Show(float fadeTime, Action onComplete = null)
    {
        SetText(IsKeyboard(InputManager.instance?.ControlScheme));

        if (m_FadeCoroutine != null)
            StopCoroutine(m_FadeCoroutine);

        m_FadeCoroutine = StartCoroutine(Fade(fadeTime, true, onComplete));
    }

    public void Hide(float fadeTime, Action onComplete = null)
    {
        if (m_FadeCoroutine != null)
            StopCoroutine(m_FadeCoroutine);

        m_FadeCoroutine = StartCoroutine(Fade(fadeTime, false, onComplete));
    }

    private IEnumerator Fade(float fadeTime, bool show, Action onComplete)
    {
        var start = m_Group.alpha;
        var target = show ? 1f : 0f;

        var timeElapsed = 0f;
        while (timeElapsed < fadeTime)
        {
            m_Group.alpha = Mathf.Lerp(start, target, timeElapsed / fadeTime);
            timeElapsed += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        m_Group.alpha = target;
        onComplete?.Invoke();
    }

    private bool IsKeyboard(string controlScheme)
    {
        return string.IsNullOrEmpty(controlScheme) || controlScheme == "Keyboard";
    }

    private void SetText(bool keyboard)
    {
        if (!m_UseControlsScheme)
            return;
        m_Keyboard.SetActive(keyboard);
        if (m_Gamepad)
            m_Gamepad.SetActive(!keyboard);
    }

    private void ControlsChanged(PlayerInput i)
    {
        SetText(IsKeyboard(i.currentControlScheme));
    }
}
