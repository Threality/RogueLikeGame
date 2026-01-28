using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Recipe", menuName = "Hub/Recipe")]
public class RecipeData : ScriptableObject
{
    [Header("Recipe Output")]
    public bool isMaterial;
    public GameManager.Material material;
    public int outputQuantity;
    public ItemData output;

    [Header("Recipe Input")]
    public List<GameManager.Material> requiredMaterials = new List<GameManager.Material>();
    public List<int> materialQuantities = new List<int>();

    [Header("Crafting Requirements")]
    public int requiredCraftingLevel;
}