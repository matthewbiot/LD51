using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crossfade : MonoBehaviour
{
    private static Crossfade s_Instance;

    public static Crossfade instance => s_Instance;

    [SerializeField] CanvasGroup m_Group;

    private Coroutine m_FadeCoroutine;

    void Awake()
    {
        if (s_Instance != null && s_Instance != this)
            Destroy(this);
        else
            s_Instance = this;
    }

    public void FadeOut(float fadeTime, Action onComplete = null)
    {
        if (m_FadeCoroutine != null)
            StopCoroutine(m_FadeCoroutine);

        var realTime = m_Group.alpha > 0 ? (1 - m_Group.alpha) * fadeTime : fadeTime;
        m_FadeCoroutine = StartCoroutine(DoFade(realTime, true, 1, onComplete));
    }

    public void FadeIn(float fadeTime, Action onComplete = null)
    {
        if (m_FadeCoroutine != null)
            StopCoroutine(m_FadeCoroutine);

        var realTime = m_Group.alpha < 1 ? m_Group.alpha * fadeTime : fadeTime;
        m_FadeCoroutine = StartCoroutine(DoFade(realTime, false, 0, onComplete));
    }

    private bool m_Started;

    private float m_ElapsedTime;
    private float m_FadeTime;
    private float m_StartValue;
    private float m_EndValue;

    private IEnumerator DoFade(float fadeTime, bool interactable, float target, Action onComplete)
    {
        m_Group.blocksRaycasts = interactable;
        m_Group.interactable = interactable;

        var start = m_Group.alpha;

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
}
