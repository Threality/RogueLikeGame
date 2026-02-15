using UnityEngine;
using UnityEngine.Tilemaps;
using Pathfinding;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;

public class WorldGenerator : MonoBehaviour
{
    #region Setup

    [Header("References")]
    public Tilemap tilemap;
    public Tilemap ladderMap;
    public TilemapCollider2D tilemapCollider2D;
    public TilesetOrganiser tilesetOrganiser;
    public ItemGenerator itemGenerator;
    public GameObject loadingScreen;
    public Transform worldParent;
    public CameraFollow mainCamera;
    public TileBase grassTile;
    public TileBase dirtTile;
    public TileBase stoneTile;
    public TileBase sandTile;
    public TileBase lightRockTile;

    [Header("Generation Settings")]
    public bool generateWorld = true;
    public int worldWidth = 256;
    public int worldHeight = 128;
    public int subSurfaceDepth = 10;
    public int seed;
    public bool randomSeed;

    [Header("Surface Generation")]
    public int baseHeight = 60;
    public float initialSurfaceScale = 0.02f;
    public float surfaceScale = 0.05f;
    public float initialHeightMultiplier = 20f;
    public float heightMultiplier = 15f;

    [Header("Biomes")]
    public float tempScale = 0.02f;
    public Biome[] biomes;

    [Header("Worm Caves")]
    public bool generateWormCaves = true;
    public int numberOfCaves = 6;
    public int caveLength = 100;
    public int caveWidth = 3;
    public float caveTurnFreq = 0.2f;
    public float verticalIntensity = 2.5f;
    public bool varyCaveWidth = true;
    public bool cavesBounce = true;

    [Header("Isolated Caves")]
    public bool generateIsolatedCaves = true;
    public float isolatedCaveSize = 0.05f;
    [Range(0f, 1f)] public float isolatedCaveThreshold = 0.65f;
    public int minDistanceFromSurface = 15;

    [Header("Blackout")]
    public Tilemap blackout;
    public Tilemap backgroundBlackout;
    public bool generateBlackout = true;
    public bool generateBackgroundBlackout = true;
    public TileBase blackoutTile;

    [Header("Background")]
    public Tilemap background;
    public bool generateBackground = true;

    [Header("Enemies")]
    public Transform enemyContainer;
    public EnemyManager enemyManager;
    public bool generateEnemyMap = true;

    [Header("Structures")]
    public bool generateStructures = true;
    public GameObject chestPrefab;
    public Transform chestContainer;
    public GameObject hub;
    public Transform gameContainer;
    public int numChests = 200;

    [Header("Game")]
    public GameObject player;

    // Data
    [HideInInspector] public int[] surfaceHeightCache;
    [HideInInspector] public float[] temperatureCache;

    [System.Serializable]
    public class Biome
    {
        public string name;
        [Range(-1f, 1f)] public float minTemp;
        public TileBase surfaceTile;
        public TilesetOrganiser.BlockSet[] subSurfaceSets;
        public TilesetOrganiser.BlockSet[] deepCaveSets;
    }

    private void Start()
    {
        NewWorld(GameManager.instance.loadSave);
    }

    public void NewWorld(bool loadSave)
    {
        // Save the game
        if (!loadSave) SaveManager.instance.SaveGame();

        // Enter loading screen
        GameManager.instance.ChangeGameState(GameManager.GameState.LoadingMenu);

        // Generate world and increase difficulty
        if (randomSeed) seed = Random.Range(0, 1_000_000);
        GameManager.instance.IncrementGalaticDepth();
        if (generateWorld) GenerateWorld();

        // Enter playing state
        GameManager.instance.ChangeGameState(GameManager.GameState.Playing);

        // Move everything to the correct starting location and stats
        ladderMap.transform.position = new Vector3(-worldWidth / 2, -worldHeight / 2, 0);
        player.transform.position = surfaceHeightCache.Count() > 0 ? new Vector2(0, surfaceHeightCache[worldWidth / 2] - worldHeight / 2 + 1) : new Vector3(0, 0, 0);
        mainCamera.transform.position = surfaceHeightCache.Count() > 0 ? new Vector2(0, surfaceHeightCache[worldWidth / 2] - worldHeight / 2 + 1) : new Vector3(0, 0, 0);
        PlayerContainer.instance.health = PlayerContainer.instance.MAX_HEALTH;
        GameManager.instance.difficultyTimer = 0f;

        // Load stats
        if (loadSave) SaveManager.instance.LoadGame();

        // Update the UI
        MenuManager.instance.UpdateUI();

        // Calculate camera bounds
        mainCamera.CalculateBounds();

        Time.timeScale = 1f;
    }


    #endregion
    #region Surface
    

    [ContextMenu("Generate World")]
    public void GenerateWorld()
    {
        // Reset entities
        ClearAllChests();
        ClearAllEnemies();

        // Generate terrain
        GenerateInitialHeightMap();
        GenerateInitialTerrain();

        // Generate caves
        if (generateWormCaves) GenerateWormCaves();
        if (generateIsolatedCaves) GenerateIsolatedCaves();

        // Generate structures
        if (generateStructures) GenerateStructures();

        // Decorate map
        tilesetOrganiser.ProcessMap(tilemap);
        if (generateBackground)
        {
            GenerateBackground();
            tilesetOrganiser.ProcessMap(background);
        }
        if (generateBlackout) GenerateBlackout();
        if (generateBackgroundBlackout) GenerateBackgroundBlackout();

        // Scan enemies A Star Graph for pathing
        if (generateEnemyMap) SetupEnemyGrid();

        // Reset ladder for player interaction
        ladderMap.ClearAllTiles();

        // Generate borders
        GenerateWorldBorders();
    }

    private void GenerateInitialHeightMap()
    {
        // Set up height cache for other parts of the program
        surfaceHeightCache = new int[worldWidth];

        // Set the initial suface height to an amplified and zoomed perlin value
        for (int x = 0; x < worldWidth; x++)
        {
            float heightNoise = Mathf.PerlinNoise((x + seed + 1000) * initialSurfaceScale, seed + 1000);
            int ySurface = baseHeight + Mathf.RoundToInt(heightNoise * initialHeightMultiplier);

            surfaceHeightCache[x] = ySurface;
        }
    }


    private void GenerateInitialTerrain()
    {
        // Get the tilemap ready for editing
        tilemap.ClearAllTiles();
        temperatureCache = new float[worldWidth];
        tilemap.transform.position = new Vector3(-worldWidth / 2, -worldHeight / 2, 0);

        // Go through each value for x and generate terrain
        for (int x = 0; x < worldWidth; x++)
        {
            // Get a value for height using perlin noise
            float heightNoise = Mathf.PerlinNoise((x + seed) * surfaceScale, seed);
            int ySurface = surfaceHeightCache[x] + Mathf.RoundToInt(heightNoise * heightMultiplier);

            // Save the height value
            surfaceHeightCache[x] = ySurface;

            // Get a value for the temperature using perlin noise
            float tempNoise = Mathf.PerlinNoise((x + seed) * tempScale, seed + 100);
            float temperature = (tempNoise * 2) - 1;

            temperatureCache[x] = temperature;

            // Get the biome this x coordinate is
            Biome currentBiome = GetBiome(temperature);

            // Go through every y level and build tilemap
            for (int y = 0; y < worldHeight; y++)
            {
                if (y < ySurface)
                {
                    // Place top tile
                    if (y == ySurface - 1)
                    {
                        tilemap.SetTile(new Vector3Int(x, y, 0), currentBiome.surfaceTile);
                    }
                    // Place soil tiles
                    else if (y > ySurface - subSurfaceDepth)
                    {
                        tilemap.SetTile(new Vector3Int(x, y, 0), currentBiome.subSurfaceSets[0].tile1x1);
                    }
                    // Place stone
                    else
                    {
                        tilemap.SetTile(new Vector3Int(x, y, 0), stoneTile);
                    }
                }
            }
        }
    }


    #endregion
    #region Caves

    private void GenerateWormCaves()
    {
        // Add slight randomness to the amount of caves on the world
        numberOfCaves = (int)Mathf.Floor(worldWidth / Random.Range(100, 150));
        numberOfCaves = numberOfCaves < 1 ? 1 : numberOfCaves;

        // Ensure caves spawn distribute across the world
        int segmentSize = worldWidth / numberOfCaves;

        // Spawn a "worm" for eah cave
        for (int i = 0; i < numberOfCaves; i++)
        {
            // Calculate starting coords
            int startX = Random.Range(i * segmentSize, (i + 1) * segmentSize);
            startX = Mathf.Clamp(startX, caveWidth, worldWidth - caveWidth);

            int startY = surfaceHeightCache[startX];

            // Spawn the "worm"
            DigWorm(startX, startY);
        }
    }

    private void GenerateIsolatedCaves()
    {
        // Adjust the seed to keep it unique
        float caveSeed = seed + 500;

        // Iterate through each x coord
        for (int x = 2; x < worldWidth - 2; x++)
        {
            int surfaceHeight = surfaceHeightCache[x];

            // Iterate through y coords. Keeps away from surface
            for (int y = 2; y < surfaceHeight - minDistanceFromSurface; y++)
            {
                float xCoord = (x + caveSeed) * isolatedCaveSize;
                float yCoord = (y + caveSeed) * isolatedCaveSize;

                // Gets the noise value
                float noiseValue = Mathf.PerlinNoise(xCoord, yCoord);

                // Destroy or keep tile
                if (noiseValue > isolatedCaveThreshold)
                {
                    tilemap.SetTile(new Vector3Int(x, y, 0), null);
                }
            }
        }
    }

    private void DigWorm(int startX, int startY)
    {
        float horizontalDir = Random.value > 0.5f ? -1f : 1f;
        Vector2 curPos = new Vector2(startX, startY);

        // Add some random variance to the route
        float noiseOffset = Random.Range(0f, 1000f);

        // Dig into the ground for a given amount of steps
        for (int step = 0; step < caveLength; step++)
        {
            // Get the next direction to dig in
            float xNoise = Mathf.PerlinNoise(step * 0.1f, noiseOffset);
            float xStep = horizontalDir * (0.5f + xNoise);
            float yNoise = Mathf.PerlinNoise(step * caveTurnFreq, noiseOffset + 500);
            float yStep = (yNoise - 0.5f) * verticalIntensity;

            // Add downwards bias
            yStep -= 0.15f;

            // Move current position
            curPos += new Vector2(xStep, yStep);

            // If it is at the edge of the world bounce back on itself
            if ((curPos.x < 5 || curPos.x > worldWidth - 5) && cavesBounce)
            {
                horizontalDir *= -1f;
                curPos.x = Mathf.Clamp(curPos.x, 5, worldWidth - 5);
            }

            // Add a random chance to bounce back on itself
            if (Random.value < 0.02f) horizontalDir *= -1f;

            // Add variation to the width
            int curWidth = varyCaveWidth ? Random.Range(0, 1) + caveWidth : caveWidth;
            if (curPos.x > curWidth && curPos.y < worldHeight && curPos.y > curWidth)
            {
                // Dig out area around this point
                CarveCircle(Mathf.RoundToInt(curPos.x), Mathf.RoundToInt(curPos.y), curWidth);
            }
        }
    }

    private void CarveCircle(int centerX, int centerY, int radius)
    {
        // For all x values in range
        for (int x = centerX - radius; x <= centerX + radius; x++)
        {
            // For all y values in range
            for (int y = centerY - radius; y <= centerY + radius; y++)
            {
                if (Vector2.Distance(new Vector2(centerX, centerY), new Vector2(x, y)) <= radius)
                {
                    // Destroy the tile
                    tilemap.SetTile(new Vector3Int(x, y, 0), null);
                }
            }
        }
    }


    #endregion
    #region Enemies


    private void SetupEnemyGrid()
    {
        // Set grid position and size
        GridGraph graph = AstarPath.active.graphs.OfType<GridGraph>().First();
        graph.SetDimensions(worldWidth, worldHeight, 1f);

        // Ensure it is on the correct mode for scanning
        tilemapCollider2D.compositeOperation = Collider2D.CompositeOperation.None;

        // Force a tilemap rebuilt and wait for completion
        tilemapCollider2D.enabled = false;
        tilemapCollider2D.enabled = true;
        Physics2D.SyncTransforms();
        
        // Scan the graph for valid route
        graph.Scan();

        // Set to the more efficient collider type
        tilemapCollider2D.compositeOperation = Collider2D.CompositeOperation.Merge;

        // Force a rebuild
        tilemapCollider2D.enabled = false;
        tilemapCollider2D.enabled = true;
    }


    #endregion
    #region Decoration


    private void GenerateBlackout()
    {
        // Ensure tilemap is empty
        blackout.ClearAllTiles();
        blackout.transform.position = new Vector3(-worldWidth / 2, -worldHeight / 2, 0);

        // Loop through every block a terrain peice can be placed on
        for (int x = 0; x < worldWidth; x++)
        {
            int surfaceHeight = surfaceHeightCache[x];
            for (int y = 0; y < surfaceHeight; y++)
            {
                // If there is a tile - place a backing tile
                if (tilemap.HasTile(new Vector3Int(x, y, 0)))
                {
                    blackout.SetTile(new Vector3Int(x, y, 0), blackoutTile);
                }
            }
        }
    }

    private void GenerateBackgroundBlackout()
    {
        // Clear all tiles and set position
        backgroundBlackout.ClearAllTiles();
        backgroundBlackout.transform.position = new Vector3(-worldWidth / 2, -worldHeight / 2, 0);

        // Place a tile on every tile below the surface
        for (int x = 0; x < worldWidth; x++)
        {
            int surfaceHeight = surfaceHeightCache[x];
            for (int y = 0; y < surfaceHeight; y++)
            {
                backgroundBlackout.SetTile(new Vector3Int(x, y, 0), blackoutTile);
            }
        }
    }

    private void GenerateBackground()
    {
        // Clear all tiles and set position
        background.ClearAllTiles();
        background.transform.position = new Vector3(-worldWidth / 2, -worldHeight / 2, 0);

        // Loop through all tiles that are below the world surface
        for (int x = 0; x < worldWidth; x++)
        {
            int surfaceHeight = surfaceHeightCache[x];
            for (int y = 0; y < surfaceHeight; y++)
            {
                // Place a background tile behind every tile
                background.SetTile(new Vector3Int(x, y, 0), lightRockTile);
            }
        }
    }


    #endregion
    #region Borders


    public void GenerateWorldBorders()
    {
        // Create world border 
        GameObject borders = new GameObject("World Border");
        borders.transform.parent = worldParent;
        int thickness = 5;

        // Create each wall
        CreateBorderPiece(borders, "Left Border", new Vector2(-thickness / 2f -0.5f, worldHeight / 2f), new Vector2(thickness, worldHeight + 20));
        CreateBorderPiece(borders, "Right Border", new Vector2(worldWidth + (thickness / 2f) - 0.5f, worldHeight / 2f), new Vector2(thickness, worldHeight + 20));
        CreateBorderPiece(borders, "Upper Border", new Vector2(worldWidth / 2f, worldHeight + 10), new Vector2(worldWidth + 20, thickness));
        CreateBorderPiece(borders, "Lower Border", new Vector2(worldWidth / 2f, -thickness / 2f - 0.5f), new Vector2(worldWidth + 20, thickness));
        
        // Set position to the middle of the map
        borders.transform.position = new Vector2(-worldWidth / 2f, -worldHeight / 2f);
    }

    private void CreateBorderPiece(GameObject parent, string name, Vector2 pos, Vector2 size)
    {
        // Create a new wall
        GameObject obj = new GameObject(name);
        obj.transform.parent = parent.transform;
        obj.transform.position = pos;

        // Ensure the player can collide with it
        BoxCollider2D collider = obj.AddComponent<BoxCollider2D>();
        collider.size = size;

        // Set it to be an obstacle
        obj.layer = LayerMask.NameToLayer("Obstacle");
    }


    #endregion
    #region Structures


    private void GenerateStructures()
    {
        // Get the amount of x room each chest needs to be evenly spaced
        int segmentSize = Mathf.FloorToInt(worldWidth / numChests);

        // Attempt to place the maximum amount of allowed chests
        for (int i = 0; i < numChests; i++)
        {
            // Allow each chest to be randomly moved throughout its zone
            int offset = Random.Range(0, segmentSize);
            int xCoord = segmentSize * i + offset;

            // If it is too close to the hub - go to the next place
            if (xCoord < worldWidth / 2f  + 10 && xCoord > worldWidth / 2f - 10f) continue;

            // Shuffle the y coordinates so they are checked randomly
            List<int> yCoords = new List<int>();
            for (int j = 0; j < worldHeight; j++) yCoords.Add(j);
            Shuffle(yCoords);

            foreach (int yCoord in yCoords)
            {
                if (StructureFits(2, 1, new Vector2Int(xCoord, yCoord)))
                {
                    // Place chest
                    GameObject chest = Instantiate(chestPrefab, chestContainer);
                    chest.transform.position = new Vector3(xCoord - (worldWidth / 2) + 1, yCoord - (worldHeight / 2) + 1, 0);

                    // Generate loot
                    chest.GetComponent<ChestData>().Initialise(itemGenerator.GenerateRandomChestLoot(GameManager.instance.GetDifficulty(chest)),
                    Mathf.FloorToInt(Random.Range(2,7) * GameManager.instance.GetDifficulty(chest)),
                    Mathf.FloorToInt(Random.Range(1,5) * GameManager.instance.GetDifficulty(chest)));
                    break;
                }    
            }
        }

        // Clear space for the hub
        int yPos = surfaceHeightCache[Mathf.FloorToInt(worldWidth / 2)];
        for (int i = -7; i < 7; i++)
        {
            for (int j = yPos; j < worldHeight; j++)
            {
                tilemap.SetTile(new Vector3Int(i + worldWidth / 2, j, 0), null);
            }
        }

        // Move the hub to the cleared location
        hub.transform.position = new Vector3(3, yPos - worldHeight / 2 + 4, 0);
    }
    public bool StructureFits(int width, int height, Vector2Int coords)
    {
        // Check there are no tiles where it will be placed
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j <= height; j++)
            {
                if (tilemap.HasTile(new Vector3Int(coords.x + i, coords.y + j, 0))) return false;
            }
        }

        // Check there is floor where under all tiles
        for (int i = 0; i < width; i++)
        {
            if (!tilemap.HasTile(new Vector3Int(coords.x + i, coords.y - 1, 0))) return false;
        }
        return true;
    }


    #endregion
    #region Utilities


    public Biome GetBiome(float temperature)
    {
        // Search through biomes (temp decreasing)
        foreach (var biome in biomes)
        {
            if (temperature >= biome.minTemp) return biome;
        }
        return biomes[0];
    }

    public void Shuffle<T>(List<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = UnityEngine.Random.Range(0, n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }


    public void ClearAllChests()
    {
        // Remove all chest game objects
        for (int i = chestContainer.childCount - 1; i >= 0; i--)
        {
            GameObject child = chestContainer.GetChild(i).gameObject;
            DestroyImmediate(child);
        }
    }

    public void ClearAllEnemies()
    {
        // Remove all enemy game objects
        for (int i = enemyContainer.childCount - 1; i >= 0; i--)
        {
            GameObject child = enemyContainer.GetChild(i).gameObject;
            enemyManager.RemoveEnemy(child);
        }
    }

    public float GameLocationToTileTemperature(GameObject obj)
    { 
        int posX = Mathf.RoundToInt(obj.transform.position.x + worldWidth / 2);
        return temperatureCache[posX];
    }

    #endregion
}