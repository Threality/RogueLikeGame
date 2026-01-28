using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HubManager : MonoBehaviour
{
    public static HubManager instance;

    [Header("Hub Levels")]
    public int craftingLevel = 1;
    public int miningLevel = 1;

    [Header("Hub Materials")]
    public int blackSteel = 0;
    public int plastic = 0;
    public int scraps = 0;
    public int polymers = 0;
    
    [Header("Crafting Recipes")]
    public List<RecipeData> recipes = new List<RecipeData>();

    [Header("Current Stats")]
    public int miningSpeed = 1;

    // Data
    private float miningProgress = 0f;

    private void Awake()
    {
        // Check it is the only instance
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    private void Update()
    {
        miningProgress += miningSpeed * Time.deltaTime;

        if (miningProgress >= 1)
        {
            miningProgress--;
            scraps++;
        }
    }

    public List<RecipeData> GetAvailableRecipes()
    {
        return recipes
            .Where(x => x.requiredCraftingLevel <= craftingLevel)
            .ToList();
    }

    private void CraftItem(RecipeData recipe)
    {
        for (int i = 0; i < recipe.requiredMaterials.Count; i++)
        {
            if (!HasSufficient(recipe.requiredMaterials[i], recipe.materialQuantities[i])) return;
        }

        for (int i = 0; i < recipe.requiredMaterials.Count; i++)
        {
            AddMaterial(recipe.requiredMaterials[i], recipe.materialQuantities[i] * -1);
        }

        if (recipe.isMaterial)
        {
            AddMaterial(recipe.material, recipe.outputQuantity);
        }
    }

    private bool HasSufficient(GameManager.Material material, int quantity)
    {
        switch (material)
        {
            case GameManager.Material.BlackSteel:
                return blackSteel >= quantity;
            case GameManager.Material.Plastic:
                return plastic >= quantity;
            case GameManager.Material.Polymers:
                return polymers >= quantity;
            case GameManager.Material.Scraps:
                return scraps >= quantity;
        }
        return false;
    }

    private bool AddMaterial(GameManager.Material material, int quantity)
    {
        switch (material)
        {
            case GameManager.Material.BlackSteel:
                blackSteel += quantity;
                break;
            case GameManager.Material.Plastic:
                plastic += quantity;
                break;
            case GameManager.Material.Polymers:
                polymers += quantity;
                break;
            case GameManager.Material.Scraps:
                scraps += quantity;
                break;
        }
        return false;
    }
}