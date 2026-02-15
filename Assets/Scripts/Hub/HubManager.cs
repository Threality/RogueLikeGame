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
    public int ladderCost = 2;

    [Header("References")]
    public ChestData hubChest;
    public GameObject hub;

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
        // Increment mining progress
        miningProgress += miningLevel * Time.deltaTime * 2;

        // Increase scrap if enough progress has been made
        if (miningProgress >= 1)
        {
            miningProgress--;
            scraps++;
            MenuManager.instance.UpdateUI();
        }
    }

    public List<int> GetHubMaterials()
    {
        List<int> mats = new List<int>
        {
            blackSteel,
            plastic,
            polymers,
            scraps
        };
        return mats;
    }

    public List<RecipeData> GetAvailableRecipes()
    {
        // Get all recipes the player can craft
        return recipes
            .Where(x => x.requiredCraftingLevel <= craftingLevel)
            .ToList();
    }

    public float GetQuota()
    {
        return Mathf.Round(25 * Mathf.Pow(1.5f, GameManager.instance.GetGalacticDepth()));
    }

    public float GetCraftingUpgradeCost()
    {
        return Mathf.Round(50 * Mathf.Pow(4f, craftingLevel - 1));
    }

    public float GetMiningUpgradeCost()
    {
        return Mathf.Round(30 * Mathf.Pow(2f, miningLevel - 1));
    }

    public void CraftItem(RecipeData recipe)
    {
        // Check the player has enough resources
        for (int i = 0; i < recipe.requiredMaterials.Count; i++)
        {
            if (!HasSufficient(recipe.requiredMaterials[i], recipe.materialQuantities[i]))
            {
                // Tell the user why nothing happened
                MenuManager.instance.AlertUser("Not enough resources");
                return;
            }
        }

        // Take away the resources
        for (int i = 0; i < recipe.requiredMaterials.Count; i++)
        {
            AddMaterial(recipe.requiredMaterials[i], recipe.materialQuantities[i] * -1);
        }

        // Add the material crafted to the hub
        if (recipe.isMaterial)
        {
            AddMaterial(recipe.material, recipe.outputQuantity);
        }

        MenuManager.instance.UpdateUI();
    }

    public void UpgradeCrafting()
    {
        if (plastic >= GetCraftingUpgradeCost())
        {
            // Increment level and charge mats
            plastic -= (int) GetCraftingUpgradeCost();
            craftingLevel++;
            
            // Tell the user and update UI
            MenuManager.instance.AlertUser("Upgraded crafting!");
            MenuManager.instance.UpdateUI();
            WorkbenchManager.instance.CloseWorkbench();
            WorkbenchManager.instance.OpenWorkbench();
        }
        else
        {
            // Tell the user they dont have enough materials
            MenuManager.instance.AlertUser("Not enough plastic!");
        }
    }

    public void UpgradeDrill()
    {
        if (plastic >= GetMiningUpgradeCost())
        {
            // Increment level and charge mats
            plastic -= (int) GetMiningUpgradeCost();
            miningLevel++;

            // Alert user
            MenuManager.instance.AlertUser("Upgraded drill!");
            MenuManager.instance.UpdateUI();
        }
        else
        {
            // Give feedback
            MenuManager.instance.AlertUser("Not enough plastic!");
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

    public bool AddMaterial(GameManager.Material material, int quantity)
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

    public void PlacedLadderCost()
    {
        scraps -= ladderCost;
    }
}