using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crossfade : MonoBehaviour
{
    [SerializeField] CanvasGroup m_Group;

    private Coroutine m_FadeCoroutine;

    public void FadeOut(float fadeTime, Action onComplete = null)
    {
        if (m_FadeCoroutine != null)
            StopCoroutine(m_FadeCoroutine);

        var realTime = m_Group.alpha > 0 ? (1 - m_Group.alpha) * fadeTime : fadeTime;
        m_FadeCoroutine = StartCoroutine(DoFade(realTime, 1, onComplete));
    }

    public void FadeIn(float fadeTime, Action onComplete = null)
    {
        if (m_FadeCoroutine != null)
            StopCoroutine(m_FadeCoroutine);

        var realTime = m_Group.alpha < 1 ? m_Group.alpha * fadeTime : fadeTime;
        m_FadeCoroutine = StartCoroutine(DoFade(realTime, 0, onComplete));
    }

    private bool m_Started;

    private float m_ElapsedTime;
    private float m_FadeTime;
    private float m_StartValue;
    private float m_EndValue;

    private IEnumerator DoFade(float fadeTime, float target, Action onComplete)
    {
        m_Group.blocksRaycasts = true;
        m_Group.interactable = true;

        var start = m_Group.alpha;

        var timeElapsed = 0f;
        while (timeElapsed < fadeTime)
        {
            m_Group.alpha = Mathf.Lerp(start, target, timeElapsed / fadeTime);
            timeElapsed += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        onComplete?.Invoke();
    }
}
