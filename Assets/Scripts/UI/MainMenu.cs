using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

internal struct Res
{
    public int width, height, frameRate;

    public Res(int width, int height, int frameRate)
    {
        this.width = width;
        this.height = height;
        this.frameRate = frameRate;
    }

    public void SetResolution()
    {
        Screen.SetResolution(width, height, Screen.fullScreenMode, frameRate);
    }
}

public class MainMenu : MonoBehaviour
{
    private static readonly Dictionary<int, Res> k_Resolutions = new Dictionary<int, Res> {
        {0, new Res(1920, 1080, 60)},
        {1, new Res(1280, 720, 60)},
        {2, new Res(960, 540, 60)}
    };

    [SerializeField] GameObject m_Menu;
    [SerializeField] GameObject m_Options;

    [SerializeField] TMP_Dropdown m_Resolution;
    [SerializeField] Toggle m_Fullscreen;
    [SerializeField] Slider m_Audio;

    void Start()
    {
        var inputs = new PlayerInputActions();
        inputs.Game.Disable();
        inputs.Menu.Enable();
    }

    public void StartGame()
    {
        GameManager.instance.StartGame();
    }

    public void Options()
    {
        int i = 0;
        foreach (var val in k_Resolutions.Values)
        {
            if (val.width == Screen.currentResolution.width)
            {
                m_Resolution.value = i;
                break;
            }
            i++;
        }
        m_Fullscreen.isOn = Screen.fullScreenMode != FullScreenMode.Windowed;
        m_Audio.value = AudioManager.instance.Volume;

        m_Menu.SetActive(false);
        m_Options.SetActive(true);
    }

    public void Menu()
    {
        m_Menu.SetActive(true);
        m_Options.SetActive(false);
    }

    public void OnResolutionChanged(int value)
    {
        if (k_Resolutions.TryGetValue(value, out var res))
            res.SetResolution();
    }

    public void OnFullscreenChanged(bool value)
    {
        Screen.fullScreenMode = value ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
    }

    public void OnAudioChanged(float value)
    {
        AudioManager.instance.SetVolume((int)value);
    }

    public void Exit()
    {
        GameManager.instance.Exit();
    }
}
