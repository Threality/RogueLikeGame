using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IPointerEnterHandler
{
    #region Setup

    public Image icon;
    public ItemData data;
    public int slotNum;
    public InformationDisplayer informationDisplayer;
    public InventoryManager inventoryManager;


    #endregion
    #region Interaction


    public void Add(ItemData itemData)
    {
        // Store item data and update UI
        data = itemData;
        icon.sprite = data.icon;
        icon.enabled = true;
    }

    public void Clear()
    {
        // Remove all item data
        data = null;
        icon.sprite = null;
        icon.enabled = false;
    }

    public void OnClick()
    {
        // Exit early if item cannot be equipped
        if (!data) return;
        if (!data.isUsable) return;

        inventoryManager.EquipItem(slotNum);
    }

    public void SetColor(Color color)
    {
        // Set alpha to 1 to avoid issues with transparency
        color.a = 1f;
        GetComponent<Image>().color = color;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        informationDisplayer.LoadItem(data);
    }

    #endregion
}