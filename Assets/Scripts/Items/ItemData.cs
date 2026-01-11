using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    // Store basic required information
    [Header("Information")]
    public string itemName;
    public Sprite icon;

    // Info for spawning
    [Header("Spawning")]
    public GameManager.Rarity rarity;
    [Range(0f, 100f)] public float spawnWeight;
    public List<GameManager.Biomes> spawnBiomes;

    // Item statistics
    [Header("Stats")]
    public bool isUsable;
    public bool isWeapon;
    public int damage;
    public float hitboxWidth;
    public float hitboxHeight;
    public GameObject projectile;
}