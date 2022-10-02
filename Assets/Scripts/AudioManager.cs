using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private static AudioManager s_Instance;

    public static AudioManager instance => s_Instance;


    [SerializeField] private GameObject m_SFX;
    [SerializeField] private GameObject m_Music;

    private int m_LastSFXSourceUsed = -1;
    private List<AudioSource> m_SFXSources;
    private AudioSource m_MusicSource;

    private float m_MaxVolume = 0.5f;
    private float m_Volume = 1f;

    public int Volume
    {
        get
        {
            return (int)(m_Volume * 10);
        }
        set
        {
            m_Volume = value / 10f;
        }
    }

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

    void Start()
    {
        m_SFXSources = new List<AudioSource>(m_SFX.GetComponents<AudioSource>());
        m_MusicSource = m_Music.GetComponent<AudioSource>();
    }

    public void SetVolume(int volume)
    {
        m_Volume = Mathf.Clamp(volume, 0, 10);
    }

    public void PlaySFX(AudioClip clip)
    {
        var index = m_LastSFXSourceUsed == -1 ? 0 : ((m_LastSFXSourceUsed + 1) % m_SFXSources.Count);

        var source = m_SFXSources[index];
        source.volume = m_Volume * m_MaxVolume;
        source.PlayOneShot(clip);

        m_LastSFXSourceUsed = index;
    }

}
