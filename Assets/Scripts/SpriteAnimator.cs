using System;
using UnityEngine;

public class SpriteAnimator : MonoBehaviour
{
    [SerializeField] SpriteRenderer m_SpriteRenderer;

    private Sheet m_CurrentSheet;

    private bool m_Playing;
    private bool m_Looping;
    private float m_TimeSinceLastSprite;
    private int m_CurrentFrame;

    void Update()
    {
        if (!m_Playing)
            return;

        if (m_TimeSinceLastSprite > (m_CurrentSheet.FrameRate / 1000f))
        {
            SetSprite(GetNextFrame());

            if (!m_Looping && GetNextFrame() == 0)
                m_Playing = false;
        }

        m_TimeSinceLastSprite += Time.deltaTime;
    }

    public void Play(Sheet sheet, bool looping, Predicate<Sheet> predicate = null)
    {
        if (sheet == null)
        {
            m_CurrentSheet = null;
            m_Playing = false;
        }

        if (predicate != null && !predicate.Invoke(m_CurrentSheet))
            return;

        m_CurrentSheet = sheet;
        m_Looping = looping;
        m_TimeSinceLastSprite = -1;

        SetSprite(0);

        m_Playing = sheet.Sprites.Count > 1;
    }

    private void SetSprite(int index)
    {
        m_CurrentFrame = index;
        m_SpriteRenderer.sprite = m_CurrentSheet.Sprites[index];
        m_TimeSinceLastSprite = 0f;
    }

    private int GetNextFrame()
    {
        var nextFrame = m_CurrentFrame + 1;
        if (nextFrame >= m_CurrentSheet.Sprites.Count)
            nextFrame = 0;

        return nextFrame;
    }
}
