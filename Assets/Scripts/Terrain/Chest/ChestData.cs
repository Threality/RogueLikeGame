using UnityEngine;
using System.Collections.Generic;

public class ChestData : MonoBehaviour
{
    // Store inventory data
    [Header("Data")]
    public List<ItemData> inventory = new  List<ItemData>();
    public int space = 20;
    public int scrapValue = 10;
    public int polymerValue = 5;

    public void Initialise(List<ItemData> items, int scrapVal, int polymerVal)
    {
        inventory = items;
        scrapValue = scrapVal;
        polymerValue = polymerVal;
    }

    public void RemoveFromInventory(ItemData item)
    {
        inventory.Remove(item);
    }

    public void AddToInventory(ItemData item)
    {
        inventory.Add(item);
    }

    public bool HasSpace()
    {
        return space > inventory.Count;
    }
}