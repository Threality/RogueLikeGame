using UnityEngine;

public class BackgroundManager : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public WorldGenerator worldGenerator;

    [Header("Settings")]
    public float fadeRate = 2f;
    
    private BiomeBackground[] backgrounds;

    private void Start()
    {        
        backgrounds = GetComponentsInChildren<BiomeBackground>();
    }

    private void Update()
    {
        // If the player is not playing exit early
        if (GameManager.instance.GetGameState() != GameManager.GameState.Playing) return;
        
        HandleBiomeTransition();
    }

    private void HandleBiomeTransition()
    {
        // Get the biome the manager should be displaying
        string targetBiome = worldGenerator.GetBiome(worldGenerator.GameLocationToTileTemperature(player.gameObject)).name;

        // Set the opacity of all biomes to show the correct one
        foreach (BiomeBackground bg in backgrounds)
        {
            // Only show current biome background
            float targetOpacity = 0f;
            if (bg.biome == targetBiome) targetOpacity = 1f;

            float currentOpacity = bg.GetComponentInChildren<SpriteRenderer>().color.a;

            // Smoothly transition to the correct background
            float newOpacity = Mathf.MoveTowards(currentOpacity, targetOpacity, fadeRate * Time.deltaTime);
            bg.SetOpacity(newOpacity);
        }
    }
}