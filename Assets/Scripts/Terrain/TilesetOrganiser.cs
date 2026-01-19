using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEditor.SpeedTree.Importer;
using Unity.Collections;

public class TilesetOrganiser : MonoBehaviour
{
    [Header("References")]
    public WorldGenerator worldGenerator;
    private Tilemap curMap;

    [System.Serializable]
    public class BlockSet
    {
        public string name;
        [Range(0f, 100f)] public float weight;

        public TileBase tile1x1;
        public TileBase[] tile2x2;
        public TileBase[] tile3x3;
    }

    // Track visited
    private bool[,] visited;
    private BoundsInt bounds;

    public void ProcessMap(Tilemap tilemap)
    {
        curMap = tilemap;
        curMap.CompressBounds();
        bounds = curMap.cellBounds;
        visited = new bool[bounds.size.x, bounds.size.y];

        List<Vector3Int> allGroundTiles = new List<Vector3Int>();

        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                if (curMap.HasTile(new Vector3Int(x, y, 0)))
                {
                    allGroundTiles.Add(new Vector3Int(x, y, 0));
                }
            }
        }

        Shuffle(allGroundTiles);

        foreach (Vector3Int pos in allGroundTiles)
        {
            if (IsVisited(pos)) continue;

            int surfaceHeight = worldGenerator.surfaceHeightCache[pos.x];
            float temperature = worldGenerator.temperatureCache[pos.x];
            WorldGenerator.Biome currentBiome = worldGenerator.GetBiome(temperature);
            int surfaceRndOffset = UnityEngine.Random.Range(0, 2);

            if (pos.y == surfaceHeight || pos.y == surfaceHeight - surfaceRndOffset)
            {
                curMap.SetTile(pos, currentBiome.surfaceTile);
                MarkVisited(pos);
            }
            else
            {
                BlockSet[] palette = (surfaceHeight - pos.y < worldGenerator.subSurfaceDepth) ? currentBiome.subSurfaceSets : currentBiome.deepCaveSets;
                BlockSet selectedMaterial = PickRandomMaterial(palette);

                if (CanFit(pos, 3, surfaceRndOffset))
                {
                    PlaceBlock(pos, 3, selectedMaterial.tile3x3);
                }
                else if (CanFit(pos, 2, surfaceRndOffset))
                {
                    PlaceBlock(pos, 2, selectedMaterial.tile2x2);
                }
                else
                {
                    curMap.SetTile(pos, selectedMaterial.tile1x1);
                    MarkVisited(pos);
                }
            }

        }
    }

    private bool CanFit(Vector3Int startPos, int size, int surfaceOffset)
    {
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                Vector3Int checkPos = startPos + new Vector3Int(x, y, 0);

                if (!curMap.HasTile(checkPos)) return false;
                if (IsVisited(checkPos)) return false;
                if (checkPos.y >= worldGenerator.surfaceHeightCache[checkPos.x] - surfaceOffset - 1) return false;
            }
        }

        return true;
    }

    private void PlaceBlock(Vector3Int startPos, int size, TileBase[] tiles)
    {
        int i = 0;
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                Vector3Int currentPos = startPos + new Vector3Int(x, y, 0);

                if (i < tiles.Length)
                {
                    curMap.SetTile(currentPos, tiles[i]);
                }

                MarkVisited(currentPos);
                i++;
            }
        }
    }

    private void Shuffle<T>(List<T> list)
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

    private BlockSet PickRandomMaterial(BlockSet[] options)
    {
        if (options == null) return null;
        float totalWeight = options.Sum(m => m.weight);
        float randomPoint = UnityEngine.Random.value * totalWeight;

        foreach (var mat in options)
        {
            if (randomPoint < mat.weight) return mat;
            randomPoint -= mat.weight;
        }

        return options[0];
    }

    private bool IsVisited(Vector3Int pos)
    {
        return visited[pos.x - bounds.xMin, pos.y - bounds.yMin];
    }

    private void MarkVisited(Vector3Int pos)
    {
        int arrayX = pos.x - bounds.xMin;
        int arrayY = pos.y - bounds.yMin;
        visited[arrayX, arrayY] = true;
    }
}