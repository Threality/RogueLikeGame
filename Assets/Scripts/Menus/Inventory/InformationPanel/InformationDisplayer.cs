using UnityEngine;
using TMPro;

public class InformationDisplayer : MonoBehaviour
{
    #region Setup

    [Header("References")]
    public Transform informationParent;
    public TextMeshProUGUI itemNameText;
    public GameObject slotPrefab;

    [Header("Data")]
    public TextMeshProUGUI[] displaySlots;
    public int NUM_SLOTS = 12;

    void Start()
    {
        for (int i = 0; i < NUM_SLOTS; i++)
        {
            Instantiate(slotPrefab, informationParent);
        }

        displaySlots = informationParent.GetComponentsInChildren<TextMeshProUGUI>();
    }


    #endregion
    #region UI

    public void LoadItem(ItemData data)
    {
        if (!data) return;

        // Load guaranteed data
        itemNameText.text = data.itemName;   
        displaySlots[0].text = data.rarity.ToString();

        // Exit early if additional data is not required
        if (!(data.isUsable && data.isWeapon)) return;

        displaySlots[1].text = data.damage.ToString();
    }

    public void ClearSlots()
    {
        itemNameText.text = "";
        
        foreach (TextMeshProUGUI slot in displaySlots)
        {
            slot.text = "";
        }
    }
    #endregion
}