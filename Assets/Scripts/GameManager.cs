using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Create instance
    public static GameManager instance;

    // Data
    public float  deathTimer = 0;
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
        
        if (!PlayerContainer.instance.playerIsAlive)
        {
            deathTimer += Time.deltaTime;
        }

        if (deathTimer > 1)
        {
            SceneManager.LoadScene("MainMenu");
        }

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
        if (curState == GameState.DeathMenu) return;

        if (state != GameState.MainMenu) MenuManager.instance.DisableAll();

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
                break;
            case GameState.DeathMenu:
                curState = GameState.DeathMenu;
                MenuManager.instance.EnterDeath();
                break;
            case GameState.MainMenu:
                curState = GameState.MainMenu;
                break;
            case GameState.LaunchMenu:
                curState = GameState.LaunchMenu;
                MenuManager.instance.EnterLaunch();
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
}
