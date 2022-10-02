using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryManager : MonoBehaviour
{
    [SerializeField] Instruction m_Easy, m_Forgot, m_Elos;
    [SerializeField] AudioClip m_Sound;

    [SerializeField] float m_FadeTime = 0.5f;
    [SerializeField] float m_TimeBetweenStories = 2f;
    [SerializeField] float m_StoryTime = 5f;


    void Start()
    {
        Invoke("StartScene", 0.5f);
    }

    void StartScene()
    {
        m_Easy.Show(m_FadeTime, () => Invoke("HideEasy", m_StoryTime));
    }

    void HideEasy()
    {
        m_Easy.Hide(m_FadeTime, () => Invoke("Forgot", m_TimeBetweenStories));
    }

    void Forgot()
    {
        m_Forgot.Show(m_FadeTime, () => Invoke("HideForgot", m_StoryTime));
    }

    void HideForgot()
    {
        m_Forgot.Hide(m_FadeTime, () => Invoke("Elos", m_TimeBetweenStories));
    }

    void Elos()
    {
        AudioManager.instance.PlaySFX(m_Sound);
        m_Elos.Show(m_FadeTime, () => Invoke("Level2", m_StoryTime));
    }

    void Level2()
    {
        GameManager.instance.LoadLevel("Level02");
    }


}
