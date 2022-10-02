using UnityEngine;

public class MainMenu : MonoBehaviour
{
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

    }

    public void Exit()
    {
        GameManager.instance.Exit();
    }
}
