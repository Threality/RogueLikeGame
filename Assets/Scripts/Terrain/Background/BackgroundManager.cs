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
        if (GameManager.instance.GetGameState() != GameManager.GameState.Playing) return;
        
        HandleBiomeTransition();
    }

    private void HandleBiomeTransition()
    {
        string targetBiome = worldGenerator.GetBiome(worldGenerator.GameLocationToTileTemperature(player.gameObject)).name;

        foreach (BiomeBackground bg in backgrounds)
        {
            float targetOpacity = 0f;

            if (bg.biome == targetBiome) targetOpacity = 1f;

            float currentOpacity = bg.GetComponentInChildren<SpriteRenderer>().color.a;

            float newOpacity = Mathf.MoveTowards(currentOpacity, targetOpacity, fadeRate * Time.deltaTime);

            bg.SetOpacity(newOpacity);
        }
    }
}