using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] GameObject m_Background;
    [SerializeField] GameObject m_Menu;
    [SerializeField] Button m_Resume;
    [SerializeField] Button m_Exit;
    [SerializeField] GameObject m_Dialog;
    [SerializeField] Button m_Yes;
    [SerializeField] Button m_No;

    private PlayerInputActions m_Inputs;

    // Start is called before the first frame update
    void Start()
    {
        m_Yes.onClick.AddListener(() =>
        {
            Time.timeScale = 1f;
            GameManager.instance.LoadLevel("MainMenu");
        });
        m_No.onClick.AddListener(() => ShowDialog(false));

        m_Inputs = new PlayerInputActions();
        m_Inputs.Game.Enable();

        Inputs.Add(m_Inputs.Game.Pause, OnPause);
    }

    public void Resume()
    {
        Inputs.Remove(m_Inputs.Menu.Cancel, OnCancel);

        Time.timeScale = 1f;
        m_Inputs.Game.Enable();
        m_Inputs.Menu.Disable();
        m_Background.SetActive(false);
    }

    public void Exit()
    {
        ShowDialog(true);
    }

    private void OnPause(InputAction.CallbackContext obj)
    {
        m_Resume.Select();
        m_Background.SetActive(true);
        Time.timeScale = 0f;
        m_Inputs.Game.Disable();
        m_Inputs.Menu.Enable();

        Inputs.Add(m_Inputs.Menu.Cancel, OnCancel);
    }

    private void OnCancel(InputAction.CallbackContext obj)
    {
        if (m_Dialog.activeSelf)
            ShowDialog(false);
        else
            Resume();
    }

    private void ShowDialog(bool visible)
    {
        m_Menu.SetActive(!visible);
        m_Dialog.SetActive(visible);

        if (visible)
            m_Yes.Select();
        else
            m_Exit.Select();
    }
}
