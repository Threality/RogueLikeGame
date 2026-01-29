using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Header("References")]
    public string gameSceneName;
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

    public void Play()
    {
        mainMenu.SetActive(false);
        loadingScreen.SetActive(true);  
        SceneManager.LoadScene(gameSceneName);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}