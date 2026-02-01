using System.Collections.Generic;

[System.Serializable]
public class SaveGameData
{
    // Resources
    public int scraps;
    public int polymers;
    public int blackSteel;
    public int plastic;

    // Hub
    public int craftingLevel;
    public int miningLevel;

    // Hub Inventory
    public List<string> hubInventoryItems;

    // Player Inventory
    public List<string> playerInventoryItems;

    // Game Stats
    public int galacticDepth;
}