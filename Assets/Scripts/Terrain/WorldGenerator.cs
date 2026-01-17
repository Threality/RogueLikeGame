using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class WorldGenerator : MonoBehaviour
{
    [Header("References")]
    public Tilemap tilemap;
    public TileBase grassTile;
    public TileBase dirtTile;
    public TileBase stoneTile;
    public TileBase sandTile;
    public TileBase lightRockTile;

    [Header("Generation Settings")]
    public int worldWidth = 256;
    public int worldHeight = 128;
    public int seed;
    public bool randomSeed;

    [Header("Surface Generation")]
    public int baseHeight = 60;
    public float surfaceScale = 0.05f;
    public float heightMultiplier = 15f;

    [Header("Biomes")]
    public float tempScale = 0.02f;
    public Biome[] biomes;

    [Header("Caves")]
    public int numberOfCaves = 6;
    public int caveLength = 100;
    public int caveWidth = 3;
    public float caveTurnFreq = 0.2f;

    private int[] surfaceHeightCache;

    [System.Serializable]
    public struct Biome
    {
        public string name;
        [Range(-1f, 1f)] public float minTemp;
        public TileBase surfaceTile;
        public TileBase fillTile;
    }

    private void Start()
    {
        if (randomSeed) seed = Random.Range(0, 1_000_000);
        GenerateWorld();
    }

    public void GenerateWorld()
    {
        tilemap.ClearAllTiles();
        surfaceHeightCache = new int[worldWidth];

        for (int x = 0; x < worldWidth; x++)
        {
            float heightNoise = Mathf.PerlinNoise((x + seed) * surfaceScale, seed);
            int ySurface = baseHeight + Mathf.RoundToInt(heightNoise * heightMultiplier);

            surfaceHeightCache[x] = ySurface;

            float tempNoise = Mathf.PerlinNoise((x + seed) * tempScale, seed + 100);
            float temperature = (tempNoise * 2) - 1;

            Biome currentBiome = GetBiome(temperature);

            for (int y = 0; y < worldHeight; y++)
            {
                if (y <= ySurface)
                {
                    if (y == ySurface)
                    {
                        tilemap.SetTile(new Vector3Int(x, y, 0), currentBiome.surfaceTile);
                    }
                    else if (y > ySurface - 10)
                    {
                        tilemap.SetTile(new Vector3Int(x, y, 0), currentBiome.fillTile);
                    }
                    else
                    {
                        tilemap.SetTile(new Vector3Int(x, y, 0), stoneTile);
                    }
                }
            }
        }

        GenerateCaves();
    }

    Biome GetBiome(float temperature)
    {
        foreach (var biome in biomes)
        {
            if (temperature >= biome.minTemp) return biome;
        }
        return biomes[0];
    }

    void GenerateCaves()
    {
        int segmentSize = worldWidth / numberOfCaves;
        numberOfCaves = (int) Mathf.Floor(worldWidth / Random.Range(30, 55));

        for (int i = 0; i < numberOfCaves; i++)
        {
            int startX = Random.Range(i * segmentSize, (i + 1) * segmentSize);
            startX = Mathf.Clamp(startX, 0, worldWidth);

            int startY = surfaceHeightCache[i];

            DigWorm(startX, startY);
        }
    }

    void DigWorm(int startX, int startY)
    {
        Vector2 curPos = new Vector2(startX, startY);

        float noiseOffset = Random.Range(0f, 1000f);

        for (int step = 0; step < caveLength; step++)
        {
            float noiseVal = Mathf.PerlinNoise(step * caveTurnFreq, noiseOffset);
            float angle = Mathf.Lerp(170f, 370f, noiseVal);

            Vector2 direction = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
            
            curPos += direction;

            CarveCircle(Mathf.RoundToInt(curPos.x), Mathf.RoundToInt(curPos.y), caveWidth);

            if (curPos.y < 2 || curPos.x < 0 || curPos.x >= worldWidth) break;
        }
    }

    void CarveCircle(int centerX, int centerY, int radius)
    {
        for (int x = centerX - radius; x <= centerX + radius; x++)
        {
            for (int y = centerY - radius; y <= centerY + radius; y++)
            {
                if (Vector2.Distance(new Vector2(centerX, centerY), new Vector2(x, y)) <= radius)
                {
                    tilemap.SetTile(new Vector3Int(x, y, 0), null);
                }
            }
        }
    }
}