using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System;

public class SaveManager : MonoBehaviour
{
    public static SaveManager instance;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    public void SaveGame()
    {
        // Create the save data
        SaveGameData save = new SaveGameData();

        save.scraps = HubManager.instance.scraps;
        save.polymers = HubManager.instance.polymers;
        save.blackSteel = HubManager.instance.blackSteel;
        save.plastic = HubManager.instance.plastic;

        save.craftingLevel = HubManager.instance.craftingLevel;
        save.miningLevel = HubManager.instance.miningLevel;

        save.hubInventoryItems = new List<string>();
        save.playerInventoryItems = new List<string>();

        save.galacticDepth = GameManager.instance.GetGalacticDepth();

        // Add item names
        foreach (ItemData item in HubManager.instance.hubChest.inventory)
        {
            if (item != null) save.hubInventoryItems.Add(item.itemName);
        }

        foreach (ItemData item in InventoryManager.instance.inventory)
        {
            if (item != null) save.playerInventoryItems.Add(item.itemName);
        }

        // Write to json
        string json = JsonUtility.ToJson(save, true);
        string path = Application.persistentDataPath + "/savefile.json";
        File.WriteAllText(path, json);

        Debug.Log("Game saved with path" + path);
    }

    public void LoadGame()
    {
        // Attempt to find save data
        string path = Application.persistentDataPath + "/savefile.json";
        if (!File.Exists(path))
        {
            Debug.Log("No save data found");
            return;
        }

        // Load json
        string json = File.ReadAllText(path);
        SaveGameData save = JsonUtility.FromJson<SaveGameData>(json);

        // Load data back into the game
        HubManager hub = HubManager.instance;
        hub.scraps = save.scraps;
        hub.polymers = save.polymers;
        hub.plastic = save.plastic;
        hub.blackSteel = save.blackSteel;

        hub.craftingLevel = save.craftingLevel;
        hub.miningLevel = save.miningLevel;

        GameManager.instance.SetGalacticDepth(save.galacticDepth + 1);

        // Read items back into the game using the asset manager
        InventoryManager.instance.inventory = new List<ItemData>();
        foreach (String itemName in save.playerInventoryItems)
        {
            ItemData item = AssetManager.instance.GetItemByName(itemName);
            if (item != null) InventoryManager.instance.Add(item);
        }

        hub.hubChest.inventory = new List<ItemData>();
        foreach (String itemName in save.hubInventoryItems)
        {
            ItemData item = AssetManager.instance.GetItemByName(itemName);
            if (item != null) hub.hubChest.AddToInventory(item);
        }

        Debug.Log("Save loaded");
    }

    public void DeleteSave()
    {
        // Create path to location
        string path = Application.persistentDataPath + "/savefile.json";

        // Delete the file if it is there
        if (File.Exists(path))
        {
            File.Delete(path);
            Debug.Log("Save deleted");
        }
    }

    public bool HasSave()
    {
        string path = Application.persistentDataPath + "/savefile.json";
        return File.Exists(path);
    }
}