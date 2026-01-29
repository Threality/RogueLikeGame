using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName= "NewEnemy", menuName = "Enemies/EnemyData")]
public class EnemyData : ScriptableObject
{
    [Header("Visuals")]
    public string enemyName;
    public GameObject prefab;
    public GameObject GraphicsPrefab;

    [Header("Stats")]
    public float moveSpeed;
    public float acceleration;
    public int damage;
    public int MAX_HEALTH;

    [Header("Spawn Stats")]
    [Range(0f, 100f)] public float spawnWeight;
    public GameManager.Rarity rarity;
    public EnemyManager.AIType aIType;
    public List<GameManager.Biomes> spawnBiomes;

    [Header("Spawn Essentials")]
    public float width;
    public float height;

    [Header("Loot")]
    public int polymerValue;
}