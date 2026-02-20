using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    [Header("References")]
    public GameObject mainMenu;
    public GameObject loadingScreen;

    private void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;  
        mainMenu.SetActive(true);
        loadingScreen.SetActive(false);  
        GameManager.instance.ChangeGameState(GameManager.GameState.MainMenu);
    }

    public void Play(bool loadGame)
    {
        mainMenu.SetActive(false);
        loadingScreen.SetActive(true);  
        GameManager.instance.StartGame(loadGame);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}