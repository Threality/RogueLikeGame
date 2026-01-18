using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class WorldGenerator : MonoBehaviour
{
    #region Setup

    [Header("References")]
    public Tilemap tilemap;
    public TileBase grassTile;
    public TileBase dirtTile;
    public TileBase stoneTile;
    public TileBase sandTile;
    public TileBase lightRockTile;
    public TilesetOrganiser tilesetOrganiser;

    [Header("Generation Settings")]
    public int worldWidth = 256;
    public int worldHeight = 128;
    public int subSurfaceDepth = 10;
    public int seed;
    public bool randomSeed;

    [Header("Surface Generation")]
    public int baseHeight = 60;
    public float surfaceScale = 0.05f;
    public float heightMultiplier = 15f;

    [Header("Biomes")]
    public float tempScale = 0.02f;
    public Biome[] biomes;

    [Header("Worm Caves")]
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

    // Data
    public int[] surfaceHeightCache;
    public float[] temperatureCache;

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
        if (randomSeed) seed = Random.Range(0, 1_000_000);
        GenerateWorld();
    }


    #endregion
    #region Surface


    public void GenerateWorld()
    {
        GenerateInitialTerrain();
        GenerateWormCaves();
        if (generateIsolatedCaves) GenerateIsolatedCaves();
        tilesetOrganiser.ProcessMap();
    }

    private void GenerateInitialTerrain()
    {
        // Get the tilemap ready for editing
        tilemap.ClearAllTiles();
        surfaceHeightCache = new int[worldWidth];
        temperatureCache = new float[worldWidth];
        tilemap.transform.position = new Vector3(-worldWidth / 2, -worldHeight / 2, 0);

        // Go through each value for x and generate terrain
        for (int x = 0; x < worldWidth; x++)
        {
            // Get a value for height using perlin noise
            float heightNoise = Mathf.PerlinNoise((x + seed) * surfaceScale, seed);
            int ySurface = baseHeight + Mathf.RoundToInt(heightNoise * heightMultiplier);

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
        numberOfCaves = (int) Mathf.Floor(worldWidth / Random.Range(100, 150));
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
        float caveSeed = seed + 500;

        for (int x = 2; x < worldWidth - 2; x++)
        {
            int surfaceHeight = surfaceHeightCache[x];
            for (int y = 2; y < surfaceHeight - minDistanceFromSurface; y++)
            {
                float xCoord = (x + caveSeed) * isolatedCaveSize;
                float yCoord = (y + caveSeed) * isolatedCaveSize;

                float noiseValue = Mathf.PerlinNoise(xCoord, yCoord);

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
            yStep -= 0.15f;

            curPos += new Vector2(xStep, yStep);
            
            if ((curPos.x < 5 || curPos.x > worldWidth - 5) && cavesBounce)
            {
                horizontalDir *= -1f;
                curPos.x = Mathf.Clamp(curPos.x, 5, worldWidth - 5);
            }

            if (Random.value < 0.02f) horizontalDir *= -1f;

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

    #endregion
}