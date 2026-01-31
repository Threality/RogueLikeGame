using System.CodeDom.Compiler;
using System.Collections.Generic;
using UnityEngine;

public class ItemGenerator : MonoBehaviour
{
    [Header("Items")]
    public List<ItemData> availableItems = new List<ItemData>();

    public List<ItemData> GenerateRandomChestLoot(float difficulty)
    {
        List<ItemData> items = new List<ItemData>();
        items.Add(GetRandomWeightedItem(difficulty));

        for (int i = 0; i < 10; i++)
        {
            if (Random.value < 0.1f) items.Add(GetRandomWeightedItem(difficulty));
        }

        return items;
    }

    private ItemData GetRandomWeightedItem(float difficulty)
    {
        // Create a dictionary of all valid availableItems
        Dictionary<ItemData, float> weights = new Dictionary<ItemData, float>();
        float totalWeight = 0f;

        // Loop through all the availableItems and add their weights to the dictionary
        foreach (var item in availableItems)
        {
            float curWeight = item.spawnWeight;

            switch (item.rarity)
            {
                // Adjust weighting based on rarity and difficulty
                // Check out animation curve later
                case GameManager.Rarity.Common:
                    if (difficulty > 2.5f) curWeight /= 2f;
                    break;
                case GameManager.Rarity.Uncommon:
                    if (difficulty <= 2.5f && difficulty >= 1.5f) curWeight *= 1.5f;
                    else if (difficulty > 2.5f) curWeight *= 2/3;
                    break;
                case GameManager.Rarity.Rare:
                    if (difficulty < 2f) curWeight /= 2f;
                    break;
                case GameManager.Rarity.Epic:
                    if (difficulty < 2.5f) curWeight /= 3f;
                    if (difficulty >= 2.5f) curWeight *= 2f;
                    break;
                case GameManager.Rarity.Legendary:
                    if (difficulty < 2f) curWeight = 0f;
                    else curWeight *=  difficulty / 3;
                    break;
                case GameManager.Rarity.Mythical:
                    if (difficulty < 3f) curWeight = 0f;
                    else curWeight *= difficulty / 6;
                    break;
            }

            // Add it to the dictionary
            weights.Add(item, curWeight);
            totalWeight += curWeight;
        }

        // Generate a random value
        float rand = Random.Range(0, totalWeight);
        float runningTotal = 0f;

        // Tally up until the random value is reached
        foreach (var weight in weights)
        {
            runningTotal += weight.Value;
            if (rand <= runningTotal)
            {  
                // Select the data for the item
                return weight.Key;
            }
        }

        // Return a default value
        Debug.LogWarning("Failed to find weighted item");
        return availableItems[0];
    }
}