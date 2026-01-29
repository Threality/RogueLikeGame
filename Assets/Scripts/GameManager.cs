using System.Collections.Generic;
using System.Text.RegularExpressions;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Create instance
    public static GameManager instance;

    // Get important references
    public GameObject player;
    public GameObject hub;

    public float  deathTimer = 0;

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
    }

    public GameState GetGameState()
    {
        return curState;
    }

    public void ChangeGameState(GameState state)
    {
        if (curState == GameState.DeathMenu) return;

        MenuManager.instance.DisableAll();

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
        }
    }

    public float GetDifficulty(GameObject obj)
    {
        // Call distance function with player and hub
        float dist = GetDistanceToObject(obj, hub);

        // Prevent undefined values and exit early with a default value
        if (dist <= 10)
        {
            return 1f;
        }
                
        // Return log representing difficulty
        float diff = Mathf.Log10(dist);
        return diff + galacticDepth - 1;
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

        Vector2 direction = mouseWorldLocation - player.transform.position;
        return direction.normalized;
    }
}
