using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RecipeSlot : MonoBehaviour
{
    public Image icon;
    public RecipeData data;
    public TextMeshProUGUI itemName;
    public TextMeshProUGUI ingredient1;
    public TextMeshProUGUI ingredient2;

    public void Add(RecipeData itemData)
    {
        // Store item data and update UI
        data = itemData;
        if (data.isMaterial)
        {
            switch (data.material)
            {
                case GameManager.Material.BlackSteel:
                    icon.sprite = WorkbenchManager.instance.materialSprites[0];
                    break;
                case GameManager.Material.Plastic:
                    icon.sprite = WorkbenchManager.instance.materialSprites[1];
                    break;
                case GameManager.Material.Polymers:
                    icon.sprite = WorkbenchManager.instance.materialSprites[2];
                    break;
                case GameManager.Material.Scraps:
                    icon.sprite = WorkbenchManager.instance.materialSprites[3];
                    break;
            }
        }
        else icon.sprite = data.output.icon;
        icon.enabled = true;

        itemName.text = data.name;
        ingredient1.text = data.requiredMaterials[0].ToString()+ ". Quantity: " + data.materialQuantities[0].ToString();
        if (data.requiredMaterials.Count > 1) ingredient2.text = data.requiredMaterials[1].ToString()+ ". Quantity: " + data.materialQuantities[1].ToString();
    }

    public void CallCrafting()
    {
        HubManager.instance.CraftItem(data);
    }
}