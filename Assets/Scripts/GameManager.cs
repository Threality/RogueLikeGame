using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    // Create instance
    public static GameManager instance;

    // Data
    public float difficultyTimer = 0;
    public float difficultyMult = 0.01f;
    public bool loadSave = true;

    // World generation
    public enum Biomes
    {
        Forest,
        Grassland,
        IceCave,
        MagmaCave
    }

    // Spawning Mechanics
    public enum Rarity
    {
        Common,
        Uncommon,
        Rare,
        Epic,
        Legendary,
        Mythical
    }
    
    public enum UseType
    {
        None,
        Sword,
        Gun,
    }

    public enum Material
    {
        BlackSteel,
        Plastic,
        Scraps,
        Polymers,
    }

    // Game Management
    public enum GameState
    {
        MainMenu,
        WorkbenchMenu,
        ChestMenu,
        InventoryMenu,
        LaunchMenu,
        LoadingMenu,
        DeathMenu,
        Playing,
    }

    private GameState curState = GameState.MainMenu;
    private int galacticDepth;

    private void Awake()
    {
        // Check it is the only instance
        if (instance == null) instance = this;
        else Destroy(gameObject);
        DontDestroyOnLoad(transform.parent);
    }

    private void Update()
    {
        if (curState == GameState.MainMenu) return;

        difficultyTimer += difficultyTimer * difficultyMult * Time.deltaTime + Time.deltaTime / 300f;
    }

    public void StartGame(bool loadGame)
    {
        loadSave = loadGame;
        SceneManager.LoadScene("Gameplay");
    }

    public GameState GetGameState()
    {
        return curState;
    }

    public void ChangeGameState(GameState state)
    {
        // Set all UI to off
        if (state != GameState.MainMenu) MenuManager.instance.DisableAll();

        // Load the correct UI and other conditions
        switch (state)
        {
            case GameState.Playing:
                curState = GameState.Playing;
                MenuManager.instance.EnterPlaying();
                break;
            case GameState.WorkbenchMenu:
                curState = GameState.WorkbenchMenu;
                MenuManager.instance.EnterWorkbench();
                break;
            case GameState.ChestMenu:
                curState = GameState.ChestMenu;
                MenuManager.instance.EnterChest();
                break;
            case GameState.InventoryMenu:
                curState = GameState.InventoryMenu;
                MenuManager.instance.EnterInventory();
                break;
            case GameState.LoadingMenu:
                curState = GameState.LoadingMenu;
                MenuManager.instance.EnterLoading();
                Time.timeScale = 0f;
                break;
            case GameState.DeathMenu:
                curState = GameState.DeathMenu;
                MenuManager.instance.EnterDeath();
                StopAllCoroutines();
                StartCoroutine(QueueDeathToMain());
                Time.timeScale = 0f;
                break;
            case GameState.MainMenu:
                curState = GameState.MainMenu;
                break;
            case GameState.LaunchMenu:
                curState = GameState.LaunchMenu;
                MenuManager.instance.EnterLaunch();
                Time.timeScale = 0f;
                break;
        }
    }

    public float GetDifficulty(GameObject obj)
    {
        // Call distance function with player and hub
        float dist = GetDistanceToObject(obj, HubManager.instance.hub);

        // Prevent undefined values and exit early with a default value
        float diff = 1f;
        if (dist > 10) diff = Mathf.Log10(dist);
                
        // Return log representing difficulty
        return diff + galacticDepth + difficultyTimer - 1;
    }

    public Biomes GetCurrentBiome()
    {
        // Link to terrain script to generate accurate result
        // For now return a default value
        return Biomes.Forest;
    }

    public int GetGalacticDepth()
    {
        return galacticDepth;
    }

    public void SetGalacticDepth(int val)
    {
        galacticDepth = val;
    }

    public void IncrementGalaticDepth()
    {
        galacticDepth++;
    }
    
    public float GetDistanceToObject(GameObject obj1, GameObject obj2)
    {
        // Calculate the change in x and y
        float x = obj1.transform.position.x - obj2.transform.position.x;
        float y = obj1.transform.position.y - obj2.transform.position.y;

        // Use pythagoras to calculate distance
        return Mathf.Sqrt(x * x + y * y);
    }

    public Vector2 GetMouseDirection()
    {
        Vector2 mouseScreenLocation = Mouse.current.position.ReadValue();
        Vector3 mouseWorldLocation = Camera.main.ScreenToWorldPoint(mouseScreenLocation);

        Vector2 direction = mouseWorldLocation - PlayerContainer.instance.player.transform.position;
        return direction.normalized;
    }

    private IEnumerator QueueDeathToMain()
    {
        yield return new WaitForSecondsRealtime(1f);
        SceneManager.LoadScene("MainMenu");
    }
}
