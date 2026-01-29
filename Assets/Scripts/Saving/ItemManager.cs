using System.Collections.Generic;
using UnityEngine;

public class AssetManager : MonoBehaviour
{
    public static AssetManager instance;

    [Header("Database")]
    public List<ItemData> allItems;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    public ItemData GetItemByName(string name)
    {
        return allItems.Find(i => i.itemName == name);
    }
}