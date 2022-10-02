using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WinMenu : MonoBehaviour
{
    private const string k_DeathsMessage = "You spent {0:D2}:{1:D2}s playing and died {2} times.";

    [SerializeField] TextMeshProUGUI m_DeathsText;

    private PlayerInputActions m_Inputs;

    void Start()
    {
        m_Inputs = new PlayerInputActions();
        m_Inputs.Menu.Enable();

        GameManager.instance.EndGame();

        var t = TimeSpan.FromSeconds(GameManager.instance.TimeInGame);

        m_DeathsText.text = string.Format(k_DeathsMessage, t.Minutes, t.Seconds, GameManager.instance.Deaths);
    }

    public void Replay()
    {
        GameManager.instance.StartGame();
    }

    public void Exit()
    {
        GameManager.instance.LoadLevel("MainMenu");
    }
}
