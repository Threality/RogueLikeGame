using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public Image icon;
    public ItemData data;
    public int slotNum;
    public InformationDisplayer informationDisplayer;

    public void Add(ItemData itemData)
    {
        data = itemData;
        icon.sprite = data.icon;
        icon.enabled = true;
    }

    public void Clear()
    {
        data = null;
        icon.sprite = null;
        icon.enabled = false;
    }

    public void DisplayInformation()
    {
        informationDisplayer.LoadItem(data);
    }
}